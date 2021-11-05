using HarmonyLib;

namespace Laboratory.Patches.RemoveLogs
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
    public static class MainMenuManager_LateUpdate_Patch
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}