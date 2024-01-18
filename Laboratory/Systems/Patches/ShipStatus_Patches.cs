using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes;
using Laboratory.Map;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace Laboratory.Systems.Patches;

[HarmonyPatch]
public static class SystemPatches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Serialize))]
    [HarmonyPrefix]
    public static bool Serialize(ShipStatus __instance, MessageWriter writer, bool initialState, out bool __result)
    {
        bool result = false;
        for (short num = 0; num < SystemTypeHelpers.AllTypes.Length; num++)
        {
            SystemTypes systemTypes = SystemTypeHelpers.AllTypes[num];
            if (__instance.Systems.TryGetValue(systemTypes, out var value) && (initialState || value.IsDirty))
            {
                result = true;
                writer.StartMessage((byte)systemTypes);
                if (value.TryCast<Object>() is ICustomSystemType sys) sys.Serialize(writer, initialState);
                else value.Serialize(writer, initialState);
                writer.EndMessage();
            }
        }
        __result = result;
        return false;
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Deserialize))]
    [HarmonyPrefix]
    public static bool Deserialize(ShipStatus __instance, MessageReader reader, bool initialState)
    {
        while (reader.Position < reader.Length)
        {
            MessageReader messageReader = reader.ReadMessage();
            SystemTypes key = (SystemTypes)messageReader.Tag;
            if (__instance.Systems.TryGetValue(key, out var value))
            {
                if (value.TryCast<Object>() is ICustomSystemType sys) sys.Deserialize(messageReader, initialState);
                else value.Deserialize(messageReader, initialState);
            }
        }

        return false;
    }
    
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
    [HarmonyPrefix]
    public static bool FixedUpdate(ShipStatus __instance)
    {
        if (!AmongUsClient.Instance || !PlayerControl.LocalPlayer)
        {
            return false;
        }
        __instance.Timer += Time.fixedDeltaTime;
        __instance.EmergencyCooldown -= Time.fixedDeltaTime;
        if ((bool)GameData.Instance)
        {
            GameData.Instance.RecomputeTaskCounts();
        }
        if (!AmongUsClient.Instance.AmClient)
        {
            return false;
        }
        for (int i = 0; i < SystemTypeHelpers.AllTypes.Length; i++)
        {
            SystemTypes key = SystemTypeHelpers.AllTypes[i];
            if (__instance.Systems.TryGetValue(key, out var value))
            {
                if (value.TryCast<Object>() is ICustomSystemType sys) sys.Detoriorate(Time.fixedDeltaTime);
                else value.Deteriorate(Time.fixedDeltaTime);
            }
        }

        return false;
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
        yield return AccessTools.Method(typeof(FungleShipStatus), nameof(FungleShipStatus.OnEnable));
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
