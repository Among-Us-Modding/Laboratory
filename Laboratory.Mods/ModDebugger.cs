using System.Collections.Generic;
using Laboratory.Debugging;
using Laboratory.Mods.Effects.Utils;
using Laboratory.Mods.Enums;
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
        }
        
            
        private void BuildUI()
        {
            if (CameraZoomController.Instance != null)
            {
                CustomGUILayout.Label($"Camera Zoom: {CameraZoomController.Instance.OrthographicSize}");
                CameraZoomController.Instance.OrthographicSize = GUILayout.HorizontalSlider(CameraZoomController.Instance.OrthographicSize, 1f, 24f, DebugWindow.EmptyOptions);
            }

            CustomGUILayout.Button("Load Unity Explorer", () =>
            {
                RuntimePluginLoader.DownloadPlugin("UnityExplorer");
            });
            
            if (AmongUsClient.Instance.AmHost && ShipStatus.Instance)
            {
                List<(byte playerId, int newHealth)> list = new();
                HealthSystem system = HealthSystem.Instance!;
                foreach ((byte pid, int health) in system.PlayerHealths)
                {
                    GUILayout.Label(GameData.Instance.GetPlayerById(pid).PlayerName, DebugWindow.EmptyOptions);
                    int newHealth = Mathf.RoundToInt(GUILayout.HorizontalSlider(health, 0, HealthSystem.MaxHealth, DebugWindow.EmptyOptions));
                    if (newHealth != health) list.Add((pid, newHealth));
                }

                foreach ((byte playerId, int newHealth) in list)
                {
                    system.SetHealth(playerId, newHealth);
                }
            }
        }
    }
}