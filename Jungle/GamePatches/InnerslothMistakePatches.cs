using HarmonyLib;

namespace Jungle.GamePatches;

[HarmonyPatch]
internal static class InnerslothMistakePatches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    [HarmonyPrefix]
    public static void MapTypePatch(ShipStatus __instance)
    {
        var airship = __instance.TryCast<AirshipStatus>();
        if (airship) airship.Type = (ShipStatus.MapType) 4;
    }
}