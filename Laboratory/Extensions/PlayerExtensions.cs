using Laboratory.Effects.Managers;
using Laboratory.Player.Managers;
using UnityEngine;

namespace Laboratory.Extensions;

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
}
