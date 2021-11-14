using System.Linq;
using HarmonyLib;
using Reactor.Extensions;

namespace Laboratory.Patches.AppData;

[HarmonyPatch(typeof(ServerManager), nameof(ServerManager.LoadServers))]
public static class ServerManager_LoadServers_Patch
{
    /// <summary>
    /// Adds custom regions to the ServerManager
    /// </summary>
    [HarmonyPrefix]
    public static bool Prefix(ServerManager __instance)
    {
        var regions = ServerManager.DefaultRegions = __instance.AvailableRegions = LaboratoryPlugin.Instance.Regions.Select(s =>
        {
            return new StaticRegionInfo(s.Name, StringNames.NoTranslation, s.Ip, new[]
            {
                new ServerInfo(s.Name + "-Master-1", s.Ip, s.Port)
            }).Cast<IRegionInfo>();
        }).ToArray();

        var currentRegion = __instance.CurrentRegion = regions.First();
        __instance.CurrentServer = currentRegion.Servers.Random();

        return false;
    }
}
