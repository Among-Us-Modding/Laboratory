using System;
using System.Linq;
using System.Reflection;
using Reactor;

namespace Laboratory.Debugging;

/// <summary>
/// Debug tab which will be added to the debug window on plugin load
/// </summary>
public abstract class BaseDebugTab
{
    internal static void Initialize()
    {
        ChainloaderHooks.PluginLoad += plugin => Register(plugin.GetType().Assembly);
    }

    private static void Register(Assembly assembly)
    {
        foreach (Type? type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseDebugTab)) && !t.IsAbstract))
        {
            BaseDebugTab? debugger = (BaseDebugTab)Activator.CreateInstance(type);
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