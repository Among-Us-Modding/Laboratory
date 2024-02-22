using System.Collections.Generic;
using System.Collections.ObjectModel;
using HarmonyLib;
using Reactor.Utilities;

namespace Laboratory.Player;

/// <summary>
/// Race condition-free speed modifiers
/// </summary>
public static class SpeedModifier
{
    public static float DefaultSpeed { get; set; } = 2.5f;

    private static readonly Dictionary<PlayerPhysics, Dictionary<object, float>> _speedModifiers = new(Il2CppEqualityComparer<PlayerPhysics>.Instance);

    public static IReadOnlyDictionary<PlayerPhysics, Dictionary<object, float>> SpeedModifiers { get; } = new ReadOnlyDictionary<PlayerPhysics, Dictionary<object, float>>(_speedModifiers);

    public static void SetSpeedModifier(this PlayerPhysics player, float value, object key)
    {
        if (!_speedModifiers.TryGetValue(player, out var set))
        {
            _speedModifiers[player] = set = new  Dictionary<object, float>();
        }
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

    public static void SetSpeedModifier<T>(this PlayerPhysics player, float value)
    {
        player.SetSpeedModifier(value, typeof(T));
    }

    public static void Update(PlayerPhysics player)
    {
        float speed = DefaultSpeed;

        foreach ((object _, float v) in _speedModifiers[player])
        {
            speed *= v;
        }

        player.Speed = speed;
    }

    internal static void Clear(PlayerPhysics player)
    {
        _speedModifiers[player].Clear();
        Update(player);
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    private static class AwakePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance.notRealPlayer) return;
            _speedModifiers[__instance.MyPhysics] = new Dictionary<object, float>();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    public static class OnDestroyPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            _speedModifiers.Remove(__instance.MyPhysics);
        }
    }
}
