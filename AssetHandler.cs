using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AtlyssEmotes
{
    public static class AssetHandler
    {
        public static Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

        /*public static T Fetch<T>(string key)
        {
            return Addressables.LoadAssetAsync<T>((object)key).WaitForCompletion();
        }*/

        public static void GetAssetBundle(string bundlename)
        {
            Plugin.Logger.LogInfo("Loading bundle: " + bundlename);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Utilities.path, bundlename));
            bundles.Add(bundlename, assetBundle);
            Plugin.Logger.LogInfo("Bundle " + bundlename + " Loaded!");
            Plugin.Logger.LogInfo(bundles[bundlename].name);
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
