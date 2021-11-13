using HarmonyLib;
using UnityEngine;

namespace Laboratory.Patches.Unstrip;

[HarmonyPatch(typeof(GUIUtility), nameof(GUIUtility.GetControlID), typeof(FocusType))]
public static class GUIUtility_GetControlId_FocusType_Patch
{
    public static bool Prefix(GUIUtility __instance, ref int __result, FocusType focus)
    {
        Rect rect = new(0, 0, 0, 0);
        __result = GUIUtility.GetControlID_Injected(0, focus, ref rect);
        return false;
    }
}