using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;

namespace Laboratory.Debugging.Patches;

[HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
public static class SelectRolesPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    public static bool Prefix()
    {
        if (!ForceImpostorTab.Enabled) return true;

        List<NetworkedPlayerInfo> list = GameData.Instance.AllPlayers.ToArray().Where(pcd => !pcd.Disconnected && !pcd.IsDead).ToList();
        List<NetworkedPlayerInfo> impostors = new();

        foreach (string name in ForceImpostorTab.CurrentlySelected)
        {
            NetworkedPlayerInfo match = list.FirstOrDefault(p => p.PlayerName == name);
            if (match == null || match.Disconnected) continue;
            impostors.Add(match);
        }

        List<NetworkedPlayerInfo> crew = list.Where(e => !impostors.Contains(e)).ToList();

        foreach (var playerInfo in impostors)
        {
            playerInfo.Object.RpcSetRole(RoleTypes.Impostor);
        }

        foreach (var playerInfo in crew)
        {
            playerInfo.Object.RpcSetRole(RoleTypes.Crewmate);
        }
        return false;
    }
}
