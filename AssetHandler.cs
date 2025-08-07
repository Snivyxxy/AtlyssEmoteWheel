using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AtlyssEmotes
{
    public static class AssetHandler
    {
        public static Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

        
        public static void GetAssetBundle(string bundlename)
        {
            Plugin.Logger.LogInfo("Loading bundle: " + bundlename);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Utilities.path, bundlename));
            bundles.Add(bundlename, assetBundle);
            Plugin.Logger.LogInfo("Bundle " + bundlename + " Loaded!");
        }

        public static void RegisterAllEmotePacks()
        {
            Plugin.Logger.LogInfo("Loading All EmotePacks...");
            Plugin.Logger.LogInfo("Loading Base EmotePack");
            EmoteManager.RegisterEmotePackage(FetchFromBundle<EmotePackage>("emotewheel", "BasePack"));
            Plugin.Logger.LogInfo("Base EmotePack Loaded!");
            Plugin.Logger.LogInfo("Finding Other EmotePacks...");
            AssetHandler.RegisterAllInPath(Utilities.pluginsFolder);
        }
        
        public static void RegisterEmotePacksInPath(string path)
        {
            Plugin.Logger.LogInfo("Loading EmotePacks at Path: " + path);

            

            foreach (string s in Directory.GetFiles(path))
            {
                RegisterEmotePack(s);
            }
            Plugin.Logger.LogInfo("EmotePacks in path: " + path + " Loaded!");
        }
        

        public static void RegisterAllInPath(string path)
        {
            foreach(string s in Directory.GetDirectories(path))
            {
                if (Path.GetFileName(s).Equals(Utilities.EmoteDirectoryName))
                {
                    RegisterEmotePacksInPath(Path.Combine(path, Utilities.EmoteDirectoryName));
                }
                else
                {
                    RegisterAllInPath(s);
                }
            }
        }


        public static void RegisterEmotePack(string bundlepath)
        {
            string bundlename = Path.GetFileName(bundlepath);
            Plugin.Logger.LogInfo("Loading EmotePack: " + bundlename);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlepath);

            EmotePackage Pack = assetBundle.LoadAsset<EmotePackage>("_Package");

            if(Pack == null)
            {
                Plugin.Logger.LogInfo("Couldn't find _Package Object. Skipping...");
                assetBundle.Unload(false);
                return;
            }

            EmoteManager.RegisterEmotePackage(Pack);

            Plugin.Logger.LogInfo("EmotePack " + bundlename + " Loaded!");
            assetBundle.Unload(false);
        }

        public static void GetAssetBundles()
        {
            GetAssetBundle("emotewheel");
        }

        public static T FetchFromBundle<T>(string bundle, string key) where T : Object
        {
            return bundles[bundle].LoadAsset<T>(key);
        }
    }
}
