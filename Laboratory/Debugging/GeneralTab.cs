using System.Collections.Generic;
using Laboratory.Effects.Utils;
using Laboratory.Systems;
using Laboratory.Utils;
using UnityEngine;

namespace Laboratory.Debugging;

public class GeneralTab : BaseDebugTab
{
    public override string Name => "General";

    public override void BuildUI()
    {
        if (CameraZoomController.Instance != null)
        {
            GUILayout.Label($"Camera Zoom: {CameraZoomController.Instance.OrthographicSize}");
            CameraZoomController.Instance.OrthographicSize = GUILayout.HorizontalSlider(CameraZoomController.Instance.OrthographicSize, 1f, 24f);
        }

        if (GUILayout.Button("Load Unity Explorer"))
        {
            RuntimePluginLoader.DownloadPlugin("UnityExplorer");
        }

        if (AmongUsClient.Instance.AmHost && ShipStatus.Instance)
        {
            List<(byte playerId, int newHealth)> list = new();
            var system = HealthSystem.Instance!;
            foreach (var (pid, health) in system.PlayerHealths)
            {
                GUILayout.Label(GameData.Instance.GetPlayerById(pid).PlayerName);
                var newHealth = Mathf.RoundToInt(GUILayout.HorizontalSlider(health, 0, HealthSystem.MaxHealth));
                if (newHealth != health) list.Add((pid, newHealth));
            }

            foreach (var (playerId, newHealth) in list)
            {
                system.SetHealth(playerId, newHealth);
            }
        }
    }
}
