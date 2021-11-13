using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Laboratory.CustomMap;

public static class SpriteSwapper
{
    public static void Swap(GameObject gameObject, List<Sprite> replacementSprites)
    {
        foreach (var renderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            var sprite = renderer.sprite;
            if (!sprite) continue;

            var replacement = replacementSprites.FirstOrDefault(s => s.name == sprite.name && s.texture.name == sprite.texture.name);
            if (replacement)
            {
                renderer.sprite = replacement;
            }
        }
    }
}
