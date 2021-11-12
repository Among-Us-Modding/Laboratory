using System;
using UnityEngine;

namespace Laboratory.Debugging
{
    public static class CustomGUILayout
    {
        /// <summary>
        /// Makes a divider on a debug window
        /// </summary>
        public static void Divider() => GUILayout.Label("", GUI.skin.horizontalSlider);

        /// <summary>
        /// Makes a button which when pressed will invoke an action
        /// </summary>
        /// <param name="label">Button label</param>
        /// <param name="action">Action to be invoked on use</param>
        /// <returns></returns>
        public static bool Button(string label, Action? action = null)
        {
            if (!GUILayout.Button(label)) return false;
            action?.Invoke();
            return true;
        }

        public static void Label(string label) => GUILayout.Label(label);
    }
}