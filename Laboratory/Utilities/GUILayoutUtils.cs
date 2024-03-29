﻿using UnityEngine;

namespace Laboratory.Utilities;

public static class GUILayoutUtils
{
    /// <summary>
    /// Makes a divider on a debug window
    /// </summary>
    public static void Divider() => GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);
}