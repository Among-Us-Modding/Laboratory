global using static Reactor.Utilities.Logger<Laboratory.LaboratoryPlugin>;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Laboratory.Attributes;
using Laboratory.Debugging;
using Laboratory.Player.Attributes;
using Laboratory.Utilities;
using Reactor;
using Reactor.Patches;
using Reactor.Utilities;

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
    public IRegionInfo[] Regions { get; } = new[]
    {
        new StaticHttpRegionInfo("Modded", StringNames.NoTranslation, "au-eu.duikbo.at", new ServerInfo[]
        {
            new ServerInfo("Http-1", "https://au-eu.duikbo.at", 443, false),
        }).Cast<IRegionInfo>(),
        new StaticRegionInfo("Local", StringNames.NoTranslation, "localhost", new ServerInfo[]
        {
            new ServerInfo("Localhost", "127.0.0.1", 22023, false),
        }).Cast<IRegionInfo>(),
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
        
        AddComponent<MapLoader>();
        AddComponent<UpdateEvents>();

        ReactorVersionShower.TextUpdated += text => text.text += "\nLaboratory " + Version;
    }
}
