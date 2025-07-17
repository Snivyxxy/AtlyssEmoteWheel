
using UnityEngine;
using UnityEngine.UI;

namespace AtlyssEmotes
{
    public class WheelIcon : MonoBehaviour
    {
        public int index;
        public Image background;
        public Image icons;

        public bool selected;

        public ScriptableEmote Emote;


        void Update()
        {
            if (Emote != null)
            {
                background.sprite = selected ? Emote.backgroundSelected : Emote.background;
                icons.sprite = Emote.icon;
            }
        }
    }
}
