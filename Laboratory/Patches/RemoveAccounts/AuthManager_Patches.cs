using HarmonyLib;

namespace Laboratory.Patches.RemoveAccounts
{
    [HarmonyPatch(typeof(AuthManager._CoWaitForNonce_d__5), nameof(AuthManager._CoWaitForNonce_d__5.MoveNext))]
    public static class AuthManager_CoWaitForNonce_Patch
    {
        /// <summary>
        /// Disables 5 second wait when joining every game
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix(out bool __result)
        {
            return __result = false;
        }
    }
    
    [HarmonyPatch(typeof(AuthManager._CoConnect_d__4), nameof(AuthManager._CoConnect_d__4.MoveNext))]
    public static class AuthManager_CoConnect_Patch
    {
        /// <summary>
        /// Stops AuthManager from authenticating the server being connected to
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix(out bool __result)
        {
            return __result = false;
        }
    }
}