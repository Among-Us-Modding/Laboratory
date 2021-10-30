using System.Collections.Generic;
using UnityEngine;

namespace Laboratory.Mods.Player
{
    public static class Moveable
    {
        public static Dictionary<int, List<string>> CanMoveables = new();

        public static bool AnyMovables(PlayerControl player) => CanMoveables[player.GetHashCode()].Count > 0;

        public static void AddMoveable(PlayerControl player, string name)
        {
            CanMoveables.TryGetValue(player.GetHashCode(), out List<string> strs);
            if (strs != null && strs.Contains(name)) return;
            strs?.Add(name);
            player.MyPhysics.body.velocity = Vector2.zero;
        }

        public static void RemoveMoveable(PlayerControl player, string name)
        {
            CanMoveables[player.GetHashCode()].RemoveAll(s => s == name);
        }
    }
}