using HarmonyLib;
using UnityEngine;

namespace Laboratory.Patches.FancyStuff
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManager_Update_Patch
    {
        /// <summary>
        /// The last resolution set by the shadow camera patch
        /// </summary>
        public static Resolution PreviousResolution;

        /// <summary>
        /// Sets shadow camera render texture to be far higher quality
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        public static void Postfix(HudManager __instance)
        {
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            if (screenWidth != PreviousResolution.width || screenHeight != PreviousResolution.height)
            {
                var shadowCamera = __instance.PlayerCam.GetComponentInChildren<ShadowCamera>().GetComponent<Camera>();

                if (!shadowCamera) return;
                
                var res = Mathf.Max(Screen.width, Screen.height);
                RenderTexture highResTexture = new(res, res, 0) {antiAliasing = 4};
        
                shadowCamera.targetTexture = highResTexture;
                shadowCamera.allowMSAA = true;
                __instance.ShadowQuad.material.mainTexture = highResTexture;
                
                PreviousResolution.width = screenWidth;
                PreviousResolution.height = screenHeight;
            }
        }
    }
}