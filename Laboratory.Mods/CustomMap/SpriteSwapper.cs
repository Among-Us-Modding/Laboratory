using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;

namespace Laboratory.Mods.CustomMap
{
    public static class SpriteSwapper
    {
        public static void Swap(GameObject gameObject, List<Sprite> replacementSprites)
        {
            Il2CppArrayBase<SpriteRenderer> rends = gameObject.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer rend in rends)
            {
                Sprite rendSprite = rend.sprite;
                if (!rendSprite) continue;
                Sprite swap = replacementSprites.FirstOrDefault(s => s.name == rendSprite.name && s.texture.name == rendSprite.texture.name);

                if (swap)
                {
                    rend.sprite = swap;
                }
            }
        }
    }
}