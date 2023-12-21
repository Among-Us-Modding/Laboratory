global using static Jungle.Test.TestPlugin;
global using static Reactor.Utilities.Logger<Jungle.JunglePlugin>;

using System.IO;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Jungle.Config;
using Jungle.Utils;
using Reactor;
using Reactor.Patches;
using Reactor.Utilities;
using UnityEngine;

namespace Jungle.Test;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(JunglePlugin.Id)]
[BepInDependency(ReactorPlugin.Id)]
public partial class TestPlugin : BasePlugin
{
    public static TestPlugin Instance => PluginSingleton<TestPlugin>.Instance;

    public Harmony Harmony { get; } = new(Id);
    public AssetManager Assets { get; } // = new(AssetBundle.LoadFromFile(Path.Combine(Paths.PluginPath, "bundles", Id.ToLower())));

    public static T LoadAsset<T>(string name) where T : Object => Instance.Assets.LoadAsset<T>(name);

    public override void Load()
    {
        Harmony.PatchAll();

        if (Assets?.Bundle != null) MapConfig.TextureSwapBundles.Add(Assets.Bundle);

        ReactorVersionShower.TextUpdated += text => text.text += "\n" + Name + " " + Version;
    }
}