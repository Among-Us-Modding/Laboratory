using Laboratory.Mods.Player.MonoBehaviours;
using UnityEngine;

namespace Laboratory.Mods.Extensions
{
    public static class PlayerExtensions
    {
        public static T GetPlayerComponent<T>(this Component component) where T : PlayerManager
        {
            // TODO Implement this properly
            return component.GetComponent<T>();
        }
    }
}