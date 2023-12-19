using HarmonyLib;
using UnityEngine;

namespace Jungle.GamePatches;

[HarmonyPatch]
public static class FancyPatches
{
    private static bool _forceRenderTextureSize;
    
    [HarmonyPatch(typeof(RenderTexture), nameof(RenderTexture.GetTemporary), typeof(int), typeof(int), typeof(int), typeof(RenderTextureFormat))]
    [HarmonyPrefix]
    public static void ForceCameraTextureSizePatch([HarmonyArgument(0)] ref int width, [HarmonyArgument(1)] ref int height)
    {
        if (!_forceRenderTextureSize) return;

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
        _forceRenderTextureSize = true;
    }
        
    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Begin))]
    [HarmonyPostfix]
    public static void DisableForceCamsTextureSizePatch()
    {
        _forceRenderTextureSize = false;
    }
    
    private static Resolution _previousScreenResolution;

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPostfix]
    public static void UpdateShadowResolutionPatch(HudManager __instance)
    {
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        if (screenWidth == _previousScreenResolution.width && screenHeight == _previousScreenResolution.height) return;
        var shadowCamera = __instance.PlayerCam.GetComponentInChildren<ShadowCamera>().GetComponent<Camera>();

        if (!shadowCamera) return;
                
        var res = Mathf.Max(Screen.width, Screen.height);
        RenderTexture highResTexture = new(res, res, 0) {antiAliasing = 4};
        
        shadowCamera.targetTexture = highResTexture;
        shadowCamera.allowMSAA = true;
        __instance.ShadowQuad.material.mainTexture = highResTexture;
                
        _previousScreenResolution.width = screenWidth;
        _previousScreenResolution.height = screenHeight;
    }
}