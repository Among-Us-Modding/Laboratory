using HarmonyLib;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class FancyCamsPatches
{
    public static bool ForceRenderTextureSize { get; set; }
    
    [HarmonyPatch(typeof(RenderTexture), nameof(RenderTexture.GetTemporary), typeof(int), typeof(int), typeof(int), typeof(RenderTextureFormat))]
    [HarmonyPrefix]
    public static void ForceCameraTextureSizePatch([HarmonyArgument(0)] ref int width, [HarmonyArgument(1)] ref int height)
    {
        if (!ForceRenderTextureSize) return;

        if (Screen.width > Screen.height)
        {
            height = (int) (height / (float) width * Screen.width);
            width = Screen.width;
        }
        else
        {
            height = Screen.height;
            width = (int) (width / (float) height * Screen.height) ;
        }
    }
    
    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Begin))]
    [HarmonyPrefix]
    public static void EnableForceCamsTextureSizePatch()
    {
        ForceRenderTextureSize = true;
    }
        
    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Begin))]
    [HarmonyPostfix]
    public static void DisableForceCamsTextureSizePatch()
    {
        ForceRenderTextureSize = false;
    }
}