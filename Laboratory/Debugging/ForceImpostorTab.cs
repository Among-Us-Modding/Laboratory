using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Laboratory.Extensions;
using Laboratory.Utilities;
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
        GUILayoutUtils.Divider();

        IEnumerable<string>? allPlayerNames = GameData.Instance.AllPlayers.ToArray().Where(d => d != null).Select(d => d.PlayerName);
        foreach (string? playerName in allPlayerNames)
        {
            bool currentlyForced = CurrentlySelected.Contains(playerName);
            bool toggled = GUILayout.Toggle(currentlyForced, $"{playerName}{(currentlyForced ? " Forced" : "")}", GUI.skin.button);
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

            List<GameData.PlayerInfo>? impostors = new List<GameData.PlayerInfo>();

            Il2CppArrayBase<GameData.PlayerInfo>? allPlayers = GameData.Instance.AllPlayers.ToArray();
            foreach (string? name in CurrentlySelected)
            {
                GameData.PlayerInfo? match = allPlayers.FirstOrDefault(p => p.PlayerName == name);
                if (match == null || match.Disconnected) continue;
                impostors.Add(match);
            }

            PlayerControl.LocalPlayer.RpcSetInfected(impostors.ToArray());
            return false;
        }
    }
}
