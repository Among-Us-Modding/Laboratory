using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Laboratory.Utils;
using MonoMod.Utils;
using Reactor;
using UnityEngine;

namespace Laboratory.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class KeybindAttribute : Attribute
{
    public KeyCode[] Keys { get; } = Array.Empty<KeyCode>();

    public int[] MouseButtons { get; } = Array.Empty<int>();

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

    static KeybindAttribute()
    {
        UnityEvents.UpdateEvent += CheckInputs;
    }

    private static List<Assembly> Processed { get; } = new();
    private static Dictionary<KeyCode, List<Action>> Keybinds { get; } = new();
    private static Dictionary<int, List<Action>> Mousebinds { get; } = new();

    internal static void Initialize()
    {
        ChainloaderHooks.PluginLoad += plugin => Register(plugin.GetType().Assembly);
    }

    private static void Register(Assembly assembly)
    {
        if (Processed.Contains(assembly)) return;
        Processed.Add(assembly);

        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            foreach (var method in type.GetMethods(AccessTools.all))
            {
                var attribute = method.GetCustomAttribute<KeybindAttribute>();
                if (attribute != null)
                {
                    if (!method.IsStatic)
                    {
                        throw new Exception($"Couldn't register {method.FullDescription()}, keybind methods have to be static");
                    }

                    if (method.GetParameters().Length != 0)
                    {
                        throw new Exception($"Couldn't register {method.FullDescription()}, keybind methods can't have parameters");
                    }

                    attribute.Keys.Do(k => AddKey(k, method.CreateDelegate<Action>()));
                    attribute.MouseButtons.Do(m => AddMouse(m, method.CreateDelegate<Action>()));
                }
            }
        }
    }

    private static void AddKey(KeyCode key, Action action)
    {
        if (!Keybinds.ContainsKey(key)) Keybinds[key] = new List<Action>();
        Keybinds[key].Add(action);
    }

    private static void AddMouse(int mouse, Action action)
    {
        if (!Mousebinds.ContainsKey(mouse)) Mousebinds[mouse] = new List<Action>();
        Mousebinds[mouse].Add(action);
    }

    private static void CheckInputs()
    {
        foreach (var key in Keybinds.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                foreach (var action in Keybinds[key])
                {
                    action();
                }
            }
        }

        foreach (var mouseButton in Mousebinds.Keys)
        {
            if (Input.GetMouseButtonDown(mouseButton))
            {
                foreach (var action in Mousebinds[mouseButton])
                {
                    action();
                }
            }
        }
    }
}