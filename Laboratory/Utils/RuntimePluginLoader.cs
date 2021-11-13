using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.IL2CPP;
using HarmonyLib;
using Mono.Cecil;
using Reactor;
using Version = SemanticVersioning.Version;

namespace Laboratory.Utils;

public static class RuntimePluginLoader
{
    public static string PluginPath { get; } = Path.Combine(Paths.BepInExRootPath, "unloaded");

    public static void DownloadPlugin(string pluginName) => DownloadPlugins(new[] { pluginName });

    public static HashSet<string> Loaded { get; } = new();

    public static void DownloadPlugins(string[] pluginsToLoad)
    {
        pluginsToLoad = pluginsToLoad.Where(Loaded.Add).ToArray();

        Dictionary<string, MemoryStream> dllStreams = new();
        foreach (var pluginName in pluginsToLoad)
        {
            try
            {
                var fileStream = File.OpenRead(Path.Combine(PluginPath, pluginName + ".dll"));
                MemoryStream ms = new();
                fileStream.CopyTo(ms);
                dllStreams.Add(pluginName, ms);
                fileStream.Dispose();
            }
            catch (Exception e)
            {
                Logger<LaboratoryPlugin>.Error(e);
            }
        }

        List<PluginInfo> pluginInfos = new();
        foreach (var (pluginName, stream) in dllStreams)
        {
            stream.Position = 0;
            using var asmDef = AssemblyDefinition.ReadAssembly(stream, TypeLoader.ReaderParameters);
            var plugin = asmDef.MainModule.Types.Select(t => BaseChainloader<BasePlugin>.ToPluginInfo(t, pluginName)).First(t => t != null);
            pluginInfos.Add(plugin);
        }

        var methodInfo = AccessTools.Method(typeof(BaseChainloader<BasePlugin>), "ModifyLoadOrder", new[] { typeof(IList<PluginInfo>) });
        var sortedPlugins = (IList<PluginInfo>)methodInfo.Invoke(IL2CPPChainloader.Instance, new object[] { pluginInfos });

        Dictionary<string, Version> processedPlugins = new();
        Dictionary<string, Assembly> loadedAssemblies = new();

        foreach (var plugin in sortedPlugins)
        {
            List<BepInDependency> missingDependencies = new();

            foreach (var dependency in plugin.Dependencies)
            {
                static bool IsHardDependency(BepInDependency dep) => (dep.Flags & BepInDependency.DependencyFlags.HardDependency) != 0;

                var dependencyExists = processedPlugins.TryGetValue(dependency.DependencyGUID, out var pluginVersion);
                if (dependencyExists && (dependency.VersionRange == null || dependency.VersionRange.IsSatisfied(pluginVersion))) continue;
                if (IsHardDependency(dependency)) missingDependencies.Add(dependency);
            }

            processedPlugins.Add(plugin.Metadata.GUID, plugin.Metadata.Version);

            if (missingDependencies.Count != 0) continue;
            if (!loadedAssemblies.TryGetValue(plugin.Location, out var ass)) loadedAssemblies[plugin.Location] = ass = Assembly.Load(dllStreams[plugin.Location].ToArray());

            IL2CPPChainloader.Instance.Plugins[plugin.Metadata.GUID] = plugin;
            AccessTools.Method(typeof(IL2CPPChainloader), "TryRunModuleCtor", new[] { typeof(PluginInfo), typeof(Assembly) }).Invoke(IL2CPPChainloader.Instance, new object[] { plugin, ass });
            AccessTools.PropertySetter(typeof(PluginInfo), nameof(PluginInfo.Instance)).Invoke(plugin, new object[] { IL2CPPChainloader.Instance.LoadPlugin(plugin, ass) });
        }
    }

    public static IEnumerable<string> GetAvailableMods()
    {
        var files = Directory.GetFiles(PluginPath);
        foreach (var fileName in files)
        {
            yield return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
