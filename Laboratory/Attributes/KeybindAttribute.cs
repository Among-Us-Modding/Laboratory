using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Laboratory.Utils;
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
        IL2CPPChainloader.Instance.PluginLoad += (_, assembly, _) => Register(assembly);
    }

    private static void Register(Assembly assembly)
    {
        if (Processed.Contains(assembly)) return;
        Processed.Add(assembly);

        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
            foreach (MethodInfo method in type.GetMethods(AccessTools.all))
            {
                KeybindAttribute attribute = method.GetCustomAttribute<KeybindAttribute>();
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
        foreach (KeyCode key in Keybinds.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                foreach (Action action in Keybinds[key])
                {
                    action();
                }
            }
        }

        foreach (int mouseButton in Mousebinds.Keys)
        {
            if (Input.GetMouseButtonDown(mouseButton))
            {
                foreach (Action action in Mousebinds[mouseButton])
                {
                    action();
                }
            }
        }
    }
}