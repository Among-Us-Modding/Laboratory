using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppSystem;
using Laboratory.Extensions;

namespace Laboratory.Debugging.Patches;

[HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
public static class SelectRolesPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    public static bool Prefix()
    {
        if (!ForceImpostorTab.Enabled) return true;

        List<GameData.PlayerInfo> list = GameData.Instance.AllPlayers.ToArray().Where(pcd => !pcd.Disconnected && !pcd.IsDead).ToList();
        List<GameData.PlayerInfo> impostors = new();

        foreach (string name in ForceImpostorTab.CurrentlySelected)
        {
            GameData.PlayerInfo match = list.FirstOrDefault(p => p.PlayerName == name);
            if (match == null || match.Disconnected) continue;
            impostors.Add(match);
        }
        
        List<GameData.PlayerInfo> crew = list.Where(e => !impostors.Contains(e)).ToList();
        
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