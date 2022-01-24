using System;
using HarmonyLib;
using Laboratory.Extensions;
using Laboratory.Player.Managers;
using PowerTools;
using UnityEngine;

namespace Laboratory.Player.AnimationControllers;

[HarmonyPatch]
internal static class AnimationPatches
{
    public static AnimationClip GetChangedClip(PlayerManager playerManager, AnimationClip original)
    {
        var animationController = playerManager.AnimationController;
        if (animationController == null) return original;

        var playerPhysics = playerManager.Physics;
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

        var animationController = playerManager.AnimationController;
        if (animationController == null) return changed;

        var playerPhysics = playerManager.Physics;
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

        var parent = __instance.transform.parent;
        if (!parent) return;

        var playerManager = parent.GetPlayerManager();
        if (playerManager is not { AnimationController: { } }) return;
        
        // This ensures that when a custom animation finishes it will allow the normal animation cycle to continue
        var actuallyPlaying = __instance.Clip is {isLooping: true} || __instance.IsPlaying();
        __result = actuallyPlaying && playerManager.AnimationController.IsPlayingCustomAnimation ? playerManager.Physics.ClimbAnim : GetOriginalClip(playerManager, __result);
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    [HarmonyPrefix]
    public static bool HandleAnimationReimplemented(PlayerPhysics __instance, [HarmonyArgument(0)] bool amDead)
    {
        if (__instance.Animator.IsPlaying(__instance.SpawnAnim) || !GameData.Instance)
        {
            return false;
        }

        Vector2 velocity = __instance.body.velocity;
        AnimationClip currentAnimation = __instance.Animator.GetCurrentAnimation();
        // PATCH START
        // Here we image that we are running GetCurrentAnimationPatch as a postifx patch for GetCurrentAnimation
        GetCurrentAnimationPatch(__instance.Animator, ref currentAnimation);
        // PATCH END
        if (currentAnimation == __instance.ClimbAnim || currentAnimation == __instance.ClimbDownAnim)
        {
            return false;
        }

        if (!amDead)
        {
            if (velocity.sqrMagnitude >= 0.05f)
            {
                bool flipX = __instance.rend.flipX;
                if (velocity.x < -0.01f)
                {
                    __instance.rend.flipX = true;
                }
                else if (velocity.x > 0.01f)
                {
                    __instance.rend.flipX = false;
                }

                if (currentAnimation != __instance.RunAnim || flipX != __instance.rend.flipX)
                {
                    __instance.Animator.Play(__instance.RunAnim);
                    __instance.Animator.Time = 11f / 24f;
                    __instance.Skin.SetRun(__instance.rend.flipX);
                }
            }
            else if (currentAnimation == __instance.RunAnim || currentAnimation == __instance.SpawnAnim || !currentAnimation)
            {
                __instance.Skin.SetIdle(__instance.rend.flipX);
                __instance.Animator.Play(__instance.IdleAnim);
                __instance.myPlayer.SetHatAlpha(1f);
            }
        }
        else
        {
            __instance.Skin.SetGhost();
            if (currentAnimation != __instance.GhostIdleAnim)
            {
                __instance.Animator.Play(__instance.GhostIdleAnim);
                __instance.myPlayer.SetHatAlpha(0.5f);
            }

            if (velocity.x < -0.01f)
            {
                __instance.rend.flipX = true;
            }
            else if (velocity.x > 0.01f)
            {
                __instance.rend.flipX = false;
            }
        }

        __instance.Skin.Flipped = __instance.rend.flipX;

        return false;
    }

    [HarmonyPatch(typeof(SpriteAnim), nameof(SpriteAnim.Play))]
    [HarmonyPrefix]
    public static void PlayChangedClipPatch(SpriteAnim __instance, ref AnimationClip? anim)
    {
        if (anim == null) return;

        var parent = __instance.transform.parent;
        if (!parent) return;

        var playerManager = parent.GetPlayerManager();
        if (playerManager is not { AnimationController: { } }) return;

        anim = GetChangedClip(playerManager, anim);
    }

    [HarmonyPatch(typeof(WaitForAnimationFinish), nameof(WaitForAnimationFinish.MoveNext))]
    [HarmonyPrefix]
    public static void WaitForChangedClipFinishPatch(WaitForAnimationFinish __instance)
    {
        if (!__instance.first) return;

        if (__instance.clip == null || __instance.animator == null) return;

        var parent = __instance.animator.transform.parent;
        if (!parent) return;

        var playerManager = parent.GetPlayerManager();
        if (playerManager is not { AnimationController: { } }) return;

        __instance.clip = GetChangedClip(playerManager, __instance.clip);
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
    [HarmonyPrefix]
    public static bool AdjustOffsets(PlayerPhysics __instance)
    {
        var anim = __instance.myPlayer.GetPlayerManager().AnimationController;
        if (anim == null) return true;

        var transform = __instance.transform;
        var position = transform.position;
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
