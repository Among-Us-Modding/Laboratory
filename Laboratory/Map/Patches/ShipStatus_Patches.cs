using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Laboratory.Config;
using Laboratory.Extensions;
using Reactor.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Laboratory.Map.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ShipStatus __instance)
    {
        if (MapConfig.DisableShadows)
        {
            HudManager.Instance.ShadowQuad.material.color = Color.clear;

            foreach (Collider2D collider2D in Object.FindObjectsOfType<Collider2D>())
            {
                if (collider2D.gameObject.layer == 10) collider2D.isTrigger = true;
            }

            Material newMat = new(Shader.Find("Sprites/Default"));
            foreach (NoShadowBehaviour shadow in Object.FindObjectsOfType<NoShadowBehaviour>())
            {
                SpriteRenderer rend = shadow.GetComponent<SpriteRenderer>();
                shadow.Destroy();
                rend.material = newMat;
            }
        }

        if (MapConfig.DisableRoomTracker) HudManager.Instance.roomTracker.text.renderer.enabled = false;

        foreach (AssetBundle textureSwapBundle in MapConfig.TextureSwapBundles)
        {
            if (!MapConfig.SwapRawTextures)
            {
                List<Sprite> sprites = textureSwapBundle.LoadAllAssets().OfIl2CppType<Sprite>().ToList();
                SpriteSwapper.Swap(__instance.gameObject, sprites);
            }
            else
            {
                SpriteSwapper.SwapMapSpritesRaw(textureSwapBundle);
            }
        }

        // Map specific
        if (__instance.Type == ShipStatus.MapType.Pb)
        {
            LavaColorChanger.ChangeColors(MapConfig.LavaColors);
            if (MapConfig.DisableSnow) Object.FindObjectOfType<SnowManager>()?.gameObject.SetActive(false);
        }
    }
}
