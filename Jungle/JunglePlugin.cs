global using static Reactor.Utilities.Logger<Jungle.JunglePlugin>;

using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Jungle.Debugging;
using Reactor;
using Reactor.Patches;
using Reactor.Utilities;

namespace Jungle;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class JunglePlugin : BasePlugin
{
    private Harmony Harmony { get; } = new(Id);

    public string AppDataSubFolderName { get; set; } = "Laboratory";

    public List<ServerInfo> Regions { get; } = new()
    {
        new ServerInfo("Modded", "78.47.142.18", 22023),
    };
    
    public static JunglePlugin Instance => PluginSingleton<JunglePlugin>.Instance;

    public JunglePlugin()
    {
        BaseDebugTab.Initialize();
    }
    
    public override void Load()
    {
        Harmony.PatchAll();
        
        DebugWindow.Instance = AddComponent<DebugWindow>();
        
        ReactorVersionShower.TextUpdated += text => text.text += "\nJungles " + Version;
    }
}