using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Laboratory.Debugging;
using Laboratory.Extensions;
using Laboratory.Mods.Effects.Utils;
using Laboratory.Mods.Systems;
using Laboratory.Utils;
using UnityEngine;

namespace Laboratory.Mods
{
    public class ModDebugger : Debugger
    {
        public override IEnumerable<DebugTab> DebugTabs()
        {
            yield return new DebugTab("Mods", BuildUI);
            yield return new DebugTab("Force Impostor", BuildForceImpostorTab);
        }

        public static bool EnableForceImpostor { get; set; }
        public static List<string> CurrentlySelected { get; } = new();

        public static void BuildForceImpostorTab()
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                GUILayout.Label("You are not the host");
                return;
            }

            EnableForceImpostor = GUILayout.Toggle(EnableForceImpostor, $"Force Impostor: {(EnableForceImpostor ? "Enabled" : "Disabled")}", GUI.skin.button);
            if (!EnableForceImpostor) return;
            CustomGUILayout.Divider();

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
        public static class ShipStatus_SelectInfected_Patch
        {
            [HarmonyArgument(Priority.Last)]
            public static bool Prefix()
            {
                if (!EnableForceImpostor) return true;

                var impostors = new List<GameData.PlayerInfo>();

                var allPlayers = GameData.Instance.AllPlayers.ToSystemList();
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

        private void BuildUI()
        {
            if (CameraZoomController.Instance != null)
            {
                CustomGUILayout.Label($"Camera Zoom: {CameraZoomController.Instance.OrthographicSize}");
                CameraZoomController.Instance.OrthographicSize = GUILayout.HorizontalSlider(CameraZoomController.Instance.OrthographicSize, 1f, 24f);
            }

            CustomGUILayout.Button("Load Unity Explorer", () =>
            {
                RuntimePluginLoader.DownloadPlugin("UnityExplorer");
            });

            if (AmongUsClient.Instance.AmHost && ShipStatus.Instance)
            {
                List<(byte playerId, int newHealth)> list = new();
                var system = HealthSystem.Instance!;
                foreach ((var pid, var health) in system.PlayerHealths)
                {
                    GUILayout.Label(GameData.Instance.GetPlayerById(pid).PlayerName);
                    var newHealth = Mathf.RoundToInt(GUILayout.HorizontalSlider(health, 0, HealthSystem.MaxHealth));
                    if (newHealth != health) list.Add((pid, newHealth));
                }

                foreach ((var playerId, var newHealth) in list)
                {
                    system.SetHealth(playerId, newHealth);
                }
            }
        }
    }
}
