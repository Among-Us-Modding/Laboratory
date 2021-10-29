using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace Laboratory.Patches.RemoveAccounts
{
    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.InitializePlatformInterface))]
    public static class EOSManager_InitializePlatformInterface_Patch
    {
        /// <summary>
        /// Sets all required properties/fields as if the game is logged in
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        public static bool Prefix(EOSManager __instance)
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
    }

    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsAllowedOnline))]
    public static class EOSManager_IsAllowedOnline_Patch
    {
        /// <summary>
        /// Re-enables the play online button
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix()
        {
            GameObject playButton = EOSManager.Instance.FindPlayOnlineButton();
            playButton.GetComponent<SpriteRenderer>().color = Color.white;
            playButton.GetComponent<PassiveButton>().enabled = true;
            playButton.GetComponent<ButtonRolloverHandler>().SetEnabledColors();
            return false;
        }
    }

    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.ProductUserId), MethodType.Getter)]
    public static class EOSManager_get_ProductUserId_Patch
    {
        /// <summary>
        /// Sets fake player id
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix(out string __result)
        {
            __result = "Crewmate";
            return false;
        }
    }
    
    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.LoginForKWS))]
    public static class EOSManager_LoginForKWS_Patch
    {
        /// <summary>
        /// Disables Epic Online Login
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}