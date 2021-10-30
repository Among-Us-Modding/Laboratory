using HarmonyLib;
using Laboratory.Mods.Effects.Utils;
using UnityEngine;

namespace Laboratory.Mods.CustomMap.Patches
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