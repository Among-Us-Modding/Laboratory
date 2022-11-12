using HarmonyLib;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class InnerslothsMistakePatches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    [HarmonyPrefix]
    public static void AirshipMapTypePatch(ShipStatus __instance)
    {
        AirshipStatus airship = __instance.TryCast<AirshipStatus>();
        if (airship) airship.Type = (ShipStatus.MapType) 3;
    }
}