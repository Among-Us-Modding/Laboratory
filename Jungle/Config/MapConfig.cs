using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Jungle.Config;

/// <summary>
/// Configuration options for the appearance of the map
/// </summary>
public static class MapConfig
{
    /// <summary>
    /// Disable shadows on the map
    /// </summary>
    public static bool DisableShadows { get; set; }

    /// <summary>
    /// Hide the room tracker on the HUD
    /// </summary>
    public static bool DisableRoomTracker { get; set; }
    
    /// <summary>
    /// AssetBundles containing the Sprites used to swap textures on the map
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    public static List<AssetBundle> TextureSwapBundles { get; set; } = new();
    
    /// <summary>
    /// Should the texture swapping use sprites from the bundle, or replace the texture currently being used on the SpriteRender
    /// I cba to explain more here so dm me if u have problems
    /// </summary>
    public static bool SwapRawTextures { get; set; }
}

[HarmonyPatch]
public static class MapConfigPatches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    [HarmonyPostfix]
    public static void SetupConfig()
    {
        if (MapConfig.DisableShadows)
        {
            HudManager.Instance.ShadowQuad.material.color = Color.clear;

            foreach (Collider2D collider2D in Object.FindObjectsOfType<Collider2D>())
            {
                if (collider2D.gameObject.layer == 10) collider2D.isTrigger = true;
            }
        }
        
        if (MapConfig.DisableRoomTracker) HudManager.Instance.roomTracker.text.renderer.enabled = false;
        
        // TODO: Texture swapping
    }
}
