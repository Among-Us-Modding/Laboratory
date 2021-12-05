using System.Collections.Generic;
using System.Collections.ObjectModel;
using HarmonyLib;
using Reactor;
using UnityEngine;

namespace Laboratory.Player;

/// <summary>
/// Race condition-free size modifiers
/// </summary>
public static class SizeModifer
{
    public static Vector3 DefaultSize { get; set; } = new Vector3(0.7f, 0.7f, 1f);

    private static readonly Dictionary<PlayerPhysics, Dictionary<object?, float>> sizeModifiers = new(Il2CppEqualityComparer<PlayerPhysics>.Instance);

    public static IReadOnlyDictionary<PlayerPhysics, Dictionary<object?, float>> SizeModifiers { get; } = new ReadOnlyDictionary<PlayerPhysics, Dictionary<object?, float>>(sizeModifiers);

    public static void SetSizeModifier(this PlayerPhysics player, float value, object? key)
    {
        var set = sizeModifiers[player];
        if (value == 1)
        {
            set.Remove(key);
        }
        else
        {
            set[key] = value;
        }

        Update(player);
    }

    public static void SetSizeModifier<T>(this PlayerPhysics player, float value)
    {
        player.SetSizeModifier(value, typeof(T));
    }

    public static void Update(PlayerPhysics player)
    {
        var size = DefaultSize;

        foreach (var (_, v) in sizeModifiers[player])
        {
            size *= v;
        }

        player.transform.localScale = size;
    }

    internal static void Clear(PlayerPhysics player)
    {
        sizeModifiers[player].Clear();
        Update(player);
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    private static class AwakePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance.notRealPlayer) return;
            sizeModifiers[__instance.MyPhysics] = new Dictionary<object?, float>();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    public static class OnDestroyPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            sizeModifiers.Remove(__instance.MyPhysics);
        }
    }
}
