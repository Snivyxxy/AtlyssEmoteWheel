using AtlyssEmotes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtlyssEmotes
{
    [CreateAssetMenu(fileName = "EmoteWheelEmotes", menuName = "ScriptableObjects/Custom/EmoteWheelEmotes", order = 1)]
    public class WheelConfig : ScriptableObject
    {
        public List<ScriptableEmote> list;
    }
}
