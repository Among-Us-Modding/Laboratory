using System;
using System.Linq;
using UnityEngine;

namespace Laboratory.Extensions;

public static class FindExtensions
{
    public static Predicate<PlayerControl> Alive { get; } = player => !player.Data.IsDead;
    public static Predicate<PlayerControl> Visible { get; } = player => player.Visible;
    public static Predicate<PlayerControl> Crewmate { get; } = player => !player.Data.IsImpostor;
    public static Predicate<PlayerControl> Impostor { get; } = player => player.Data.IsImpostor;

    public static PlayerControl? FindPlayer(Vector3 position, float maxDistance, params Predicate<PlayerControl>[] filters)
    {
        PlayerControl? current = null;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (filters.Any(filter => !filter(player))) continue;

            var delta = Vector2.Distance(position, player.transform.position);
            if (delta < maxDistance)
            {
                maxDistance = delta;
                current = player;
            }
        }

        return current;
    }

    public static PlayerControl? FindCrewmate(Vector3 position, float maxDistance, params Predicate<PlayerControl>[] filters)
    {
        return FindPlayer(position, maxDistance, Alive, Visible, Crewmate, player => filters.All(filter => filter(player)));
    }

    public static PlayerControl? FindImpostor(Vector3 position, float maxDistance, params Predicate<PlayerControl>[] filters)
    {
        return FindPlayer(position, maxDistance, Alive, Visible, Impostor, player => filters.All(filter => filter(player)));
    }
}
