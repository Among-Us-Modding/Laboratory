using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities.Extensions;

namespace Jungle.GamePatches;

[HarmonyPatch]
internal static class CustomServerPatches
{
    [HarmonyPatch(typeof(ServerManager), nameof(ServerManager.LoadServers))]
    [HarmonyPrefix]
    public static bool LoadServersPatch(ServerManager __instance)
    {
        var regions = ServerManager.DefaultRegions = __instance.AvailableRegions = JunglePlugin.Instance.Regions.Select(s =>
        {
            return new StaticRegionInfo(s.Name, StringNames.NoTranslation, s.Ip, new[]
            {
                new ServerInfo(s.Name + "-Master-1", s.Ip, s.Port, false)
            }).Cast<IRegionInfo>();
        }).ToArray();

        var currentRegion = __instance.CurrentRegion = regions.First();
        __instance.CurrentUdpServer = currentRegion.Servers.Random();

        return false;
    }
    
    [HarmonyPatch(typeof(AuthManager._CoWaitForNonce_d__6), nameof(AuthManager._CoWaitForNonce_d__6.MoveNext))]
    [HarmonyPrefix]
    public static bool CoWaitForNoncePatch(out bool __result)
    {
        return __result = false;
    }
    
    [HarmonyPatch(typeof(AuthManager._CoConnect_d__4), nameof(AuthManager._CoConnect_d__4.MoveNext))]
    [HarmonyPrefix]
    public static bool DontAuthenticateServerPatch(out bool __result)
    {
        return __result = false;
    }
}