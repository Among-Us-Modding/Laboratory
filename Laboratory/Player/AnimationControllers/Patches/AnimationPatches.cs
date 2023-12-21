using System;
using HarmonyLib;
using Laboratory.Player.Extensions;
using Laboratory.Player.Managers;
using PowerTools;
using UnityEngine;

namespace Laboratory.Player.AnimationControllers.Patches;

[HarmonyPatch]
internal static class AnimationPatches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.DoingCustomAnimation), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool DoingCustomAnimationGetPatch(PlayerPhysics __instance, ref bool __result)
    {
        var manager = __instance.GetPlayerManager();
        if (manager is not { AnimationController.IsPlayingCustomAnimation: true }) return true;
        return __result = false;
    }
    
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
    [HarmonyPrefix]
    public static bool AdjustOffsets(PlayerPhysics __instance)
    {
        IAnimationController anim = __instance.myPlayer.GetPlayerManager().AnimationController;
        if (anim == null) return true;

        Transform transform = __instance.transform;
        Vector3 position = transform.position;
        position.z = (position.y - anim.RendererOffset.y) / 1000f + anim.ZOffset;
        transform.position = position;

        return false;
    }
}

/// <summary>
/// This should allow SpriteAnimNodeSync components to default to the first node if it is trying to reference a missing node
/// </summary>
[HarmonyPatch(typeof(SpriteAnimNodeSync), nameof(SpriteAnimNodeSync.LateUpdate))]
internal static class ForceSpriteAnimNodePatch
{
    public static void Prefix(SpriteAnimNodeSync __instance, out int __state)
    {
        __state = -1;
        if (!__instance.Parent) return;
        if (__instance.NodeId != 1) return;
        if (__instance.Parent.m_node0 == default && Math.Abs(__instance.Parent.m_ang0) < 0.05) return;
        if (__instance.Parent.m_node1 != default || Math.Abs(__instance.Parent.m_ang1) > 0.05) return;

        __state = 1;
        __instance.NodeId = 0;
    }

    public static void Postfix(SpriteAnimNodeSync __instance, int __state)
    {
        if (__state < 0) return;
        __instance.NodeId = __state;
    }
}
