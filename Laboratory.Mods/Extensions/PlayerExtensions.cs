using System;
using System.Collections.Generic;
using Laboratory.Mods.Player.MonoBehaviours;
using Laboratory.Utils;
using UnityEngine;

namespace Laboratory.Mods.Extensions
{
    public static class PlayerExtensions
    {
        private static Dictionary<Component, Dictionary<Type, PlayerComponent>> Components = new(Il2CppEqualityComparer<Component>.Instance);
        public static T GetPlayerComponent<T>(this Component component) where T : PlayerComponent
        {
            if (!Components.TryGetValue(component, out var comps))
            {
                comps = Components[component] = new Dictionary<Type, PlayerComponent>();
            }

            Type t = typeof(T);
            if (comps.TryGetValue(t, out PlayerComponent playerComponent))
            {
                return playerComponent.TryCast<T>();
            }

            T comp = component.GetComponent<T>();
            if (!comp) return null;
            
            comps[t] = comp;
            return comp;
        }
    }
}