using System;
using HarmonyLib;
using Laboratory.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Laboratory.Patches.RemoveAccounts
{
    [HarmonyPatch(typeof(AccountManager), nameof(AccountManager.CanPlayOnline))]
    public static class AccountManager_CanPlayOnline_Patch
    {
        /// <summary>
        /// Allows online play regardless of sign in status
        /// </summary>
        [HarmonyPrefix]
        private static bool Prefix(out bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(AccountManager), nameof(AccountManager.Awake))]
    public static class AccountManager_Awake_Patch
    {
        /// <summary>
        /// Disables the AccountManager and adds a new name text box
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix(AccountManager __instance)
        {
            __instance.gameObject.SetActive(false);
        }
    }
}