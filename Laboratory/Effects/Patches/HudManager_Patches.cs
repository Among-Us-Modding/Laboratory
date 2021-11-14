using HarmonyLib;
using Laboratory.Effects.Managers;
using UnityEngine;

namespace Laboratory.Effects.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
public static class HudManager_Start_Patch
{
    public static void Postfix()
    {
        if (!GlobalEffectManager.Instance) GlobalEffectManager.Instance = new GameObject("GlobalEffects").AddComponent<GlobalEffectManager>();
    }
}