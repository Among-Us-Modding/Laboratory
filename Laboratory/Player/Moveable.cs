using System.Collections.Generic;
using HarmonyLib;
using Reactor;
using UnityEngine;

namespace Laboratory.Player;

/// <summary>
/// Race condition-free immovable players
/// </summary>
public static class Moveable
{
    private static readonly Dictionary<PlayerControl, HashSet<object>> _immovable = new(Il2CppEqualityComparer<PlayerControl>.Instance);

    public static void SetMoveable(this PlayerControl player, bool canMove, object key)
    {
        HashSet<object>? set = _immovable[player];
        bool changed = canMove ? set.Remove(key) : set.Add(key);

        if (changed && !canMove)
        {
            player.MyPhysics.body.velocity = Vector2.zero;
        }
    }

    public static void SetMoveable<T>(this PlayerControl player, bool canMove)
    {
        player.SetMoveable(canMove, typeof(T));
    }

    public static bool IsMoveable(this PlayerControl player, object key)
    {
        return !_immovable[player].Contains(key);
    }

    public static bool IsMoveable<T>(this PlayerControl player)
    {
        return player.IsMoveable(typeof(T));
    }

    internal static void Clear(PlayerControl player)
    {
        _immovable[player].Clear();
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    private static class AwakePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance.notRealPlayer) return;
            _immovable[__instance] = new HashSet<object>();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
    private static class GetCanMovePatch
    {
        public static void Postfix(PlayerControl __instance, ref bool __result)
        {
            if (_immovable[__instance].Count > 0)
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    public static class OnDestroyPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            _immovable.Remove(__instance);
        }
    }
}
