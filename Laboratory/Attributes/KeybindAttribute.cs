using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Laboratory.Extensions;
using Laboratory.Utils;
using Reactor;
using UnityEngine;

namespace Laboratory.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class KeybindAttribute : Attribute
    {
        /// <summary>
        /// Binds the keys listed to invoke the method on keydown
        /// </summary>
        public KeybindAttribute(params KeyCode[] keys)
        {
            Keys = keys;
        }

        /// <summary>
        /// Binds the mouse buttons listed to invoke the method on mousedown
        /// </summary>
        public KeybindAttribute(params int[] mouseButtons)
        {
            MouseButtons = mouseButtons;
        }

        private KeyCode[] Keys = Array.Empty<KeyCode>();
        private int[] MouseButtons = Array.Empty<int>();

        static KeybindAttribute()
        {
            UnityEvents.UpdateEvent += CheckInputs;
        }
        
        private static List<Assembly> Processed { get; } = new();
        private static Dictionary<KeyCode, List<MethodInfo>> Keybinds = new();
        private static Dictionary<int, List<MethodInfo>> Mousebinds = new();

        internal static void Initialize()
        {
            ChainloaderHooks.PluginLoad += plugin => Register(plugin.GetType().Assembly);
        }
        
        private static void Register(Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            if (Processed.Contains(assembly)) return;
            Processed.Add(assembly);

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                foreach (var method in type.GetMethods(AccessTools.all))
                {
                    var attribute = method.GetCustomAttribute<KeybindAttribute>();
                    if (attribute != null && method.IsStatic)
                    {
                        attribute.Keys.ForEach(k => AddKey(k, method));
                        attribute.MouseButtons.ForEach(m => AddMouse(m, method));
                    }
                }
            }
        }
        
        private static void AddKey(KeyCode key, MethodInfo info)
        {
            if (!Keybinds.ContainsKey(key)) Keybinds[key] = new List<MethodInfo>();
            Keybinds[key].Add(info);
        }
        
        private static void AddMouse(int mouse, MethodInfo info)
        {
            if (!Mousebinds.ContainsKey(mouse)) Mousebinds[mouse] = new List<MethodInfo>();
            Mousebinds[mouse].Add(info);
        }

        private static void CheckInputs()
        {
            foreach (var key in Keybinds.Keys)
            {
                if (Input.GetKeyDown(key))
                {
                    Keybinds[key].ForEach(m => m.Invoke(null, Array.Empty<object>()));
                }
            }
            
            foreach (int mouseButton in Mousebinds.Keys)
            {
                if (Input.GetMouseButtonDown(mouseButton))
                {
                    Mousebinds[mouseButton].ForEach(m => m.Invoke(null, Array.Empty<object>()));
                }
            }
        }
    }
}