using HarmonyLib;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class FancyShadowsPatches
{
    private static Resolution PreviousScreenResolution;

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPostfix]
    public static void UpdateShadowResolutionPatch(HudManager __instance)
    {
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        if (screenWidth != PreviousScreenResolution.width || screenHeight != PreviousScreenResolution.height)
        {
            var shadowCamera = __instance.PlayerCam.GetComponentInChildren<ShadowCamera>().GetComponent<Camera>();

            if (!shadowCamera) return;
                
            var res = Mathf.Max(Screen.width, Screen.height);
            RenderTexture highResTexture = new(res, res, 0) {antiAliasing = 4};
        
            shadowCamera.targetTexture = highResTexture;
            shadowCamera.allowMSAA = true;
            __instance.ShadowQuad.material.mainTexture = highResTexture;
                
            PreviousScreenResolution.width = screenWidth;
            PreviousScreenResolution.height = screenHeight;
        }
    }
}