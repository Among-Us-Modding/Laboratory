using System;
using System.Collections.Generic;
using Jungle.Buttons;
using Jungle.HUDMap;
using Jungle.Utils;
using UnityEngine;

namespace Jungle.Debugging;

public class GeneralTab : BaseDebugTab
{
    public override string Name => "General";

    public override void BuildUI()
    {
        if (GUILayout.Button("Widescreen Fullscreen"))
        {
            Screen.SetResolution(2560, 1080, true);
            ResolutionManager.SetResolution(2560, 1080, true);
        }
        
        if (GUILayout.Button("Test Button"))
        {
            CooldownButton.Create<ClimbButton>();
        }

        if (GUILayout.Button("Teleport"))
        {
            void MouseUpEvent(CustomMapBehaviour instance, int mousebutton, Vector2 worldposition)
            {
                if (mousebutton != 1) return;
                instance.MouseUpEvent -= MouseUpEvent;
                instance.Parent!.Close();
                PlayerControl.LocalPlayer.moveable = true;
                Message(PlayerControl.LocalPlayer.NetTransform.transform.position.ToString());
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(worldposition);
                Message(PlayerControl.LocalPlayer.NetTransform.transform.position.ToString());
            }
            CustomMapBehaviour.ShowWithAllPlayers(new Color32(158, 240, 103, 255), MouseUpEvent);
        }
        
        if (CameraZoomController.Instance != null)
        {
            GUILayout.Label($"Camera Zoom: {CameraZoomController.Instance.OrthographicSize:F2}");
            CameraZoomController.Instance.OrthographicSize = GUILayout.HorizontalSlider(CameraZoomController.Instance.OrthographicSize, 1f, 24f);
        }
    }
}
