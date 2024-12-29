using AtlyssEmotes;
using BepInEx.Configuration;
using BepInEx;
using Nessie.ATLYSS.EasySettings;
using UnityEngine;

public static class EmoteWheelSettings
{
    private static ConfigFile saveFile;
    public static ConfigEntry<KeyCode> EmoteWheelKey { get; private set; }
    public static ConfigEntry<bool> Mouth { get; private set; }
    // Start is called before the first frame update
    public static void Awake()
    {
        Plugin.Logger.LogInfo("Initializing Settings");
        saveFile = (Plugin.plugin).Config;

        InitConfiguration();

        Settings.OnInitialized.AddListener(AddSettings);
        Settings.OnApplySettings.AddListener(Apply);
        
    }

    private static void Apply()
    {
        saveFile.Save();
        if (Plugin.Mouth != null)
        {
            Plugin.Mouth.SetActive(Mouth.Value);
        }
    }


    private static void InitConfiguration()
    {
        ConfigDefinition saveDefinition = new ConfigDefinition("Keys", "EmoteWheel");
        ConfigDescription saveDescription = new ConfigDescription("Key to pull up the Emote Wheel.");
        EmoteWheelKey = saveFile.Bind(saveDefinition, KeyCode.RightBracket, saveDescription);
        ConfigDefinition definition = new ConfigDefinition("Toggles", "EmoteWheel");
        ConfigDescription description = new ConfigDescription("Toggles Live Mouth Reaction");
        Mouth = saveFile.Bind(definition, false, description);
    }

    private static void AddSettings()
    {
        SettingsTab tab = Settings.ModTab;
        tab.AddHeader("Emote Wheel");
        tab.AddKeyButton("Emote Wheel Key", EmoteWheelKey);
        tab.AddToggle("Live Mouth Reaction", Mouth);
    }

}
