using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;

namespace AtlyssEmotes
{
    public static class EmoteManager
    {
        public static Dictionary<string, ScriptableEmote> allEmotes { get; private set; } = new Dictionary<string, ScriptableEmote>();
        
        public static List<string> publicNames { get; private set; }
        public static List<string> internalNames { get; private set; }

        public static List<string> registeredPackages { get; private set; } = new List<string>();
        
        public static void ClearEmoteData()
        {
            allEmotes.Clear();
            registeredPackages.Clear();
        }

        public static void RegisterEmotePackage(EmotePackage package)
        {

            if (!registeredPackages.Contains(package.packID))
            {
                registeredPackages.Add(package.packID);
            }
            foreach (ScriptableEmote emote in package.emotes)
            {
                RegisterEmote(package.packID, emote);
            }
        }
        
        public static void RegisterEmote(string package, ScriptableEmote emote)
        {
            emote.packageName = package;
            allEmotes[emote.GetName()] = emote;
        }

    }
}
