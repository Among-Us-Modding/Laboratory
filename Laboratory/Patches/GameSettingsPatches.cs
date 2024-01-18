using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class GameSettingsPatches
{
    [HarmonyPatch(typeof(OptionBehaviour), nameof(OptionBehaviour.SetAsPlayer))]
    [HarmonyPrefix]
    public static bool DisableOptionsWhenNotHostPatch(OptionBehaviour __instance)
    {
        if (!__instance.TryCast<ToggleOption>()) return true;
        __instance.GetComponentsInChildren<PassiveButton>().Do(t => t.enabled = false);
        return false;
    }
    
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
    [HarmonyPrefix]
    public static void ShowAdditionalOptionsPatch(GameSettingMenu __instance)
    {
        __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
    }
    
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    [HarmonyPrefix]
    public static void ChatMessagesRatePatch(ChatController __instance)
    {
        __instance.timeSinceLastMessage = 5;
    }
}