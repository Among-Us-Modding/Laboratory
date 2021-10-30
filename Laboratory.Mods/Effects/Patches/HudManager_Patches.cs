using HarmonyLib;
using Laboratory.Mods.Effects.MonoBehaviours;
using UnityEngine;

namespace Laboratory.Mods.Effects.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class HudManager_Start_Patch
    {
        public static void Postfix()
        {
            if (!GlobalEffectManager.Instance) GlobalEffectManager.Instance = new GameObject("GlobalEffects").AddComponent<GlobalEffectManager>();
        }
    }
}