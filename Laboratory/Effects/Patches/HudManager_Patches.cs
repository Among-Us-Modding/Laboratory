using HarmonyLib;
using Laboratory.Effects.Managers;
using Laboratory.Effects.Utils;
using UnityEngine;

namespace Laboratory.Effects.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
public static class HudManager_Start_Patch
{
    public static void Postfix()
    {
        Camera.main!.gameObject.AddComponent<CameraZoomController>();
        if (!GlobalEffectManager.Instance) GlobalEffectManager.Instance = new GameObject("GlobalEffects").AddComponent<GlobalEffectManager>();
    }
}