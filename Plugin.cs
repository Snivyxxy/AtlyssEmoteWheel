using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace AtlyssEmotes;

[BepInPlugin("com.snivyxxy.plugins.atlyssemotes", "Atlyss Emotes", "1.0.3")]
[BepInDependency("EasySettings")]
[BepInProcess("ATLYSS.exe")]
public class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    public static Plugin plugin {get; private set;}

    private static Player player;

    public static GameObject Mouth;



    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        plugin = this;


        new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();

        EmoteWheelSettings.Awake();

        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        AssetHandler.GetAssetBundles();
    }

    [HarmonyPatch]
    private static class GetPlayer
    {
        [HarmonyPatch(typeof(Player), "OnGameConditionChange")]
        [HarmonyPostfix]
        private static void getPlayer(ref Player __instance, GameCondition _newCondition)
        {
            bool flag = __instance._currentGameCondition == GameCondition.IN_GAME && __instance.isLocalPlayer;
            if (flag)
            {
                Logger.LogInfo("Getting Player...");
                bool flag2 = player == null;
                if (flag2)
                {
                    Logger.LogInfo("Loading Assets...");
                    player = __instance;
                    GameObject mouth = AssetHandler.FetchFromBundle<GameObject>("emotewheel", "LiveMouthReaction");
                    GameObject emoteWheel = AssetHandler.FetchFromBundle<GameObject>("emotewheel", "EmoteWheel");
                    Mouth = Instantiate(mouth, player.gameObject.transform.Find("_Canvas_DynamicPlayerUI"));
                    Mouth.SetActive(EmoteWheelSettings.Mouth.Value);
                    Instantiate(emoteWheel, player.gameObject.transform);
                    Logger.LogInfo("Assets Loaded!");
                }
                bool flag3 = player != null;
                if (flag3)
                {
                    Logger.LogInfo("Player " + __instance.gameObject.name + " got!");
                }
                else
                {
                    Logger.LogError("Player wasn't got because it was null ;-;");
                }
            }
        }
    }
}
