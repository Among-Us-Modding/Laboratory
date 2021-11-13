using HarmonyLib;

namespace Laboratory.Patches.UI;

[HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.OpenTopmostMenu))]
public static class ControllerManager_OpenTopmostMenu_Patch
{
    /// <summary>
    /// Stops UI elements being selected (and thus green) by default
    /// </summary>
    /// <returns></returns>
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return false;
    }
}