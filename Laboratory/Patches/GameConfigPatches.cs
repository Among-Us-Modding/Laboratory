using System.Collections;
using BepInEx.IL2CPP.Utils.Collections;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

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
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPostfix]
    public static void HideButtonsPatch(HudManager __instance)
    {
        if (GameConfig.DisableKillButton)
        {
            __instance.KillButton.gameObject.SetActive(false);
        }

        if (GameConfig.DisableReportButton)
        {
            __instance.ReportButton.gameObject.SetActive(false);
        }

        if (GameConfig.DisableTaskPanel)
        {
            __instance.TaskText.transform.parent.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(ArrowBehaviour), nameof(ArrowBehaviour.Awake))]
    [HarmonyPostfix]
    public static void ArrowBehaviourAwakePatch(ArrowBehaviour __instance)
    {
        if (!GameConfig.DisableTaskArrows || !__instance.image) return;
        __instance.image.sprite = null;
    }

    [HarmonyPatch(typeof(TaskPanelBehaviour), nameof(TaskPanelBehaviour.Update))]
    [HarmonyPrefix]
    public static bool TaskPanelPatch(TaskPanelBehaviour __instance)
    {
        if (GameConfig.DisableTaskPanel)
        {
            __instance.gameObject.SetActive(false);
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.CoShowIntro))]
    [HarmonyPrefix]
    public static bool CoShowIntroPatch(HudManager __instance, List<PlayerControl> yourTeam, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        if (!GameConfig.DisableIntroCutscene)
        {
            return true;
        }

        __result = CoShowIntro(__instance, yourTeam).WrapToIl2Cpp();
        return false;
    }


    private static IEnumerator CoShowIntro(HudManager hudManager, List<PlayerControl> yourTeam)
    {
        while (!ShipStatus.Instance)
        {
            yield return null;
        }

        hudManager.isIntroDisplayed = true;
        DestroyableSingleton<HudManager>.Instance.FullScreen.transform.SetLocalZ(-250f);
        PlayerControl.LocalPlayer.SetKillTimer(10f);
        ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().ForceSabTime(10f);
        yield return ShipStatus.Instance.PrespawnStep();
        yield return hudManager.CoFadeFullScreen(Color.black, Color.clear);
        DestroyableSingleton<HudManager>.Instance.FullScreen.transform.SetLocalZ(-500f);
        hudManager.isIntroDisplayed = false;
    }
}
