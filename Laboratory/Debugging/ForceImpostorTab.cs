using System.Collections.Generic;
using System.Linq;
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

        IEnumerable<string> allPlayerNames = GameData.Instance.AllPlayers.ToArray().Where(d => d != null).Select(d => d.PlayerName);
        foreach (string playerName in allPlayerNames)
        {
            bool currentlyForced = CurrentlySelected.Contains(playerName);
            bool toggled = GUILayout.Toggle(currentlyForced, $"{playerName}{(currentlyForced ? " Forced" : "")}", GUI.skin.button);
            if (toggled && !currentlyForced) CurrentlySelected.Add(playerName);
            if (!toggled && currentlyForced) CurrentlySelected.Remove(playerName);
        }
    }
}
