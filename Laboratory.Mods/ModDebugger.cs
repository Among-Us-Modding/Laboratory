using System;
using System.Collections.Generic;
using Laboratory.Debugging;
using Laboratory.Mods.Buttons;
using Laboratory.Mods.Effects.Interfaces;
using Laboratory.Mods.Effects.MonoBehaviours;
using Laboratory.Mods.Effects.Utils;
using Laboratory.Mods.Player.MonoBehaviours;
using Reactor;
using UnityEngine;

namespace Laboratory.Mods
{
    public class TestEffect : IEffect
    {
        public PlayerManager Owner { get; set; }
        public float Timer { get; set; } = 10f;

        public void Update()
        {
            Timer -= Time.deltaTime;
            CameraZoomController.Instance.OrthographicSize = Mathf.Lerp(10, 3, Timer / 10f);
        }
    }

    public class ModDebugger : Debugger
    {
        public override IEnumerable<DebugTab> DebugTabs()
        {
            yield return new DebugTab("Mods", BuildUI);
        }

        private void BuildUI()
        {
            if (CameraZoomController.Instance)
            {
                CustomGUILayout.Label($"Camera Zoom: {CameraZoomController.Instance.OrthographicSize}");
                CameraZoomController.Instance.OrthographicSize = GUILayout.HorizontalSlider(CameraZoomController.Instance.OrthographicSize, 1f, 24f, DebugWindow.EmptyOptions);
            }

            CustomGUILayout.Button("Test Button", () =>
            {
                var button = CooldownButton.Create<TargetButton>();
                button.SetSprite(HudManager.Instance.KillButton.renderer.sprite);
                button.CurrentTime = button.Cooldown = 5f;
                button.OnClickAction += () => Logger<ModPlugin>.Message("Clicked");
            });
        }
    }
}