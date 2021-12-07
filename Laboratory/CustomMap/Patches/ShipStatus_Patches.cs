using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Laboratory.Extensions;
using Reactor.Extensions;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Laboratory.CustomMap.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ShipStatus __instance)
    {
        if (MapConfig.DisableShadows)
        {
            HudManager.Instance.ShadowQuad.material.color = Color.clear;

            foreach (var collider2D in Object.FindObjectsOfType<Collider2D>())
            {
                if (collider2D.gameObject.layer == 10) collider2D.isTrigger = true;
            }

            Material newMat = new(Shader.Find("Sprites/Default"));
            foreach (var shadow in Object.FindObjectsOfType<NoShadowBehaviour>())
            {
                var rend = shadow.GetComponent<SpriteRenderer>();
                shadow.Destroy();
                rend.material = newMat;
            }
        }

        if (MapConfig.DisableRoomTracker) HudManager.Instance.roomTracker.text.renderer.enabled = false;

        foreach (var textureSwapBundle in MapConfig.TextureSwapBundles)
        {
            if (!MapConfig.SwapRawTextures)
            {
                var sprites = textureSwapBundle.LoadAllAssets().OfIl2CppType<Sprite>().ToList();
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

[HarmonyPatch]
public static class ShipStatus_OnEnable_Patch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(ShipStatus), nameof(ShipStatus.OnEnable));
        yield return AccessTools.Method(typeof(PolusShipStatus), nameof(PolusShipStatus.OnEnable));
        yield return AccessTools.Method(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable));
    }

    public static void Prefix(ShipStatus __instance, out bool __state)
    {
        __state = __instance.Systems == null;
    }

    public static void Postfix(ShipStatus __instance, bool __state)
    {
        if (!__state) return;

        foreach (var customSystemType in CustomSystemType.List)
        {
            __instance.Systems[customSystemType] = ((Il2CppObjectBase)Activator.CreateInstance(customSystemType.Value)).TryCast<ISystemType>();
        }
    }
}
