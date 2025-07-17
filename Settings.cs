using AtlyssEmotes;
using BepInEx.Configuration;
using BepInEx;
using Nessie.ATLYSS.EasySettings;
using UnityEngine;
using System.Collections.Generic;
using Nessie.ATLYSS.EasySettings.UIElements;

namespace AtlyssEmotes
{
    public static class Settings
    {
        private static ConfigFile saveFile;
        public static ConfigEntry<KeyCode> EmoteWheelKey { get; private set; }
        public static ConfigEntry<bool> Mouth { get; private set; }

        public static ConfigEntry<int> NumOfEmotes { get; private set; }

        public static ConfigEntry<string>[] emotes { get; private set; }

        // Start is called before the first frame update
        public static void Awake()
        {
            Plugin.Logger.LogInfo("Initializing Settings");
            saveFile = (Plugin.plugin).Config;

            InitConfiguration();

            Nessie.ATLYSS.EasySettings.Settings.OnInitialized.AddListener(AddSettings);
            Nessie.ATLYSS.EasySettings.Settings.OnApplySettings.AddListener(Apply);

        }

        private static void Apply()
        {
            saveFile.Save();
            if (Plugin.Mouth != null)
            {
                Plugin.Mouth.SetActive(Mouth.Value);
            }
            if(WheelBehavior.instance != null)
            {
                WheelBehavior.instance.Start();
            }
        }


        private static void InitConfiguration()
        {
            ConfigDefinition saveDefinition = new ConfigDefinition("Keys", "EmoteWheelKey");
            ConfigDescription saveDescription = new ConfigDescription("Key to pull up the Emote Wheel.");
            EmoteWheelKey = saveFile.Bind(saveDefinition, KeyCode.RightBracket, saveDescription);

            ConfigDefinition MouthDefinition = new ConfigDefinition("Toggles", "LiveMouthReaction");
            ConfigDescription MouthDescription = new ConfigDescription("Toggles Live Mouth Reaction");
            Mouth = saveFile.Bind(MouthDefinition, false, MouthDescription); 

            ConfigDefinition NumEmotesdefinition = new ConfigDefinition("Ints", "NumOfEmotes");

            ConfigDescription NumEmotesdescription = new ConfigDescription("Number of Emotes in the Wheel", new AcceptableValueRange<int>(2, 12));
            NumOfEmotes = saveFile.Bind(NumEmotesdefinition, 6, NumEmotesdescription);

            emotes = new ConfigEntry<string>[12];

            for(int i = 0; i < 12; i++)
            {
                ConfigDefinition Emotesdefinition = new ConfigDefinition("Strs", "Emote" + i);

                ConfigDescription Emotesdescription = new ConfigDescription("Emote " + i + " in the wheel");
                emotes[i] = saveFile.Bind(Emotesdefinition, "None", Emotesdescription);
            }
        }


        static AtlyssDropdown AddDropdown(string label, List<string> options, string value, ConfigEntry<string> config)
        {
            int index = options.Contains(value) ? options.IndexOf(value) : 0;

            AtlyssDropdown element = Nessie.ATLYSS.EasySettings.Settings.ModTab.AddDropdown(label,options,index);
            element.OnValueChanged.AddListener(newValue => { config.Value = options[newValue]; });

            return element;
        }


        private static void AddSettings()
        {
            SettingsTab tab = Nessie.ATLYSS.EasySettings.Settings.ModTab;
            tab.AddHeader("Emote Wheel");
            tab.AddKeyButton("Emote Wheel Key", EmoteWheelKey);
            tab.AddToggle("Live Mouth Reaction", Mouth);

            tab.AddAdvancedSlider("Number Of Emotes", NumOfEmotes);

            for(int i = 0; i < 12; i++)
            {
                AddDropdown("Emote " + i, new List<string>(EmoteManager.allEmotes.Keys), emotes[i].Value, emotes[i]);
            }
        }

    }
}
