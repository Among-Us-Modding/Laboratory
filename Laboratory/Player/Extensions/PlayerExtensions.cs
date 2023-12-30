using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using Laboratory.Effects.Managers;
using Laboratory.Extensions;
using Laboratory.Player.Managers;
using UnityEngine;

namespace Laboratory.Player.Extensions;

public static class PlayerExtensions
{
    public static PlayerManager GetPlayerManager(this GameObject player)
    {
        return player.GetCachedComponent<PlayerManager>();
    }

    public static PlayerManager GetPlayerManager(this Component player) => player.gameObject.GetPlayerManager();

    public static PlayerEffectManager GetEffectManager(this GameObject player)
    {
        return player.GetCachedComponent<PlayerEffectManager>();
    }

    public static PlayerEffectManager GetEffectManager(this Component player) => player.gameObject.GetEffectManager();

    public static Coroutine AnimateCustom(this PlayerControl player, AnimationClip clip)
    {
        return player.StartCoroutine(player.CoAnimateCustom(clip));
    }

    private static IEnumerator CoAnimateCustom(this PlayerControl player, AnimationClip clip)
    {
        player.MyPhysics.DoingCustomAnimation = true;

        yield return player.MyPhysics.Animations.CoPlayCustomAnimation(clip);
        player.cosmetics.AnimateSkinIdle();
        player.MyPhysics.Animations.PlayIdleAnimation();
        player.cosmetics.SetBodyCosmeticsVisible(b: true);
        player.MyPhysics.DoingCustomAnimation = false;
    }
}
