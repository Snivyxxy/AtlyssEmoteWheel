using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;

namespace AtlyssEmotes
{
    public class PlayerVisualPatcher : MonoBehaviour
    {
        PlayerVisual target;
        void Awake()
        {
            target = GetComponent<PlayerVisual>();
        }

        void Update()
        {
            if(target._visualAnimator != null)
            {
                AddMoreEmotes(target._visualAnimator);
                Destroy(this);
            }

        }
        
        public static void AddMoreEmotes(Animator anim)
        {
            if(anim.GetComponent<EmoteOverrider>() == null)
            {
                anim.gameObject.AddComponent<EmoteOverrider>();
            }
        }

        [HarmonyPatch]
        public static class Patches
        {
            [HarmonyPatch(typeof(PlayerVisual),"Awake")]
            [HarmonyPrefix]
            static void AddPatcher(PlayerVisual __instance)
            {
                __instance.gameObject.AddComponent<PlayerVisualPatcher>();
            }
        }
    }
}
