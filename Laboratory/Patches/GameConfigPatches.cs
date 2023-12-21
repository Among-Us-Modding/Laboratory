using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Laboratory.Config;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch]
public static class GameConfigPatches
{
    [HarmonyPatch(typeof(LogicGameFlow), nameof(LogicGameFlow.CheckEndCriteria))]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlow.CheckEndCriteria))]
    [HarmonyPatch(typeof(LogicGameFlowHnS), nameof(LogicGameFlow.CheckEndCriteria))]
    [HarmonyPrefix]
    public static bool DisableEndGamePatch()
    {
        return !GameConfig.DisableEndGame;
    }
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    [HarmonyPrefix]
    public static bool DisableMeetingsPatch()
    {
        return !GameConfig.DisableMeetings;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive), typeof(PlayerControl), typeof(RoleBehaviour), typeof(bool))]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPostfix]
    public static void DisableUIElementsPatch(HudManager __instance)
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
            __instance.TaskPanel.gameObject.SetActive(false);
        }
    }
    
    [HarmonyPatch(typeof(ArrowBehaviour), nameof(ArrowBehaviour.Awake))]
    [HarmonyPostfix]
    public static void DisableArrowsPatch(ArrowBehaviour __instance)
    {
        if (!GameConfig.DisableTaskArrows || !__instance.image) return;
        __instance.image.sprite = null;
    }
    
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.CoShowIntro))]
    [HarmonyPrefix]
    public static bool DisableIntroPatch(HudManager __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        if (!GameConfig.DisableIntroCutscene)
        {
            return true;
        }

        __result = ModifiedCoShowIntro(__instance).WrapToIl2Cpp();
        return false;
    }
    
    private static IEnumerator ModifiedCoShowIntro(HudManager hudManager)
    {
        while (!ShipStatus.Instance)
        {
            yield return null;
        }
        hudManager.IsIntroDisplayed = true;
        hudManager.LobbyTimerExtensionUI.HideAll();
        DestroyableSingleton<HudManager>.Instance.FullScreen.transform.localPosition = new Vector3(0f, 0f, -250f);
        PlayerControl.LocalPlayer.SetKillTimer(10f);
        
        ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().SetInitialSabotageCooldown();
        ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Doors, out var systemType);
        if (systemType.TryCast<IDoorSystem>() is { } doorSystem)
        {
            doorSystem.SetInitialSabotageCooldown();
        }
        
        yield return ShipStatus.Instance.PrespawnStep();
        PlayerControl.LocalPlayer.AdjustLighting();
        yield return hudManager.CoFadeFullScreen(Color.black, Color.clear, 0.2f, false);
        hudManager.FullScreen.transform.localPosition = new Vector3(0f, 0f, -500f);
        hudManager.IsIntroDisplayed = false;
        hudManager.CrewmatesKilled.gameObject.SetActive(GameManager.Instance.ShowCrewmatesKilled());
        GameManager.Instance.StartGame();
    }
    
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void DisablePingTrackerPatch(PingTracker __instance)
    {
        __instance.gameObject.SetActive(false);
    }
}