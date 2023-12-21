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
        
        IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
        int numImpostors = impostors.Count;
        
        GameManager.Instance.LogicRoleSelection.AssignRolesForTeam(
            impostors.ToIl2CppList(), currentGameOptions, 
            RoleTeamTypes.Impostor, numImpostors, new Nullable<RoleTypes>(RoleTypes.Impostor));
        GameManager.Instance.LogicRoleSelection.AssignRolesForTeam(
            crew.ToIl2CppList(), currentGameOptions, RoleTeamTypes.Crewmate, int.MaxValue,
            new Nullable<RoleTypes>(RoleTypes.Crewmate));

        
        return false;
    }
}