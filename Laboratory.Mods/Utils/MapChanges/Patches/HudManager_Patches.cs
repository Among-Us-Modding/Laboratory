using HarmonyLib;
using Laboratory.Mods.Utils.General;
using UnityEngine;

namespace Laboratory.Mods.Utils.MapChanges.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class HudManager_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Camera.main!.gameObject.AddComponent<CameraZoomController>();
        }
    }
}