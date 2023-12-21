using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Jungle.Extensions;
using Jungle.Map;
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
    /// Removes the snow particles on polus
    /// </summary>
    public static bool DisableSnow { get; set; }
    
    /// <summary>
    /// Sets the new Colors for the lava
    /// Leaving as default will keep original
    /// </summary>
    public static Color32[] LavaColors { get; set; } = LavaColorChanger.Default;
    
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
    public static void SetupConfig(ShipStatus __instance)
    {
        if (MapConfig.DisableShadows)
        {
            HudManager.Instance.ShadowQuad.material.color = Color.clear;

            foreach (var collider2D in Object.FindObjectsOfType<Collider2D>())
            {
                if (collider2D.gameObject.layer == 10) collider2D.isTrigger = true;
            }
        }
        
        if (MapConfig.DisableRoomTracker) HudManager.Instance.roomTracker.text.renderer.enabled = false;
        
        foreach (AssetBundle textureSwapBundle in MapConfig.TextureSwapBundles)
        {
            if (!MapConfig.SwapRawTextures)
            {
                List<Sprite> sprites = textureSwapBundle.LoadAllAssets().OfIl2CppType<Sprite>().ToList();
                SpriteSwapper.Swap(__instance.gameObject, sprites);
            }
            else
            {
                SpriteSwapper.SwapMapSpritesRaw(textureSwapBundle);
            }
        }
        
        // Map specific
        if (__instance.Type == ShipStatus.MapType.Pb)
        {
            if (MapConfig.DisableSnow) Object.FindObjectOfType<SnowManager>()?.gameObject.SetActive(false);
        }
        
        // TODO: Texture swapping
    }
}
