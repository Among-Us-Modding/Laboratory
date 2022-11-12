using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Laboratory.Extensions;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Laboratory.Map;

public static class SpriteSwapper
{
    public static void Swap(GameObject gameObject, List<Sprite> replacementSprites)
    {
        foreach (SpriteRenderer renderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            Sprite sprite = renderer.sprite;
            if (!sprite) continue;

            Sprite replacement = replacementSprites.FirstOrDefault(s => s.name == sprite!.name && s.texture.name == sprite.texture.name);
            if (replacement)
            {
                renderer.sprite = replacement;
            }
        }

        CreateGround(replacementSprites);
    }

    public static void SwapMapSpritesRaw(AssetBundle bundle)
    {
        IEnumerable<Texture2D> newTex = bundle.LoadAllAssets().OfIl2CppType<Texture2D>();
        newTex = newTex.OrderBy(t => t.name.Length);

        Il2CppArrayBase<SpriteRenderer> mapRends = ShipStatus.Instance.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in mapRends)
        {
            if (spriteRenderer.sprite && spriteRenderer.sprite.texture)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                Texture2D match = newTex.FirstOrDefault(t => t.name.Replace('_', '-').Contains(spriteRenderer.sprite.texture.name.Replace('_', '-') + '-'));
                if (match)
                {
                    MaterialPropertyBlock block = new();
                    block.AddTexture("_MainTex", match);
                    spriteRenderer.SetPropertyBlock(block);
                }
            }
        }

        CreateGround(bundle.LoadAllAssets().OfIl2CppType<Sprite>().ToList());
    }

    public static void CreateGround(List<Sprite> sprites)
    {
        if (ShipStatus.Instance.Type == ShipStatus.MapType.Pb)
        {
            float z = -1f;

            foreach (Sprite groundSprite in sprites.Where(s => s.name.Contains("Ground")))
            {
                GameObject.Find("NewMapGround")?.Destroy();
                
                Transform background = ShipStatus.Instance.transform.Find("Background");
                Transform ground = new GameObject() { layer = 9, name = "NewMapGround" }.transform;
                ground.parent = background;
                ground.localPosition = new Vector3(19.97f, -13.5f, z);
                ground.localScale = new Vector3(0.3915f, 0.3915f, 1f);
                SpriteRenderer groundRend = ground.gameObject.AddComponent<SpriteRenderer>();
                groundRend.sprite = groundSprite;

                z -= 0.001f;
            }

            return;
        }

        if (ShipStatus.Instance.Type == ShipStatus.MapType.Ship)
        {
            Sprite groundSprite = sprites.FirstOrDefault(s => s.name.Contains("SkeldFloor"));
            if (groundSprite)
            {
                GameObject.Find("NewMapGround")?.Destroy();

                Transform background = ShipStatus.Instance.transform.Find("Hull2");
                background.GetComponent<MeshRenderer>().enabled = false;
                Transform ground = new GameObject() { layer = 9, name = "NewMapGround" }.transform;
                ground.parent = background;
                ground.localPosition = new Vector3(10.64f, -8.46f, -0.001f);
                ground.localScale = new Vector3(0.1644f, 0.1644f, 1f);
                SpriteRenderer groundRend = ground.gameObject.AddComponent<SpriteRenderer>();
                groundRend.sprite = groundSprite;
            }

            return;
        }
    }
}
