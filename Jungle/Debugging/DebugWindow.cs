using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.Attributes;
using Jungle.Utils;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.ImGui;
using UnityEngine;

namespace Jungle.Debugging;

[RegisterInIl2Cpp]
public class DebugWindow : MonoBehaviour
{
    /// <summary>
    /// Current Instance of the debug window
    /// </summary>
    public static DebugWindow Instance { get; set; }

    /// <summary>
    /// List of tabs in the debug window
    /// </summary>
    public static List<BaseDebugTab> Tabs { get; } = new();

    /// <summary>
    /// If the debug window is enabled
    /// </summary>
    [HideFromIl2Cpp]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Index into the tabs array which is currently active
    /// </summary>
    [HideFromIl2Cpp]
    public BaseDebugTab SelectedTab { get; set; }

    /// <summary>
    /// Draw window used to draw debug components
    /// </summary>
    [HideFromIl2Cpp]
    private DragWindow Window { get; }

    public DebugWindow(IntPtr ptr) : base(ptr)
    {
        Window = new DragWindow(new Rect(20, 20, 100, 100), "Debug", BuildWindow);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) Enabled = !Enabled;
    }

    private void OnGUI()
    {
        if (!Enabled) return;

        Window.OnGUI();

        if (SelectedTab is { IsVisible: true })
        {
            SelectedTab.OnGUI();
        }
    }

    [HideFromIl2Cpp]
    private void BuildWindow()
    {
        try
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            foreach (var tab in Tabs)
            {
                if (GUILayout.Toggle(SelectedTab == tab, tab.Name, GUI.skin.button))
                {
                    SelectedTab = tab;
                }
            }

            GUILayout.EndHorizontal();


            if (SelectedTab is { IsVisible: true })
            {
                GUILayoutUtils.Divider();
                SelectedTab?.BuildUI();
            }

            GUILayout.EndVertical();
        }
        catch (Exception e)
        {
            Error(e);
        }
    }
}
