using Laboratory.Extensions;
using Laboratory.Player.AnimationControllers;
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

    public static IAnimationController GetAnimationController(this GameObject player)
    {
        return player.GetPlayerManager().AnimationController;
    }

    public static IAnimationController GetAnimationController(this Component player) => player.gameObject.GetAnimationController();
}
