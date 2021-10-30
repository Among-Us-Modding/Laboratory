using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using Laboratory.Mods.Player.Attributes;
using Reactor;
using Reactor.Patches;

namespace Laboratory.Mods
{
    [BepInDependency(LaboratoryPlugin.GUID)]
    [BepInProcess("Among Us.exe")]
    [BepInPlugin(GUID, Name, Version)]
    public class ModPlugin : BasePlugin
    {
        public const string GUID = "Mods";
        public const string Name = "Mods";
        public const string Version = "0.0.0";
        
        public static ModPlugin Instance => PluginSingleton<ModPlugin>.Instance;
        
        public override void Load()
        {
            Harmony.CreateAndPatchAll(GetType().Assembly);
            PlayerComponentAttribute.Initialize();
            ReactorVersionShower.TextUpdated += text => text.text += "\nLaboratory.Mods Loaded";
        }
    }
}