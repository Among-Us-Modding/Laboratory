using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
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

        int num1 = 0;
        RoleManager.AssignRolesFromList(impostors.ToIl2CppList(), impostors.Count, 
            Enumerable.Repeat(RoleTypes.Impostor, impostors.Count).ToList().ToIl2CppList(), ref num1);

        int num2 = 0;
        List<GameData.PlayerInfo> crew = list.Where(e => !impostors.Contains(e)).ToList();
        RoleManager.AssignRolesFromList(crew.ToIl2CppList(), crew.Count,
            Enumerable.Repeat(RoleTypes.Crewmate, crew.Count).ToList().ToIl2CppList(), ref num2);
        
        return false;
    }
}