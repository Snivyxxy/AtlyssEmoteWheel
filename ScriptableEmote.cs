using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Emote", menuName = "ScriptableObjects/Custom/Emote", order = 1)]
public class ScriptableEmote : ScriptableObject
{
    public Sprite icon;
    public string emoteString;
}