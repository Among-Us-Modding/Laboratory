using HarmonyLib;

namespace Laboratory.Mods.Patches
{
    [HarmonyPatch]
    internal static class EndGamePatches
    {
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
        [HarmonyPrefix]
        public static bool CheckEndCriteriaPatch() => !GameConfig.DisableEndGame;

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckTaskCompletion))]
        [HarmonyPrefix]
        public static bool CheckTaskCompletionPatch() => !GameConfig.DisableEndGame;

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.IsGameOverDueToDeath))]
        [HarmonyPostfix]
        public static void IsGameOverDueToDeathPatch(ref bool __result)
        {
            __result = !GameConfig.DisableEndGame && __result;
        }
    }
}
