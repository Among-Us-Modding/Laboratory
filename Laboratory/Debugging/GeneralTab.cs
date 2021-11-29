using System;
using System.Collections.Generic;
using Laboratory.Effects.Utils;
using Laboratory.Extensions;
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
        
        if (ProgressSystem.Instance != null)
        {
            GUILayout.Label("Mod Phase: " + ProgressSystem.Instance.Stage);
            if (AmongUsClient.Instance.AmHost)
            {
                var timer = GUILayout.HorizontalSlider(ProgressSystem.Instance.Timer, 0, ProgressSystem.Instance.TotalTime + 0.0001f);
                if (Math.Abs(timer - ProgressSystem.Instance.Timer) > 0.001f)
                {
                    ProgressSystem.Instance.Timer = timer;
                    ProgressSystem.Instance.IsDirty = true;
                }
            }
        }
        
        if (HealthSystem.Instance != null)
        {
            GUILayout.Label("Max Health: " + HealthSystem.MaxHealth);
            if (AmongUsClient.Instance.AmHost)
            {
                var maxHealth = Mathf.RoundToInt(GUILayout.HorizontalSlider(HealthSystem.MaxHealth/10f, 0, 25)) * 10;
                if (maxHealth != HealthSystem.MaxHealth)
                {
                    HealthSystem.MaxHealth = maxHealth;
                }
                
                GUILayoutExtensions.Divider();

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
}
