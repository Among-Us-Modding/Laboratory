using HarmonyLib;

namespace Jungle.GamePatches;

[HarmonyPatch]
public static class AchievementPatches
{
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.UnlockAchievement))]
    [HarmonyPrefix]
    public static bool DontUnlockAchievement() => false;
    
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.UpdateAchievementProgress))]
    [HarmonyPrefix]
    public static bool DontUpdateAchievement() => false;
    
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.UpdateAchievementsAndStats))]
    [HarmonyPrefix]
    public static bool DontUpdateStats() => false;
}