using System;
using System.Collections.Generic;
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
        
        if (CameraZoomController.Instance != null)
        {
            GUILayout.Label($"Camera Zoom: {CameraZoomController.Instance.OrthographicSize:F2}");
            CameraZoomController.Instance.OrthographicSize = GUILayout.HorizontalSlider(CameraZoomController.Instance.OrthographicSize, 1f, 24f);
        }
    }
}
