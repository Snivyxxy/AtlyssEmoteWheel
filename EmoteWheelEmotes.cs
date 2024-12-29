using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmoteWheelEmotes", menuName = "ScriptableObjects/Custom/EmoteWheelEmotes", order = 1)]
public class EmoteWheelEmotes : ScriptableObject
{
    public List<ScriptableEmote> list;
}
