using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace Laboratory.Debugging.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.SelectInfected))]
private static class SelectInfectedPatch
{
    [HarmonyArgument(Priority.Last)]
    public static bool Prefix()
    {
        if (!Enabled) return true;

        List<GameData.PlayerInfo>? impostors = new List<GameData.PlayerInfo>();

        Il2CppArrayBase<GameData.PlayerInfo>? allPlayers = GameData.Instance.AllPlayers.ToArray();
        foreach (string? name in CurrentlySelected)
        {
            GameData.PlayerInfo? match = allPlayers.FirstOrDefault(p => p.PlayerName == name);
            if (match == null || match.Disconnected) continue;
            impostors.Add(match);
        }

        PlayerControl.LocalPlayer.RpcSetInfected(impostors.ToArray());
        return false;
    }
}