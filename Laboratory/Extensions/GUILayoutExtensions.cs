using UnityEngine;

namespace Laboratory.Extensions;

public static class GUILayoutExtensions
{
    /// <summary>
    /// Makes a divider on a debug window
    /// </summary>
    public static void Divider() => GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);
}