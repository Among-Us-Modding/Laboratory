using System;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
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
        
        public override void Load()
        {
            Harmony.CreateAndPatchAll(GetType().Assembly);
            
            ReactorVersionShower.TextUpdated += text => text.text += "\nLaboratory.Mods Loaded";
        }
    }
}