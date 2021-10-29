using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace Laboratory.Patches.UI
{
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
    public static class GameSettingMenu_OnEnable_Patch
    {
        /// <summary>
        /// Shows map and impostor selectors in game options menu
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        public static void Prefix(GameSettingMenu __instance)
        {
            __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
        }
    }
}