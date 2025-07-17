using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AtlyssEmotes
{
    [CreateAssetMenu(fileName = "Emote", menuName = "ScriptableObjects/Custom/EmotePackage", order = 1)]
    public class EmotePackage : ScriptableObject
    {
        public string packID;
        
        public List<ScriptableEmote> emotes;


    }
}
