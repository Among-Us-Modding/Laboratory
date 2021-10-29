using System;
using Reactor;
using Reactor.Extensions;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Debugging
{
    [RegisterInIl2Cpp]
    public class DebugWindow : MonoBehaviour
    {
        /// <summary>
        /// Default empty GUILayoutOptions array
        /// </summary>
        public static Il2CppReferenceArray<GUILayoutOption> EmptyOptions { get; } = new(0);
        
        /// <summary>
        /// Current Instance of the debug window
        /// </summary>
        public static DebugWindow Instance { get; set; }

        public DebugWindow(IntPtr ptr) : base(ptr) { }

        /// <summary>
        /// If the debug window is enabled
        /// </summary>
        public bool Enabled { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
        
        /// <summary>
        /// Index into the tabs array which is currently active
        /// </summary>
        public int SelectedTab { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
        
        /// <summary>
        /// Window Id used to build windw
        /// </summary>
        public int WindowId { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
        
        /// <summary>
        /// Rect of the debug window
        /// </summary>
        private Rect PrimaryWindow { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; } = new(20, 20, 100, 100);
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) Enabled = !Enabled;
        }

        private void OnGUI()
        {
            if (!Enabled) return;

            WindowId = 0;
            PrimaryWindow = new Rect(PrimaryWindow.x, PrimaryWindow.y, 20, 20);
            PrimaryWindow = GUILayout.Window(WindowId++, PrimaryWindow, (Action<int>) BuildPrimaryWindow, "Debug", EmptyOptions);
            PrimaryWindow.ClampScreen();

            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && PrimaryWindow.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
            {
                Input.ResetInputAxes();
            }
        }

        [HideFromIl2Cpp]
        private void BuildPrimaryWindow(int windowId)
        {
            try
            {
                GUILayout.BeginVertical(EmptyOptions);

                bool anyActive = false;
                GUILayout.BeginHorizontal(EmptyOptions);
                for (int index = 0; index < Debugger.Tabs.Count; index++)
                {
                    DebugTab debugTab = Debugger.Tabs[index];
                    if (!debugTab.Visible()) continue;
                    if (GUILayout.Toggle(SelectedTab == index, debugTab.TabName, GUI.skin.button, EmptyOptions)) SelectedTab = index;
                    anyActive = true;
                }

                GUILayout.EndHorizontal();

                if (anyActive)
                {
                    CustomGUILayout.Divider();
                    Debugger.Tabs[SelectedTab].BuildUI();
                }

                GUILayout.EndVertical();
                GUI.DragWindow();
            }
            catch (Exception e)
            {
                Logger<LaboratoryPlugin>.Error(e);
            }
        }
    }
}