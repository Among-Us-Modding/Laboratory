using System.IO;
using HarmonyLib;
using InnerNet;
using Steamworks;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class RemoveAccountsPatches
{
    [HarmonyPatch(typeof(AccountManager), nameof(AccountManager.CanPlayOnline))]
    [HarmonyPrefix]
    private static bool CanPlayOnlinePatch(out bool __result)
    {
        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(AccountManager), nameof(AccountManager.Awake))]
    [HarmonyPrefix]
    public static void DisableAccountManagerPatch(AccountManager __instance)
    {
        __instance.gameObject.SetActive(false);
    }
    
    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.InitializePlatformInterface))]
    [HarmonyPrefix]
    public static bool ForceCorrectAccountStatePatch(EOSManager __instance)
    {
        SaveManager.LoadPlayerPrefs();
        SaveManager.hasLoggedIn = true;
        SaveManager.isGuest = false;
        SaveManager.ChatModeType = QuickChatModes.FreeChatOrQuickChat;
        SaveManager.SavePlayerPrefs();

        __instance.ageOfConsent = 0;
        __instance.loginFlowFinished = true;
        __instance.platformInitialized = true;

        __instance.gameObject.SetActive(false);
        return false;
    }

    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsAllowedOnline))]
    [HarmonyPrefix]
    public static bool EnableOnlineButtonPatch()
    {
        var playButton = EOSManager.Instance.FindPlayOnlineButton();
        playButton.GetComponent<SpriteRenderer>().color = Color.white;
        playButton.GetComponent<PassiveButton>().enabled = true;
        playButton.GetComponent<ButtonRolloverHandler>().SetEnabledColors();
        return false;
    }
    
    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.LoginForKWS))]
    [HarmonyPrefix]
    public static bool LoginForKWSPatch()
    {
        return false;
    }
    
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.BirthDateYear), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool ForceBirthdayPatch(out int __result)
    {
        __result = 1990;
        return false;
    }
    
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.ChatModeType), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool ForceChatModePatch(out QuickChatModes __result)
    {
        __result = QuickChatModes.FreeChatOrQuickChat;
        return false;
    }
    
    [HarmonyPatch(typeof(SteamAPI), nameof(SteamAPI.RestartAppIfNecessary))]
    [HarmonyPrefix]
    public static bool RestartAppIfNessesaryPatch(out bool __result)
    {
        if (!File.Exists("steam_appid.txt")) File.WriteAllText("steam_appid.txt", "945360");
        return __result = false;
    }
    
    [HarmonyPatch(typeof(SteamAPI), nameof(SteamAPI.Init))]
    [HarmonyPrefix]
    public static bool SteamAPIInitPatch() => false;
}