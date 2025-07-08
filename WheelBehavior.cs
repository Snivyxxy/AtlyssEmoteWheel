using System;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using AtlyssEmotes;

public class WheelBehavior : MonoBehaviour
{
    public static WheelBehavior instance;
    [Header("Emote Icons")]
    public Image[] EmoteBackgrounds;
    public Image[] EmoteIcons;
    public EmoteWheelEmotes Emotes;

    public Sprite EmoteUnselected;
    public Sprite EmoteSelected;

    public float WheelRadius;
    public float LerpSpeed;

    [SerializeField]
    bool InUnityEdtior;

    private Vector2 centeredMousePosition;
    private float mouseAngle;
    private float openProgress;
    private int selectedIndex;
    private bool active;

    private Canvas canvas;
    private ChatBehaviour chat;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        if(instance == null)
        {
            instance = this;
        }
        if(!InUnityEdtior)
        {
            chat = ChatBehaviour._current;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (active)
        {
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CameraFunction._current._unlockedCamera = true;


            openProgress = Mathf.Lerp(openProgress, 1f, 1f - Mathf.Pow(1 - (1 / LerpSpeed), Time.deltaTime));

            //get mouse pos relative to center
            centeredMousePosition = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);

            //get mouse angle relative to center with 0 radians being pi/6 left to upwards and + radians being clockwise
            mouseAngle = (2 * Mathf.PI / 3) - Mathf.Atan2(centeredMousePosition.y, centeredMousePosition.x);

            //get mouse angle from 0 to 6, 0 being 0 radians and 6 being 2 radians
            float normalizedMouseAngle = mouseAngle / (2 * Mathf.PI) * 6;
            if (normalizedMouseAngle < 0)
            {
                normalizedMouseAngle += 6;
            }

            selectedIndex = Mathf.FloorToInt(normalizedMouseAngle);


            if (Input.GetMouseButtonUp(0))
            {
                Plugin.Logger.LogInfo(EmoteWheelSettings.Mouth.Value);
                active = false;
                PlayEmote(selectedIndex);
                CameraFunction._current._unlockedCamera = false;
            }

        }
        else
        {
            openProgress = Mathf.Lerp(openProgress, 0f, 1f - Mathf.Pow(1 - (1 / LerpSpeed), Time.deltaTime));
        }

        if (InUnityEdtior)
        {
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                active = !active;
            }
        }
        else
        {
            if (!ChatBehaviour._current._focusedInChat && Input.GetKeyDown(EmoteWheelSettings.EmoteWheelKey.Value) || (active && Input.GetKeyDown(KeyCode.Escape)))
            {
                active = !active;
                //crude, redo
                if(!active)
                {
                    CameraFunction._current._unlockedCamera = false;
                }
            }
            if (SettingsManager._current._isOpen || DialogManager._current._isDialogEnabled || TabMenu._current._isOpen)
            {
                active = false;
            }
        }


        if (openProgress > 0.1f)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }

        for (int i = 0; i < EmoteBackgrounds.Length; i++)
        {
            //setactive
            EmoteBackgrounds[i].gameObject.SetActive(openProgress > 0.1f);

            if (i == selectedIndex)
            {
                EmoteBackgrounds[i].sprite = EmoteSelected;
            }
            else
            {
                EmoteBackgrounds[i].sprite = EmoteUnselected;
            }

            EmoteBackgrounds[i].transform.localPosition = new Vector2(openProgress * WheelRadius * Mathf.Sin(i * Mathf.PI / 3),
                                                                      openProgress * WheelRadius * Mathf.Cos(i * Mathf.PI / 3));

            if (active)
            {
                EmoteIcons[i].sprite = Emotes.list[i].icon;
            }
        }


    }


    void PlayEmote(int index)
    {
        if (InUnityEdtior)
        {
            Debug.Log(Emotes.list[index].emoteString);
        }
        else
        {
            chat.Cmd_SendChatMessage(Emotes.list[index].emoteString, ChatBehaviour.ChatChannel.ROOM);
        }
    }

    //crude. Fix later.
    [HarmonyPatch]
    class Patches
    {
        [HarmonyPatch(typeof(PlayerCombat), "<Init_Attack>g__AbleToInitAttack|101_0")]
        [HarmonyPostfix]
        static void PatchCanAttack(ref bool __result)
        {
            
            if(WheelBehavior.instance != null && WheelBehavior.instance.active)
            {
                __result = false;
            }
        }
    }
}
