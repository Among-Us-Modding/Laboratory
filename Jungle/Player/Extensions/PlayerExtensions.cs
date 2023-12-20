using Jungle.Effects.Managers;
using Jungle.Extensions;
using Jungle.Player.AnimationControllers;
using Jungle.Player.Managers;
using UnityEngine;

namespace Jungle.Player.Extensions;

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
