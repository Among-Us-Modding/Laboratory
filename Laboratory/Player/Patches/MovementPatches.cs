using HarmonyLib;
using Laboratory.Extensions;

namespace Laboratory.Player.Patches;

[HarmonyPatch]
internal static class MovementPatches
{
    public static PlayerPhysics LastPhysics = null;
    
    [HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
    [HarmonyPrefix]
    public static void PrepareForChangingTrueSpeed(CustomNetworkTransform __instance)
    {
        LastPhysics = __instance.GetCachedComponent<PlayerPhysics>();
    }
    
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.TrueSpeed), MethodType.Getter)]
    [HarmonyPrefix, HarmonyPriority(Priority.First)]
    public static bool GetActualTrueSpeed(ref float __result)
    {
        if (LastPhysics == null) return true;
            
        PlayerPhysics physics = LastPhysics;
        LastPhysics = null;
        __result = physics.TrueSpeed;
        return false;
    }
}