using HarmonyLib;
using Laboratory.Extensions;
using PowerTools;
using UnityEngine;

namespace Laboratory.Player.AnimationControllers;

[HarmonyPatch(typeof(SpriteAnim), nameof(SpriteAnim.Play))]
internal static class AnimationSwapPatch
{
    public static void Prefix(SpriteAnim __instance, ref AnimationClip? anim)
    {
        var parent = __instance.transform.parent;
        if (!parent) return;

        var playerManager = parent.GetPlayerManager();
        if (!playerManager) return;

        var animationController = playerManager.AnimationController;
        if (animationController == null) return;

        var playerPhysics = playerManager.Physics;
        if (playerPhysics == null) return;

        if (__instance != playerPhysics.Animator) return;

        if (anim == playerPhysics.SpawnAnim)
        {
            anim = animationController.SpawnAnim;
        }
        else if (anim == playerPhysics.ClimbDownAnim)
        {
            anim = animationController.ClimbDownAnim;
        }
        else if (anim == playerPhysics.ClimbAnim)
        {
            anim = animationController.ClimbAnim;
        }
        else if (anim == playerPhysics.IdleAnim)
        {
            anim = animationController.IdleAnim;
        }
        else if (anim == playerPhysics.GhostIdleAnim)
        {
            anim = animationController.GhostIdleAnim;
        }
        else if (anim == playerPhysics.RunAnim)
        {
            anim = animationController.RunAnim;
        }
        else if (anim == playerPhysics.EnterVentAnim)
        {
            anim = animationController.EnterVentAnim;
        }
        else if (anim == playerPhysics.ExitVentAnim)
        {
            anim = animationController.ExitVentAnim;
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
internal static class HandleAnimationPatch
{
    public static bool Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] bool amDead)
    {
        var anim = __instance.GetPlayerManager().AnimationController;
        if (anim == null) return true;

        if (__instance.Animator.IsPlaying(anim.SpawnAnim)) return false;

        var velocity = __instance.body.velocity;
        var currentAnimation = __instance.Animator.GetCurrentAnimation();
        if (anim.IsPlayingCustomAnimation(currentAnimation, __instance.Animator)) return false;

        if (!amDead)
        {
            if (velocity.sqrMagnitude >= 0.05f)
            {
                var flipX = __instance.rend.flipX;
                if (velocity.x < -0.01f) __instance.rend.flipX = true;
                else if (velocity.x > 0.01f) __instance.rend.flipX = false;

                if (currentAnimation != anim.RunAnim || flipX != __instance.rend.flipX)
                {
                    __instance.Animator.Play(anim.RunAnim, 1f);
                    __instance.Animator.Time = 0.45833334f;
                    __instance.Skin.SetRun(__instance.rend.flipX);
                }
            }
            else if (currentAnimation != anim.IdleAnim)
            {
                __instance.Skin.SetIdle(__instance.rend.flipX);
                __instance.Animator.Play(anim.IdleAnim, 1f);
                __instance.myPlayer.SetHatAlpha(1f);
            }
        }
        else
        {
            __instance.Skin.SetGhost();
            if (currentAnimation != anim.GhostIdleAnim)
            {
                __instance.Animator.Play(anim.GhostIdleAnim, 1f);
                __instance.myPlayer.SetHatAlpha(0.5f);
            }

            if (velocity.x < -0.01f) __instance.rend.flipX = true;
            else if (velocity.x > 0.01f) __instance.rend.flipX = false;
        }

        __instance.Skin.Flipped = __instance.rend.flipX;
        return false;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
internal static class ZOffsetPatch
{
    [HarmonyPostfix]
    public static bool Prefix(PlayerPhysics __instance)
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
