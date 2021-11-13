using System;
using System.Collections.Generic;
using Laboratory.Enums;
using Laboratory.Systems;
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
    /// Custom Systems that will be added to the map
    /// Type must be able to be cast into ISystemType
    /// </summary>
    public static Dictionary<SystemTypes, Type> CustomSystems { get; set; } = new()
    {
        { CustomSystemTypes.HealthSystem, typeof(HealthSystem) }
    };

    /// <summary>
    /// Removes the snow particles on polus
    /// </summary>
    public static bool DisableSnow { get; set; }

    /// <summary>
    /// Sets the new Colors for the lava
    /// Leaving as default will keep original
    /// </summary>
    public static Color32[]? LavaColors { get; set; }

    /// <summary>
    /// Blue / Water color scheme for lava colors
    /// </summary>
    public static Color32[] LavaColors_Blue { get; } =
    {
        new Color32(17, 111, 137, 255),
        new Color32(33, 143, 173, 255),
        new Color32(24, 79, 94, 255),
        new Color32(105, 198, 227, 255),
        new Color32(17, 111, 137, 255),
    };

    /// <summary>
    /// AssetBundles containing the Sprites used to swap textures on the map
    /// </summary>
    public static List<AssetBundle> TextureSwapBundles { get; set; } = new();
}
