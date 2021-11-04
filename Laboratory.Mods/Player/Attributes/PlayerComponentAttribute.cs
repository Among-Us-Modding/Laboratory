using System;
using System.Collections.Generic;
using System.Reflection;
using Reactor;

namespace Laboratory.Mods.Player.Attributes
{
    public class PlayerComponentAttribute : Attribute
    {
        internal static readonly HashSet<Type> PlayerComponentTypes = new();

        internal static void Initialize()
        {
            ChainloaderHooks.PluginLoad += plugin => Register(plugin.GetType().Assembly);
        }
        
        private static void Register(Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.GetCustomAttribute<PlayerComponentAttribute>() is not null)
                {
                    PlayerComponentTypes.Add(type);
                }
            }
        }
    }
}