using System;
using UnhollowerBaseLib;
using UnityEngine;

namespace Laboratory.Debugging
{
    public static class CustomGUILayout
    {
        private static readonly Il2CppReferenceArray<GUILayoutOption> EmptyOptions = new(0);
        
        /// <summary>
        /// Makes a divider on a debug window
        /// </summary>
        public static void Divider() => GUILayout.Label("", GUI.skin.horizontalSlider, EmptyOptions);

        /// <summary>
        /// Makes a button which when pressed will invoke an action
        /// </summary>
        /// <param name="label">Button label</param>
        /// <param name="action">Action to be invoked on use</param>
        /// <returns></returns>
        public static bool Button(string label, Action? action = null)
        {
            if (!GUILayout.Button(label, EmptyOptions)) return false;
            action?.Invoke();
            return true;
        }

        public static void Label(string label) => GUILayout.Label(label, EmptyOptions);
    }
}