using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using Laboratory.Mods.Player.Attributes;
using Laboratory.Mods.Utils;
using Reactor;
using Reactor.Patches;
using System.Reflection;

namespace Laboratory.Mods
{
    [BepInAutoPlugin]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(LaboratoryPlugin.Id)]
    public partial class ModPlugin : BasePlugin
    {
        public static ModPlugin Instance => PluginSingleton<ModPlugin>.Instance;

        public Harmony Harmony { get; } = new(Id);

        public ModPlugin()
        {
            PlayerComponentAttribute.Initialize();
        }

        public override void Load()
        {
            Harmony.PatchAll();
            
            MapLoader.Instance = AddComponent<MapLoader>();
            ReactorVersionShower.TextUpdated += text => text.text += "\nLaboratory.Mods " + GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;;
        }
    }
}