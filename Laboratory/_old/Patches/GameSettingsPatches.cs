using System.Linq;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class GameSettingsPatches
{
    [HarmonyPatch(typeof(CustomPlayerMenu), nameof(CustomPlayerMenu.Start))]
    [HarmonyPostfix]
    public static void HideLobbyStoreButtonPatch(CustomPlayerMenu __instance)
    {
        __instance.GetComponentsInChildren<PassiveButton>(true)
            .Where(t => t.name == "StoreButton")
            .Do(t => t.gameObject.SetActive(false));
    }
    
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
        __instance.TimeSinceLastMessage = 5;
    }
}