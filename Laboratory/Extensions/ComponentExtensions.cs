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
}