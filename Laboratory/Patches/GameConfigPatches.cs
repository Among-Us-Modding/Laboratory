using HarmonyLib;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class GameConfigPatches
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

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    [HarmonyPrefix]
    public static bool CmdReportDeadBodyPatch()
    {
        return !GameConfig.DisableMeetings;
    }

    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    [HarmonyPrefix]
    public static bool PerformKillPatch()
    {
        return !GameConfig.DisableKillButton;
    }

    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.SetTarget))]
    [HarmonyPrefix]
    public static bool SetTargetPatch()
    {
        return !GameConfig.DisableKillButton;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive))]
    [HarmonyPostfix]
    public static void HideKillButtonPatch(HudManager __instance)
    {
        if (GameConfig.DisableKillButton)
        {
            __instance.KillButton.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(ArrowBehaviour), nameof(ArrowBehaviour.Awake))]
    [HarmonyPostfix]
    public static void ArrowBehaviourAwakePatch(ArrowBehaviour __instance)
    {
        if (!GameConfig.DisableTaskArrows || !__instance.image) return;
        __instance.image.sprite = null;
    }
}
