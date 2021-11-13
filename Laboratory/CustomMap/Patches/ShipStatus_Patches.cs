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
            var sprites = textureSwapBundle.LoadAllAssets().OfIl2CppType<Sprite>().ToList();
            SpriteSwapper.Swap(__instance.gameObject, sprites);
        }


        // Map specific
        if (__instance.Type == ShipStatus.MapType.Pb)
        {
            if (MapConfig.LavaColors != default) LavaColorChanger.ChangeColor(MapConfig.LavaColors);
            if (MapConfig.DisableSnow) Object.FindObjectOfType<SnowManager>()?.gameObject.SetActive(false);
        }
    }
}

[HarmonyPatch]
public static class ShipStatus_OnEnable_Patch
{
    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(ShipStatus), nameof(ShipStatus.OnEnable));
        yield return AccessTools.Method(typeof(PolusShipStatus), nameof(PolusShipStatus.OnEnable));
        yield return AccessTools.Method(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable));
    }

    [HarmonyPrefix]
    public static void Prefix(ShipStatus __instance, out bool __state)
    {
        __state = __instance.Systems == null && MapConfig.CustomSystems != default;
    }

    [HarmonyPostfix]
    public static void Postfix(ShipStatus __instance, bool __state)
    {
        if (!__state) return;

        var allTypes = SystemTypeHelpers.AllTypes.ToHashSet();
        var castMethod = AccessTools.Method(typeof(Il2CppObjectBase), nameof(Il2CppObjectBase.TryCast));
        foreach (var customSystem in MapConfig.CustomSystems)
        {
            allTypes.Add(customSystem.Key);
            // __instance.Systems[customSystem.Key] = (ISystemType) castMethod.MakeGenericMethod(typeof(ISystemType)).Invoke(Activator.CreateInstance(customSystem.Value), Array.Empty<object>());
            __instance.Systems[customSystem.Key] = ((Il2CppObjectBase)Activator.CreateInstance(customSystem.Value)).TryCast<ISystemType>();
        }

        SystemTypeHelpers.AllTypes = allTypes.ToArray();
    }
}