using System.Linq;
using HarmonyLib;

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
        ServerManager.DefaultRegions = LaboratoryPlugin.Instance.Regions.Select(s => {
            return new StaticRegionInfo(s.Name, StringNames.NoTranslation, s.Ip, new[]
            {
                new ServerInfo(s.Name + "-Master-1", s.Ip, s.Port)
            }).TryCast<IRegionInfo>();
        }).ToArray();
            
        __instance.StartCoroutine(__instance.ReselectRegionFromDefaults());

        return false;
    }
}