using HarmonyLib;
using UnityEngine;

namespace Laboratory.Patches.FancyStuff;

[HarmonyPatch(typeof(RenderTexture), nameof(RenderTexture.GetTemporary), typeof(int), typeof(int), typeof(int), typeof(RenderTextureFormat))]
public static class RenderTexture_GetTemporary_Patch
{
    /// <summary>
    /// Sets if patch should run or not
    /// </summary>
    public static bool Enabled { get; set; }
    
    /// <summary>
    /// Forces render textures to be the size of the screen regardless of requested size
    /// </summary>
    [HarmonyPrefix]
    public static void Prefix([HarmonyArgument(0)] ref int width, [HarmonyArgument(1)] ref int height)
    {
        if (!Enabled) return;

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
}