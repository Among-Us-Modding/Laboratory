using HarmonyLib;
using UnityEngine;

namespace Laboratory.Patches.RemoveAccounts;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.GiveFocus))]
public static class TextBoxTMP_GiveFocus_Patch
{
    /// <summary>
    /// Removed focus when having multiple active text boxes seen on MMOnline scene
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPrefix]
    public static void Prefix(TextBoxTMP __instance)
    {
        foreach (var textBox in Object.FindObjectsOfType<TextBoxTMP>())
        {
            if (textBox.GetHashCode() != __instance.GetHashCode()) textBox.LoseFocus();
        }
    }
}