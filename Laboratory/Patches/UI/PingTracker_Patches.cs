using HarmonyLib;

namespace Laboratory.Patches.UI;

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update_Patch
{
    /// <summary>
    /// Hide ping tracker
    /// </summary>
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void Postfix(PingTracker __instance)
    {
        __instance.gameObject.SetActive(false);
    }
}