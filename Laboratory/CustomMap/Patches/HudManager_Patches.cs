using HarmonyLib;
using Laboratory.Effects.Utils;
using UnityEngine;

namespace Laboratory.CustomMap.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
public static class HudManager_Start_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        Camera.main!.gameObject.AddComponent<CameraZoomController>();
    }
}
