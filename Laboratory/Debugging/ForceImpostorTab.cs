using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Laboratory.Extensions;
using UnityEngine;

namespace Laboratory.Debugging;

public class ForceImpostorTab : BaseDebugTab
{
    public override string Name => "Force impostor";

    public static bool Enabled { get; set; }
    public static List<string> CurrentlySelected { get; } = new();

    public override void BuildUI()
    {
        if (!AmongUsClient.Instance.AmHost)
        {
            GUILayout.Label("You are not the host");
            return;
        }

        Enabled = GUILayout.Toggle(Enabled, $"Force Impostor: {(Enabled ? "Enabled" : "Disabled")}", GUI.skin.button);
        if (!Enabled) return;
        GUILayoutExtensions.Divider();

        var allPlayerNames = GameData.Instance.AllPlayers.ToArray().Where(d => d != null).Select(d => d.PlayerName);
        foreach (var playerName in allPlayerNames)
        {
            var currentlyForced = CurrentlySelected.Contains(playerName);
            var toggled = GUILayout.Toggle(currentlyForced, $"{playerName}{(currentlyForced ? " Forced" : "")}", GUI.skin.button);
            if (toggled && !currentlyForced) CurrentlySelected.Add(playerName);
            if (!toggled && currentlyForced) CurrentlySelected.Remove(playerName);
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.SelectInfected))]
    private static class SelectInfectedPatch
    {
        [HarmonyArgument(Priority.Last)]
        public static bool Prefix()
        {
            if (!Enabled) return true;

            var impostors = new List<GameData.PlayerInfo>();

            var allPlayers = GameData.Instance.AllPlayers.ToArray();
            foreach (var name in CurrentlySelected)
            {
                var match = allPlayers.FirstOrDefault(p => p.PlayerName == name);
                if (match == null || match.Disconnected) continue;
                impostors.Add(match);
            }

            PlayerControl.LocalPlayer.RpcSetInfected(impostors.ToArray());
            return false;
        }
    }
}
