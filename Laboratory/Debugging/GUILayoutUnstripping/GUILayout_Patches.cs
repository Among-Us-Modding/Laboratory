using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace Laboratory.Debugging.GUILayoutUnstripping
{
    [HarmonyPatch(typeof(GUILayout), nameof(GUILayout.DoTextField), typeof(string), typeof(int), typeof(bool), typeof(GUIStyle), typeof(Il2CppReferenceArray<GUILayoutOption>))]
    public static class GUILayout_DoTextField_Patch
    {
        public static bool Prefix(GUILayout __instance, ref string __result, string text, int maxLength, bool multiline, GUIStyle style, Il2CppReferenceArray<GUILayoutOption> options)
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
    }
}