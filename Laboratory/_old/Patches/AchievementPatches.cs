using HarmonyLib;

namespace Laboratory.Patches;

public class AchievementPatches
{
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.OnMatchStart))]
    public static class AchievementManager_OnMatchStart_Patch
    {
        public static bool Prefix() => false;
    }
    
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.OnMatchEnd))]
    public static class AchievementManager_OnMatchEnd_Patch
    {
        public static bool Prefix() => false;
    } 
    
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.OnConsoleUse))]
    public static class AchievementManager_OnConsoleUse_Patch
    {
        public static bool Prefix() => false;
    }
    
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.OnMurder))]
    public static class AchievementManager_OnMurder_Patch
    {
        public static bool Prefix() => false;
    }
    //

    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.OnTaskComplete))]
    public static class AchievementManager_OnTaskComplete_Patch
    {
        public static bool Prefix() => false;
    }
    
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.OnTaskFailure))]
    public static class AchievementManager_OnTaskFailure_Patch
    {
        public static bool Prefix() => false;
    }
}