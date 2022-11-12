using HarmonyLib;
using UnityEngine;

namespace Laboratory.Debugging.Patches;

[HarmonyPatch(typeof(GUIUtility), nameof(GUIUtility.GetControlID), typeof(FocusType))]
internal static class GUIUtility_GetControlID_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(GUIUtility __instance, out int __result, FocusType focus)
    {
        Rect rect = new(0, 0, 0, 0);
        __result = GUIUtility.GetControlID_Injected(0, focus, ref rect);
        return false;
    }
}