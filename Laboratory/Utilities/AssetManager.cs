using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Laboratory.Utilities;

/// <summary>
/// Utilities for managing assets
/// </summary>
public sealed class AssetManager
{
    internal static readonly List<AssetManager> _managers = new();

    private static readonly Dictionary<string, Sprite> _cachedEmbeddedSprites = new();
    private static readonly Dictionary<string, AssetBundle> _cachedEmbeddedBundles = new();

    /// <summary>
    /// Create a Sprite from a named embedded resource
    /// </summary>
    /// <param name="spriteName">Name of the embedded resource</param>
    /// <param name="ppu">pixels per unit of the sprite</param>
    /// <param name="assembly">Assembly containing the sprite - defaults to searching all loaded assemblies</param>
    public static Sprite LoadSprite(string spriteName, float ppu = 100f, Assembly assembly = null)
    {
        Assembly[] assemblies = assembly == null ? AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToArray() : new[] { assembly };

        foreach (Assembly ass in assemblies)
        {
            string match = ass?.GetManifestResourceNames().FirstOrDefault(n => n.Contains(spriteName));
            if (match == null) continue;

            if (_cachedEmbeddedSprites.ContainsKey(match)) return _cachedEmbeddedSprites[match];
            var buffer = ass.GetManifestResourceStream(match)?.ReadFully();
            if (buffer == null) return null;

            Texture2D tex = new(2, 2, TextureFormat.ARGB32, false);
            ImageConversion.LoadImage(tex, buffer, false);

            return _cachedEmbeddedSprites[match] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu).DontDestroy();
        }

        return null;
    }

    /// <summary>
    /// Load an AssetBundle from an assemblies embedded resources
    /// </summary>
    /// <param name="bundleName">Name of the embedded resource</param>
    /// <param name="assembly">Assembly containing the bundle - defaults to searching all loaded assemblies</param>
    /// <returns></returns>
    public static AssetBundle LoadBundle(string bundleName, Assembly assembly = null)
    {
        if (bundleName == null) return null;

        Assembly[] assemblies = assembly == null ? AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToArray() : new[] { assembly };

        foreach (Assembly asm in assemblies)
        {
            string match = asm?.GetManifestResourceNames().FirstOrDefault(n => n.Contains(bundleName));
            if (match == null) continue;

            if (_cachedEmbeddedBundles.ContainsKey(match)) return _cachedEmbeddedBundles[match];
            var buffer = asm.GetManifestResourceStream(match)?.ReadFully();
            if (buffer == null) return null;

            return _cachedEmbeddedBundles[match] = AssetBundle.LoadFromMemory(buffer).DontUnload();
        }

        return null;
    }

    /// <summary>
    /// Creates AssetManager by getting bundle from embedded resources
    /// </summary>
    /// <param name="bundleName">Name of the embedded resource being used</param>
    public AssetManager(string bundleName)
    {
        _name = bundleName;
        _managers.Add(this);
    }

    /// <summary>
    /// Creates AssetManager from an already existing AssetBundle
    /// </summary>
    /// <param name="bundle">Bundle to create manager with</param>
    public AssetManager(AssetBundle bundle)
    {
        _bundle = bundle;
        _bundle.DontUnload();
        _managers.Add(this);
    }

    private Dictionary<string, Object> ObjectCache { get; } = new();
    private AssetBundle _bundle;
    private readonly string _name;

    /// <summary>
    /// AssetManager's primary AssetBundle
    /// </summary>
    public AssetBundle Bundle => _bundle ??= LoadBundle(_name);

    /// <summary>
    /// Load asset of a given name from the manager's bundle
    /// </summary>
    public T LoadAsset<T>(string name) where T : Object
    {
        if (ObjectCache.TryGetValue(name, out Object result)) return result.TryCast<T>();
        if (Bundle == null) throw new NullReferenceException();
        var asset = Bundle.LoadAsset<T>(name);
        if (!asset)
        {
            Message($"Null Asset: {name}");
            return null;
        }

        if (asset == null) return asset;
        asset.DontUnload();
        ObjectCache[name] = asset;

        return asset;
    }

    /// <summary>
    /// Load all assets stored in the manager's bundle
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets()
    {
        if (Bundle == null) return null;
        Il2CppReferenceArray<Object> assets = Bundle.LoadAllAssets();
        foreach (Object asset in assets) asset.DontUnload();
        return assets;
    }
}

[HarmonyPatch]
internal static class AssetManagerPatches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
    [HarmonyPostfix]
    public static void AmongUsClientAwakePatch()
    {
        foreach (AssetManager manager in AssetManager._managers)
        {
            manager.Bundle!.LoadAllAssetsAsync(Il2CppType.Of<Object>());
        }
    }
}