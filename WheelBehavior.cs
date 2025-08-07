using System;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using AtlyssEmotes;
using TMPro;
using Nessie.ATLYSS.EasySettings.UIElements;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AtlyssEmotes
{
    public class WheelBehavior : MonoBehaviour
    {
        public static WheelBehavior instance;

        public WheelConfig Config;
        public int NumOfEmotes;
        public GameObject IconPrefab;
        public float WheelRadius;
        public float LerpSpeed;
        public float TextDistance;
        public TextMeshProUGUI emoteNameText;

        [SerializeField]
        bool InUnityEdtior;

        private WheelIcon[] Icons;
        private Vector2 centeredMousePosition;
        private float mouseAngle;
        private float openProgress;
        private int selectedIndex;
        private bool active;

        private Canvas canvas;
        private ChatBehaviour chat;

        public void Start()
        {
            NumOfEmotes = Settings.NumOfEmotes.Value;
            if(Config != null)
            {
                Destroy(Config);
            }
            Config = ScriptableObject.CreateInstance<WheelConfig>();

            Config.list = new List<ScriptableEmote>();


            if (Icons != null)
            {
                foreach (WheelIcon icon in Icons)
                {
                    Destroy(icon.gameObject);
                }
            }
            Icons = new WheelIcon[NumOfEmotes];
            for (int i = 0; i < NumOfEmotes; i++)
            {
                if (EmoteManager.allEmotes.ContainsKey(Settings.emotes[i].Value))
                {
                    Config.list.Add(EmoteManager.allEmotes[Settings.emotes[i].Value]);
                }
                else
                {
                    Config.list.Add(EmoteManager.allEmotes["None"]);
                }
                WheelIcon e = Instantiate(IconPrefab, gameObject.transform).GetComponent<WheelIcon>();
                e.index = i;
                e.Emote = Config.list[i];
                Icons[i] = e;
            }

            if(emoteNameText)
            {
                emoteNameText.GetComponent<RectTransform>().anchoredPosition = TextDistance * Vector2.up;
            }

            canvas = GetComponent<Canvas>();
            if (instance == null)
            {
                instance = this;
            }
            if (!InUnityEdtior)
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
                if(!InUnityEdtior)
                {
                    CameraFunction._current._unlockedCamera = true;
                }
                


                openProgress = Mathf.Lerp(openProgress, 1f, 1 - Mathf.Pow(LerpSpeed, Time.deltaTime));

                //get mouse pos relative to center
                centeredMousePosition = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);

                //get mouse angle relative to center with 0 radians being pi/6 left to upwards and + radians being clockwise
                mouseAngle = (Mathf.PI / 2) + (Mathf.PI / NumOfEmotes)- Mathf.Atan2(centeredMousePosition.y, centeredMousePosition.x);

                //get mouse angle from 0 to 6, 0 being 0 radians and 6 being 2 radians
                float normalizedMouseAngle = mouseAngle / (2 * Mathf.PI) * NumOfEmotes;
                if (normalizedMouseAngle < 0)
                {
                    normalizedMouseAngle += NumOfEmotes;
                }

                selectedIndex = Mathf.FloorToInt(normalizedMouseAngle);


                if (Input.GetMouseButtonUp(0))
                {
                    if(InUnityEdtior)
                    {
                        UnityEngine.Debug.Log(selectedIndex);
                    }
                    else
                    {
                        Plugin.Logger.LogInfo(Settings.Mouth.Value);
                        active = false;
                        PlayEmote(selectedIndex);
                        CameraFunction._current._unlockedCamera = false;
                    }
                }
                emoteNameText.text = Config.list[selectedIndex].emoteName;
            }
            else
            {
                openProgress = Mathf.Lerp(openProgress, 0f, 1 - Mathf.Pow(LerpSpeed, Time.deltaTime));
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
                if (!ChatBehaviour._current._focusedInChat && Input.GetKeyDown(Settings.EmoteWheelKey.Value) || (active && Input.GetKeyDown(KeyCode.Escape)))
                {
                    active = !active;
                    //crude, redo
                    if (!active)
                    {
                        CameraFunction._current._unlockedCamera = false;
                    }
                }
                if (SettingsManager._current._isOpen || DialogManager._current._isDialogEnabled || TabMenu._current._isOpen)
                {
                    active = false;
                }
            }

            canvas.enabled = openProgress > 0.1f;

            foreach (WheelIcon ei in Icons)
            {
                ei.selected = (ei.index == selectedIndex);
                ei.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Sin(ei.index * 2 * Mathf.PI / NumOfEmotes),
                                                                                Mathf.Cos(ei.index * 2 * Mathf.PI / NumOfEmotes)) * openProgress * WheelRadius;
            }
            emoteNameText.color = new Color(emoteNameText.color.r,
                                            emoteNameText.color.g,
                                            emoteNameText.color.b,
                                            openProgress);
            

        }


        void PlayEmote(int index)
        {
            if (InUnityEdtior)
            {
                Debug.Log(Config.list[index].emoteID);
            }
            else if (chat._emoteBuffer <= 0f)
            {
                string id = Config.list[index].isVanilla ? Config.list[index].emoteID : Config.list[index].GetName();


                
                chat._player._pVisual.Cmd_CrossFadeAnim(id, 0.1f, 11);
                chat._player._pVisual.Local_CrossFadeAnim(id, 0.1f, 11);
                chat._emoteBuffer = 0.85f;
            }
        }


        [HarmonyPatch]
        class Patches
        {
            [HarmonyPatch(typeof(PlayerCombat), "<Init_Attack>g__AbleToInitAttack|102_0")]
            [HarmonyPostfix]
            static void PatchCanAttack(ref bool __result)
            {

                if (WheelBehavior.instance != null && WheelBehavior.instance.active)
                {
                    __result = false;
                }
            }
        }
    }
}