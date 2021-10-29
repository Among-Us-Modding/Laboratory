using HarmonyLib;

namespace Laboratory.Patches.AppData
{
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SavePlayerPrefs))]
    public static class SaveManager_SavePlayerPrefs_Patch
    {
        /// <summary>
        /// Fixes appdata issues when not using SteamAPI
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix()
        {
            if (SaveManager.lastLanguage == uint.MaxValue) SaveManager.lastLanguage = 0;
        }
    }

    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.GetPurchase))]
    public static class SaveManager_GetPurchase_Patch
    {
        /// <summary>
        /// Stops getting purchases
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix(out bool __result)
        {
            __result = true;
            return false;
        }
    }
    
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SetPurchased))]
    public static class SaveManager_SetPurchased_Patch
    {
        /// <summary>
        /// Stops setting purchases
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.LoadSecureData))]
    public static class SaveManager_LoadSecureData_Patch
    {
        /// <summary>
        /// Stops loading purchases
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SaveSecureData))]
    public static class SaveManager_SaveSecureData_Patch
    {
        /// <summary>
        /// Stops saving purchases
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}