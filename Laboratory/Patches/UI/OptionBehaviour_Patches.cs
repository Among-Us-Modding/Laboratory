using HarmonyLib;

namespace Laboratory.Patches.UI;

[HarmonyPatch(typeof(OptionBehaviour), nameof(OptionBehaviour.SetAsPlayer))]
public static class OptionBehaviour_SetAsPlayer_Patch
{
    /// <summary>
    /// Disables buttons in game options when not host
    /// </summary>
    [HarmonyPrefix]
    public static bool Prefix(OptionBehaviour __instance)
    {
        if (!__instance.TryCast<ToggleOption>()) return true;
        
        __instance.GetComponentsInChildren<PassiveButton>().Do(t => t.enabled = false);

        return false;
    }
}