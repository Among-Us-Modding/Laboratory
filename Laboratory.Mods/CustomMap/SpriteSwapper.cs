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
            var rends = gameObject.GetComponentsInChildren<SpriteRenderer>();

            foreach (var rend in rends)
            {
                var rendSprite = rend.sprite;
                if (!rendSprite) continue;
                var swap = replacementSprites.FirstOrDefault(s => s.name == rendSprite.name && s.texture.name == rendSprite.texture.name);

                if (swap)
                {
                    rend.sprite = swap;
                }
            }
        }
    }
}