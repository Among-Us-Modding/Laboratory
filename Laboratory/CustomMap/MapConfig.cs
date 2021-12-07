using System.Collections.Generic;
using UnityEngine;

namespace Laboratory.CustomMap;

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
    public static List<AssetBundle> TextureSwapBundles { get; set; } = new();
    
    /// <summary>
    /// Should the texture swapping use sprites from the bundle, or replace the texture currently being used on the SpriteRender
    /// I cba to explain more here so dm me if u have problems
    /// </summary>
    public static bool SwapRawTextures { get; set; }
}
