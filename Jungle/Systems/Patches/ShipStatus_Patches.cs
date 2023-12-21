using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;

namespace Jungle.Systems.Patches;

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

        foreach (CustomSystemType customSystemType in CustomSystemType.List)
        {
            __instance.Systems[customSystemType] = ((Il2CppObjectBase)Activator.CreateInstance(customSystemType.Value)!).TryCast<ISystemType>();
        }
    }
}
