using System.Collections.Generic;
using System.Linq;
using Laboratory.Extensions;
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
    
    // TODO add filtering options
    public static void SwapMapSpritesRaw(AssetBundle bundle)
    {
        var newTex = bundle.LoadAllAssets().OfIl2CppType<Texture2D>();

        var mapRends = ShipStatus.Instance.GetComponentsInChildren<SpriteRenderer>();
        
        foreach (var spriteRenderer in mapRends)
        {
            if (spriteRenderer.sprite && spriteRenderer.sprite.texture)
            {
                var match = newTex.FirstOrDefault(t => t.name.Contains(spriteRenderer.sprite.texture.name + '-'));
                if (match)
                {
                    var block = new MaterialPropertyBlock();
                    block.AddTexture("_MainTex", match);
                    spriteRenderer.SetPropertyBlock(block);
                }
            }
        }

        // TODO add stuff here for other maps
        if (ShipStatus.Instance.Type == ShipStatus.MapType.Pb)
        {
            var groundSprite = bundle.LoadAllAssets().OfIl2CppType<Sprite>().FirstOrDefault(s => s.name.Contains("Polus_Ground"));
            if (groundSprite)
            {
                var background = ShipStatus.Instance.transform.Find("Background");
                var ground = new GameObject() { layer = 9, name = "Ground" }.transform;
                ground.parent = background;
                ground.localPosition = new Vector3(19.97f, -13.5f, -1f);
                ground.localScale = new Vector3(0.3915f, 0.3915f, 1f);
                var groundRend = ground.gameObject.AddComponent<SpriteRenderer>();
                groundRend.sprite = groundSprite;
            }
        }
    }
}
