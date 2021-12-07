using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Laboratory.Utils;
using Reactor;
using UnityEngine;

namespace Laboratory.Extensions;

public static class FindExtensions
{
    static FindExtensions()
    {
        UnityEvents.OnEnableEvent += Clear;
        UnityEvents.FixedUpdateEvent += Clear;
    }
    
    public static Predicate<PlayerControl> Alive { get; } = player => !player.Data.IsDead;
    public static Predicate<PlayerControl> Visible { get; } = player => player.Visible;
    public static Predicate<PlayerControl> Crewmate { get; } = player => !player.Data.IsImpostor;
    public static Predicate<PlayerControl> Impostor { get; } = player => player.Data.IsImpostor;

    internal static Dictionary<Predicate<PlayerControl>, IEnumerable<PlayerControl>> _predicateStore { get; } = new();
    internal static Dictionary<PlayerControl, Vector2> _playerPositions { get; } = new(Il2CppEqualityComparer<PlayerControl>.Instance);

    internal static IEnumerable<PlayerControl> GetPlayers(Predicate<PlayerControl>[] filters)
    {
        PlayerControl[] allPlayers = PlayerControl.AllPlayerControls.ToArray();
        IEnumerable<PlayerControl> validPlayers = allPlayers;

        foreach (Predicate<PlayerControl> predicate in filters)
        {
            if (_predicateStore.TryGetValue(predicate, out var set))
            {
                // ReSharper disable once PossibleMultipleEnumeration
                validPlayers = validPlayers.Intersect(set);
            }
            else
            {
                _predicateStore[predicate] = allPlayers.Where(player => predicate(player));
            }
        }

        return validPlayers;
    }

    internal static Vector2 GetPlayerPosition(PlayerControl player)
    {
        if (_playerPositions.TryGetValue(player, out var pos)) return pos;
        return _playerPositions[player] = player.GetTruePosition();
    }
    
    public static PlayerControl? FindPlayer(Vector2 position, float maxDistance, params Predicate<PlayerControl>[] filters)
    {
        PlayerControl? current = null;

        var validPlayers = GetPlayers(filters);
        foreach (var player in validPlayers)
        {
            var delta = Vector2.Distance(position, GetPlayerPosition(player));
            if (delta < maxDistance)
            {
                maxDistance = delta;
                current = player;
            }
        }

        return current;
    }

    public static PlayerControl? FindCrewmate(Vector2 position, float maxDistance, params Predicate<PlayerControl>[] filters)
    {
        return FindPlayer(position, maxDistance, Alive, Visible, Crewmate, player => filters.All(filter => filter(player)));
    }

    public static PlayerControl? FindImpostor(Vector2 position, float maxDistance, params Predicate<PlayerControl>[] filters)
    {
        return FindPlayer(position, maxDistance, Alive, Visible, Impostor, player => filters.All(filter => filter(player)));
    }

    internal static void Clear()
    {
        _playerPositions.Clear();
        _predicateStore.Clear();
    }
}
