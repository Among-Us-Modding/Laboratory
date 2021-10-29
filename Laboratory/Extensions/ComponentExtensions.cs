using UnityEngine;

namespace Laboratory.Extensions
{
    public static class ComponentExtensions
    {
        public static T EnsureComponent<T>(this GameObject obj) where T : Component
        {
            var comp = obj.GetComponent<T>();
            if (!comp) comp = obj.AddComponent<T>();
            return comp;
        }

        public static T EnsureComponent<T>(this Component obj) where T : Component => EnsureComponent<T>(obj.gameObject);
    }
}