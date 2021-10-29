using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reactor;

namespace Laboratory.Debugging
{
    public abstract class Debugger
    {
        /// <summary>
        /// List of tabs in the debug window
        /// </summary>
        internal static List<DebugTab> Tabs { get; } = new();

        internal static void Initialize()
        {
            ChainloaderHooks.PluginLoad += plugin => Register(plugin.GetType().Assembly);
        }

        private static void Register(Assembly asm)
        {
            foreach (Type type in asm.GetTypes().Where(typeof(Debugger).IsAssignableFrom))
            {
                Debugger debugger = (Debugger) Activator.CreateInstance(type);
                Tabs.AddRange(debugger.DebugTabs());
            }
        }
        
        /// <summary>
        /// Tabs which will be added to the main debugger window
        /// This is called once during initialization
        /// </summary>
        public abstract IEnumerable<DebugTab> DebugTabs();
    }
}