using System.Collections.Generic;
using Laboratory.Utils;
using Reactor;
using UnityEngine;

namespace Laboratory.Extensions;

public static class ComponentExtensions
{
    /// <summary>
    /// Gets or adds a component to a GameObject
    /// </summary>
    public static T EnsureComponent<T>(this GameObject obj) where T : Component
    {
        var comp = obj.GetComponent<T>();
        if (!comp) comp = obj.AddComponent<T>();
        return comp;
    }

    /// <summary>
    /// Gets or adds a component to a the GameObject of another component
    /// </summary>
    public static T EnsureComponent<T>(this Component obj) where T : Component => EnsureComponent<T>(obj.gameObject);

    public static T GetCachedComponent<T>(this GameObject gameObject) where T : Component
    {
        if (CachedComponentStore<T>.Map.TryGetValue(gameObject, out var result))
        {
            return result;
        }

        var component = gameObject.GetComponent<T>();

        var map = CachedComponentStore<T>.Map;
        map[gameObject] = component;

        gameObject.EnsureComponent<UnityEventListener>().OnDestroyEvent += () => map.Remove(gameObject);

        return component;
    }

    public static T GetCachedComponent<T>(this Component component) where T : Component => GetCachedComponent<T>(component.gameObject);

    private static class CachedComponentStore<T> where T : Component
    {
        public static Dictionary<GameObject, T> Map { get; } = new(Il2CppEqualityComparer<GameObject>.Instance);
    }
}
