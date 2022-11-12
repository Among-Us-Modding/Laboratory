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

    public Dictionary<string, int> OldHealths { get; } = new();
    public bool AllowDeath { get; set; }

    
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
                ProgressSystem.Instance.ShouldRun = GUILayout.Toggle(ProgressSystem.Instance.ShouldRun, $"Progress Bar: {(ProgressSystem.Instance.ShouldRun ? "Running" : "Stopped")}", GUI.skin.button);
                float timer = GUILayout.HorizontalSlider(ProgressSystem.Instance.Timer, 0, ProgressSystem.Instance.TotalTime + 0.0001f);
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
                int maxHealth = Mathf.RoundToInt(GUILayout.HorizontalSlider(HealthSystem.MaxHealth / 10f, 0, 25)) * 10;
                if (maxHealth != HealthSystem.MaxHealth)
                {
                    HealthSystem.MaxHealth = maxHealth;
                }
                
                if (GUILayout.Button("Save Current Health"))
                {
                    foreach (GameData.PlayerInfo? instanceAllPlayer in GameData.Instance.AllPlayers)
                    {
                        OldHealths[instanceAllPlayer.PlayerName] = Math.Clamp(HealthSystem.Instance.GetHealth(instanceAllPlayer.PlayerId), 1, 999999);
                    }
                }

                if (GUILayout.Button("Load Old Health"))
                {
                    foreach (GameData.PlayerInfo? instanceAllPlayer in GameData.Instance.AllPlayers)
                    {
                        if (OldHealths.TryGetValue(instanceAllPlayer.PlayerName, out int health))
                        {
                            HealthSystem.Instance.SetHealth(instanceAllPlayer.PlayerId, health);
                        }
                    }
                }
                
                AllowDeath = GUILayout.Toggle(AllowDeath, "Allow Death When Changing Health");

                List<(byte playerId, int newHealth)> list = new();
                HealthSystem? system = HealthSystem.Instance!;
                foreach ((byte pid, int health) in system.PlayerHealths)
                {
                    GUILayout.Label(GameData.Instance.GetPlayerById(pid).PlayerName);
                    int newHealth = Mathf.RoundToInt(GUILayout.HorizontalSlider(health, 0, HealthSystem.Instance.GetMaxHealth(pid)));
                    if (newHealth != health) list.Add((pid, AllowDeath ? newHealth : Math.Clamp(newHealth, 1, int.MaxValue)));
                }

                foreach ((byte playerId, int newHealth) in list)
                {
                    system.SetHealth(playerId, newHealth);
                }
            }
        }
    }
}
