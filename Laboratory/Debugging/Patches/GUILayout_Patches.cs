using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace Laboratory.Debugging.Patches;

[HarmonyPatch]
internal static class UnstrippingPatches
{
    [HarmonyPatch(typeof(GUILayout), nameof(GUILayout.DoTextField), typeof(string), typeof(int), typeof(bool), typeof(GUIStyle), typeof(Il2CppReferenceArray<GUILayoutOption>))]
    [HarmonyPrefix]
    public static bool GUILayoutDoTextFieldPatch(GUILayout __instance, out string __result, string text, int maxLength, bool multiline, GUIStyle style, Il2CppReferenceArray<GUILayoutOption> options)
    {
        int controlId = GUIUtility.GetControlID(FocusType.Keyboard);
        GUIContent.Temp(text);
        GUIContent content = GUIUtility.keyboardControl == controlId ? GUIContent.Temp(text + GUIUtility.compositionString) : GUIContent.Temp(text);
        Rect rect = GUILayoutUtility.GetRect(content, style, options);
        if (GUIUtility.keyboardControl == controlId) content = GUIContent.Temp(text);
        GUI.DoTextField(rect, controlId, content, multiline, maxLength, style);
        __result = content.text;
        return false;
    }

    [HarmonyPatch(typeof(GUIUtility), nameof(GUIUtility.GetControlID), typeof(FocusType))]
    [HarmonyPrefix]
    public static bool GUIUtilityGetControlIDPatch(GUIUtility __instance, out int __result, FocusType focus)
    {
        Rect rect = new(0, 0, 0, 0);
        __result = GUIUtility.GetControlID_Injected(0, focus, ref rect);
        return false;
    }
}