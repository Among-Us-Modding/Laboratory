using HarmonyLib;

namespace Laboratory.Patches.Misc
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    public static class ShipStatus_Awake_Patch
    {
        /// <summary>
        /// Sets correct MapType on airship
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        public static void Prefix(ShipStatus __instance)
        {
            var airship = __instance.TryCast<AirshipStatus>();
            if (airship) airship.Type = (ShipStatus.MapType) 3;
        }
    }
}