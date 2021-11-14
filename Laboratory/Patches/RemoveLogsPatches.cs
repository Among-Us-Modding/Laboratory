using HarmonyLib;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class RemoveLogsPatches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
    [HarmonyPrefix]
    public static bool NoValidMenuActivePatch()
    {
        return false;
    }
}