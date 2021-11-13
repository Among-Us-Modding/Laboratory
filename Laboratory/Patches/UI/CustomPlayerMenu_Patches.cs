using System.Linq;
using HarmonyLib;

namespace Laboratory.Patches.UI;

[HarmonyPatch(typeof(CustomPlayerMenu), nameof(CustomPlayerMenu.Start))]
public static class CustomPlayerMenu_Start_Patch
{
    /// <summary>
    /// Hides store button in cosmetics picker
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    public static void Postfix(CustomPlayerMenu __instance)
    {
        __instance.GetComponentsInChildren<PassiveButton>(true)
            .Where(t => t.name == "StoreButton")
            .Do(t => t.gameObject.SetActive(false));
    }
}