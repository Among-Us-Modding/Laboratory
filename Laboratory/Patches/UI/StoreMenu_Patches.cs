using HarmonyLib;

namespace Laboratory.Patches.UI;

[HarmonyPatch(typeof(StoreMenu), nameof(StoreMenu.Open))]
public static class StoreMenu_Open_Patch
{
    /// <summary>
    /// Disables the store
    /// </summary>
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return false;
    }
}