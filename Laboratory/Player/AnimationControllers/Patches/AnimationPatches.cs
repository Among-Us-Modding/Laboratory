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
    public static AnimationClip GetChangedClip(PlayerManager playerManager, AnimationClip original)
    {
        IAnimationController animationController = playerManager.AnimationController;
        if (animationController == null) return original;

        PlayerPhysics.AnimationGroup playerPhysics = playerManager.Physics.CurrentAnimationGroup;
        if (playerPhysics == null) return original;

        if (original == playerPhysics.IdleAnim) return animationController.IdleAnim;
        if (original == playerPhysics.GhostIdleAnim) return animationController.GhostIdleAnim;
        if (original == playerPhysics.RunAnim) return animationController.RunAnim;
        if (original == playerPhysics.EnterVentAnim) return animationController.EnterVentAnim;
        if (original == playerPhysics.ExitVentAnim) return animationController.ExitVentAnim;
        if (original == playerPhysics.SpawnAnim) return animationController.SpawnAnim;
        if (original == playerPhysics.ClimbDownAnim) return animationController.ClimbDownAnim;
        if (original == playerPhysics.ClimbAnim) return animationController.ClimbAnim;

        return original;
    }

    public static AnimationClip GetOriginalClip(PlayerManager playerManager, AnimationClip changed)
    {
        if (!changed) return changed;

        IAnimationController animationController = playerManager.AnimationController;
        if (animationController == null) return changed;

        PlayerPhysics.AnimationGroup playerPhysics = playerManager.Physics.CurrentAnimationGroup;
        if (playerPhysics == null) return changed;

        if (changed == animationController.IdleAnim) return playerPhysics.IdleAnim;
        if (changed == animationController.GhostIdleAnim) return playerPhysics.GhostIdleAnim;
        if (changed == animationController.RunAnim) return playerPhysics.RunAnim;
        if (changed == animationController.EnterVentAnim) return playerPhysics.EnterVentAnim;
        if (changed == animationController.ExitVentAnim) return playerPhysics.ExitVentAnim;
        if (changed == animationController.SpawnAnim) return playerPhysics.SpawnAnim;
        if (changed == animationController.ClimbDownAnim) return playerPhysics.ClimbDownAnim;
        if (changed == animationController.ClimbAnim) return playerPhysics.ClimbAnim;

        return changed;
    }


    // SpriteAnim.GetCurrentAnimation is too short to patch
    // Imagine this is a prefix which is enabled before PlayerPhysics.HandleAnimation and disabled afterwards
    // As such we will just call this in the reimplemented version of HandleAnimation as would be done in a postfix
    public static void GetCurrentAnimationPatch(SpriteAnim __instance, ref AnimationClip __result)
    {
        if (__result == null) return;

        Transform parent = __instance.transform.parent;
        if (!parent) return;

        PlayerManager playerManager = parent.GetPlayerManager();
        if (playerManager is not { AnimationController: { } }) return;
        
        // This ensures that when a custom animation finishes it will allow the normal animation cycle to continue
        bool actuallyPlaying = __instance.Clip is {isLooping: true} || __instance.IsPlaying();
        __result = actuallyPlaying ? playerManager.AnimationController.IsPlayingCustomAnimation ? playerManager.Physics.CurrentAnimationGroup.ClimbAnim : GetOriginalClip(playerManager, __result) : null;
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    [HarmonyPrefix]
    public static bool HandleAnimationReimplemented(PlayerPhysics __instance, [HarmonyArgument(0)] bool amDead)
    {
        if (__instance.Animator.IsPlaying(__instance.CurrentAnimationGroup.SpawnAnim) || !GameData.Instance)
        {
            return false;
        }

        Vector2 velocity = __instance.body.velocity;
        AnimationClip currentAnimation = __instance.Animator.GetCurrentAnimation();
        // PATCH START
        // Here we image that we are running GetCurrentAnimationPatch as a postifx patch for GetCurrentAnimation
        GetCurrentAnimationPatch(__instance.Animator, ref currentAnimation);
        // PATCH END
        if (currentAnimation == __instance.CurrentAnimationGroup.ClimbAnim || currentAnimation == __instance.CurrentAnimationGroup.ClimbDownAnim)
        {
            return false;
        }
        
        if (!amDead)
        {
            if (velocity.sqrMagnitude >= 0.05f)
            {
                bool flipX = __instance.FlipX;
                if (velocity.x < -0.01f)
                {
                    __instance.FlipX = true;
                }
                else if (velocity.x > 0.01f)
                {
                    __instance.FlipX = false;
                }
                if (currentAnimation != __instance.CurrentAnimationGroup.RunAnim || flipX != __instance.FlipX)
                {
                    __instance.Animator.Play(__instance.CurrentAnimationGroup.RunAnim, 1f);
                    if (!Constants.ShouldHorseAround())
                    {
                        __instance.Animator.Time = 0.45833334f;
                    }
                    __instance.myPlayer.cosmetics.AnimateSkinRun();
                }
            }
            else if (currentAnimation == __instance.CurrentAnimationGroup.RunAnim || currentAnimation == __instance.CurrentAnimationGroup.SpawnAnim || !currentAnimation)
            {
                __instance.myPlayer.cosmetics.AnimateSkinIdle();
                __instance.Animator.Play(__instance.CurrentAnimationGroup.IdleAnim, 1f);
                __instance.myPlayer.SetHatAndVisorAlpha(1f);
            }
        }
        else
        {
            __instance.myPlayer.cosmetics.SetGhost();
            if (__instance.myPlayer.Data.Role.Role == RoleTypes.GuardianAngel)
            {
                if (currentAnimation != __instance.CurrentAnimationGroup.GhostGuardianAngelAnim)
                {
                    __instance.Animator.Play(__instance.CurrentAnimationGroup.GhostGuardianAngelAnim, 1f);
                    __instance.myPlayer.SetHatAndVisorAlpha(0.5f);
                }
            }
            else if (currentAnimation != __instance.CurrentAnimationGroup.GhostIdleAnim)
            {
                __instance.Animator.Play(__instance.CurrentAnimationGroup.GhostIdleAnim, 1f);
                __instance.myPlayer.SetHatAndVisorAlpha(0.5f);
            }
            if (velocity.x < -0.01f)
            {
                __instance.FlipX = true;
            }
            else if (velocity.x > 0.01f)
            {
                __instance.FlipX = false;
            }
        }

        return false;
    }

    [HarmonyPatch(typeof(SpriteAnim), nameof(SpriteAnim.Play))]
    [HarmonyPrefix]
    public static void PlayChangedClipPatch(SpriteAnim __instance, ref AnimationClip anim)
    {
        if (anim == null) return;

        Transform parent = __instance.transform.parent;
        if (!parent) return;

        PlayerManager playerManager = parent.GetPlayerManager();
        if (playerManager is not { AnimationController: { } }) return;

        anim = GetChangedClip(playerManager, anim);
    }

    [HarmonyPatch(typeof(WaitForAnimationFinish), nameof(WaitForAnimationFinish.MoveNext))]
    [HarmonyPrefix]
    public static void WaitForChangedClipFinishPatch(WaitForAnimationFinish __instance)
    {
        if (!__instance.first) return;

        if (__instance.clip == null || __instance.animator == null) return;

        Transform parent = __instance.animator.transform.parent;
        if (!parent) return;

        PlayerManager playerManager = parent.GetPlayerManager();
        if (playerManager is not { AnimationController: { } }) return;

        __instance.clip = GetChangedClip(playerManager, __instance.clip);
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
