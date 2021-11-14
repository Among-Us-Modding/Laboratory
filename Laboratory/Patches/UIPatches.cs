using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class UIPatches
{
    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.GiveFocus))]
    [HarmonyPrefix]
    public static void RefocusTextBoxPatch(TextBoxTMP __instance)
    {
        foreach (var textBox in Object.FindObjectsOfType<TextBoxTMP>())
        {
            if (textBox.GetHashCode() != __instance.GetHashCode()) textBox.LoseFocus();
        }
    }
    
    [HarmonyPatch(typeof(ActionMapGlyphDisplay), nameof(ActionMapGlyphDisplay.Awake))]
    [HarmonyPostfix]
    public static void HideControllerGlyphPatch(ActionMapGlyphDisplay __instance)
    {
        __instance.sr.gameObject.SetActive(false);
    }
    
    [HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.OpenTopmostMenu))]
    [HarmonyPrefix]
    public static bool StopAutomaticGreenHighlighting()
    {
        return false;
    }
    
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void DisablePingTrackerPatch(PingTracker __instance)
    {
        __instance.gameObject.SetActive(false);
    }
    
    [HarmonyPatch(typeof(StoreMenu), nameof(StoreMenu.Open))]
    [HarmonyPrefix]
    public static bool NoStorePatch()
    {
        return false;
    }
}