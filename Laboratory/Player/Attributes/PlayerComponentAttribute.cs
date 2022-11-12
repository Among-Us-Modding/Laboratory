using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime;

namespace Laboratory.Player.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PlayerComponentAttribute : Attribute
{
    private static readonly HashSet<Type> _types = new();

    internal static void Initialize()
    {
        IL2CPPChainloader.Instance.PluginLoad += (_, assembly, _) => Register(assembly);
    }

    private static void Register(Assembly assembly)
    {
        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
            if (type.GetCustomAttribute<PlayerComponentAttribute>() is not null)
            {
                _types.Add(type);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    public static class PlayerControlPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance.notRealPlayer)
            {
                return;
            }

            foreach (Type type in _types)
            {
                Il2CppSystem.Type il2cppType = Il2CppType.From(type);
                if (!__instance.GetComponent(il2cppType)) __instance.gameObject.AddComponent(il2cppType);
            }
        }
    }
}
