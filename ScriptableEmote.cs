using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace AtlyssEmotes
{

    [CreateAssetMenu(fileName = "Emote", menuName = "ScriptableObjects/Custom/Emote", order = 1)]
    public class ScriptableEmote : ScriptableObject
    {
        public Sprite icon;
        public Sprite background;
        public Sprite backgroundSelected;
        public string emoteName;
        public string emoteID;
        public bool isVanilla;
        [NonSerialized]
        public string packageName;

        public AnimationClip init;
        public AnimationClip loop;

        public float ExitTime;
        public float TransitionDuration;
        public float TransitionOffset;


        //AnimatorStateTransition trans;

        public string GetName()
        {
            if (name.Equals("None") && emoteID.Equals(""))
            {
                return "None";
            }
            return packageName + ":" + emoteName;
        }
    }
}