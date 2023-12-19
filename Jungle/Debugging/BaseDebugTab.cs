using System;
using System.Linq;
using System.Reflection;
using BepInEx.Unity.IL2CPP;

namespace Jungle.Debugging;

/// <summary>
/// Debug tab which will be added to the debug window on plugin load
/// </summary>
public abstract class BaseDebugTab
{
    internal static void Initialize()
    {
        IL2CPPChainloader.Instance.PluginLoad += (_, assembly, _) => Register(assembly);
    }

    private static void Register(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseDebugTab)) && !t.IsAbstract))
        {
            BaseDebugTab debugger = (BaseDebugTab)Activator.CreateInstance(type)!;
            DebugWindow.Tabs.Add(debugger);
        }
    }

    public abstract string Name { get; }
    public virtual bool IsVisible => true;

    public abstract void BuildUI();

    public virtual void OnGUI()
    {
    }
}