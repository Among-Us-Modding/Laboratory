using System.IO;
using HarmonyLib;
using Steamworks;

namespace Laboratory.Patches.Misc;

[HarmonyPatch(typeof(SteamAPI), nameof(SteamAPI.RestartAppIfNecessary))]
public static class SteamAPI_RestartAppIfNecessary_Patch
{
    /// <summary>
    /// Prevents the game from restarting if steam is not initialized
    /// </summary>
    [HarmonyPrefix]
    public static bool Prefix(out bool __result)
    {
        if (!File.Exists("steam_appid.txt")) File.WriteAllText("steam_appid.txt", "945360");
        return __result = false;
    }
}
    
[HarmonyPatch(typeof(SteamAPI), nameof(SteamAPI.Init))]
public static class SteamAPI_Init_Patch
{
    /// <summary>
    /// Stops steam from initializing
    /// </summary>
    [HarmonyPrefix]
    public static bool Prefix() => false;
}