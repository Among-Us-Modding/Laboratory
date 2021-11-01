using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Reactor.Extensions;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Laboratory.Utils
{
    /// <summary>
    /// Utilities for managing assets
    /// </summary>
    public sealed class AssetManager
    {
        public static readonly Dictionary<string, Sprite> CachedEmbeddedSprites = new();
        public static readonly Dictionary<string, AssetBundle> CachedEmbeddedBundles = new();

        /// <summary>
        /// Create a Sprite from a named embedded resource
        /// </summary>
        /// <param name="spriteName">Name of the embedded resource</param>
        /// <param name="ppu">pixels per unit of the sprite</param>
        /// <param name="assembly">Assembly containing the sprite - defaults to searching all loaded assemblies</param>
        public static Sprite? LoadSprite(string spriteName, float ppu = 100f, Assembly? assembly = null)
        {
            var assemblies = assembly == null ? 
                AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToArray() : 
                new[] {assembly};
            
            for (int i = 0; i < assemblies.Length; i++)
            {
                var asm = assemblies[i];
                string? match = asm?.GetManifestResourceNames().FirstOrDefault(n => n.Contains(spriteName));
                if (match == null) continue;
                
                if (CachedEmbeddedSprites.ContainsKey(match)) return CachedEmbeddedSprites[match];
                byte[]? buffer = ReadAll(asm?.GetManifestResourceStream(match));
                if (buffer == null) return null;

                Texture2D tex = new(2, 2, TextureFormat.ARGB32, false);
                ImageConversion.LoadImage(tex, buffer, false);

                return CachedEmbeddedSprites[match] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu).DontDestroy();
            }
            return null;
        }

        /// <summary>
        /// Load an AssetBundle from an assemblies embedded resources
        /// </summary>
        /// <param name="bundleName">Name of the embedded resource</param>
        /// <param name="assembly">Assembly containing the bundle - defaults to searching all loaded assemblies</param>
        /// <returns></returns>
        public static AssetBundle? LoadBundle(string? bundleName, Assembly? assembly = null)
        {
            if (bundleName == null) return null;

            var assemblies = assembly == null ? 
                AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToArray() : 
                new[] {assembly};
            
            for (var i = 0; i < assemblies.Length; i++)
            {
                var asm = assemblies[i];
                string? match = asm?.GetManifestResourceNames().FirstOrDefault(n => n.Contains(bundleName));
                if (match == null) continue;
                
                if (CachedEmbeddedBundles.ContainsKey(match)) return CachedEmbeddedBundles[match];
                byte[]? buffer = ReadAll(asm?.GetManifestResourceStream(match));
                if (buffer == null) return null;

                return CachedEmbeddedBundles[match] = AssetBundle.LoadFromMemory(buffer).DontUnload();
            }
            return null;
        }

        private static byte[]? ReadAll(Stream? stream)
        {
            using MemoryStream ms = new();
            
            stream?.CopyTo(ms);
            return stream != null ? ms.ToArray() : null;
        }

        /// <summary>
        /// Creates AssetManager by getting bundle from embedded resources
        /// </summary>
        /// <param name="bundleName">Name of the embedded resource being used</param>
        public AssetManager(string bundleName)
        {
            m_Name = bundleName;
            LoadAllAssets();
        }

        /// <summary>
        /// Creates AssetManager from an already existing AssetBundle
        /// </summary>
        /// <param name="bundle">Bundle to create manager with</param>
        public AssetManager(AssetBundle bundle)
        {
            m_Bundle = bundle;
            LoadAllAssets();
        }

        private Dictionary<string, Object> ObjectCache { get; } = new();
        private AssetBundle? m_Bundle;
        private string? m_Name;
        
        /// <summary>
        /// AssetManager's primary AssetBundle
        /// </summary>
        public AssetBundle? Bundle => m_Bundle ??= LoadBundle(m_Name);
        
        /// <summary>
        /// Load asset of a given name from the manager's bundle
        /// </summary>
        public T? LoadAsset<T>(string name) where T : Object
        {
            if (ObjectCache.TryGetValue(name, out Object result)) return result.TryCast<T>();
            
            T asset = Bundle.LoadAsset<T>(name);
            if (!asset) return null;
            
            asset.DontUnload();
            ObjectCache[name] = asset;
            
            return asset;
        }

        /// <summary>
        /// Load all assets stored in the manager's bundle
        /// </summary>
        /// <returns></returns>
        public Object[]? LoadAllAssets()
        {
            if (Bundle == null) return null;
            Il2CppReferenceArray<Object> assets = Bundle.LoadAllAssets();
            foreach (Object asset in assets) asset.DontUnload();
            return assets;
        }
    }
}