using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Laboratory.Attributes;
using Laboratory.Debugging;
using Laboratory.Utils;
using Reactor;
using Reactor.Networking.MethodRpc;
using Reactor.Patches;
using UnityEngine.SceneManagement;

namespace Laboratory
{
    [BepInDependency(ReactorPlugin.Id)]
    [BepInProcess("Among Us.exe")]
    [BepInPlugin(GUID, Name, Version)]
    public class LaboratoryPlugin : BasePlugin
    {
        public const string GUID = "Laboratory";
        public const string Name = "Laboratory";
        public const string Version = "0.0.0";

        /// <summary>
        /// The name of the subfolder used for appdata
        /// </summary>
        public string AppDataSubFolderName { get; set; } = "Laboratory";
        
        /// <summary>
        /// Custom regions which will be added
        /// </summary>
        public List<ServerInfo> Regions { get; } = new()
        {
            new ServerInfo("Modded", "3.80.231.147", 22023),
        };
        
        public static LaboratoryPlugin Instance => PluginSingleton<LaboratoryPlugin>.Instance;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(GetType().Assembly);
            KeybindAttribute.Initialize();
            Debugger.Initialize();
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) OnSceneLoaded);
            AddComponent<UnityEvents>();

            DebugWindow.Instance = AddComponent<DebugWindow>();
            ReactorVersionShower.TextUpdated += text => text.text += "\nLaboratory Loaded";
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
}
