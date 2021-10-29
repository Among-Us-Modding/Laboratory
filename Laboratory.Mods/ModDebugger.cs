using System.Collections.Generic;
using Laboratory.Debugging;
using Laboratory.Mods.Utils.General;
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
            if (CameraZoomController.Instance)
            {
                CustomGUILayout.Label($"Camera Zoom: {CameraZoomController.Instance.OrthographicSize}");
                CameraZoomController.Instance.OrthographicSize = GUILayout.HorizontalSlider(CameraZoomController.Instance.OrthographicSize, 1f, 24f, DebugWindow.EmptyOptions);
            }
        }
    }
}