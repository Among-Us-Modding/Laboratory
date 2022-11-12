global using static Reactor.Utilities.Logger<Laboratory.LaboratoryPlugin>;

using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Laboratory.Attributes;
using Laboratory.Debugging;
using Laboratory.Player.Attributes;
using Laboratory.Utils;
using Reactor;
using Reactor.Patches;
using Reactor.Utilities;
using UnityEngine.SceneManagement;

namespace Laboratory;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class LaboratoryPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    /// <summary>
    /// The name of the subfolder used for appdata
    /// </summary>
    public string AppDataSubFolderName { get; set; } = "Laboratory";

    /// <summary>
    /// Custom regions which will be added
    /// </summary>
    public List<ServerInfo> Regions { get; } = new()
    {
        new ServerInfo("Modded", "64.227.0.249", 22023),
    };

    public static LaboratoryPlugin Instance => PluginSingleton<LaboratoryPlugin>.Instance;

    public LaboratoryPlugin()
    {
        KeybindAttribute.Initialize();
        BaseDebugTab.Initialize();
        PlayerComponentAttribute.Initialize();
    }

    public override void Load()
    {
        Harmony.PatchAll();
        DebugWindow.Instance = AddComponent<DebugWindow>();
        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)OnSceneLoaded);
        
        AddComponent<MapLoader>();
        AddComponent<UnityEvents>();

        ReactorVersionShower.TextUpdated += text => text.text += "\nLaboratory " + Version;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        switch (scene.name)
        {
            // Starts account login process
            case "MainMenu":
                EOSManager.Instance.InitializePlatformInterface();
                break;
        }
    }
}
