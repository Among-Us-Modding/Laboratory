using HarmonyLib;

namespace Laboratory.Patches.NoBan;

[HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
public static class StatsManager_get_AmBanned_Patch
{
    /// <summary>
    /// Stops bans from leaving games too quickly
    /// </summary>
    /// <param name="__result"></param>
    [HarmonyPostfix]
    public static void Postfix(out bool __result)
    {
        __result = false;
    }
}