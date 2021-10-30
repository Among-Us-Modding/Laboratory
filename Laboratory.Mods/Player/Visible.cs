using System.Collections.Generic;
using Reactor;
using UnityEngine;

namespace Laboratory.Mods.Player
{
    public static class Visible
    {
        public static Dictionary<int, List<string>> Visibles = new();
        public static Dictionary<int, bool> SetVisibles = new();

        public static bool AnyVisibles(PlayerControl player)
        {
            int playerHashCode = player.GetHashCode();
            return !SetVisibles[playerHashCode] || Visibles[playerHashCode].Count > 0;
        }

        public static void AddVisible(PlayerControl player, string name)
        {
            Visibles.TryGetValue(player.GetHashCode(), out List<string> strs);
            if (strs != null && strs.Contains(name)) return;
            strs?.Add(name);
            UpdateVisible(player);
        }

        public static void RemoveVisible(PlayerControl player, string name)
        {
            Visibles[player.GetHashCode()].RemoveAll(s => s == name);
            UpdateVisible(player);
        }

        public static void SetVisible(PlayerControl player, bool value)
        {
            SetVisibles[player.GetHashCode()] = value;
        }

        public static void UpdateVisible(PlayerControl player)
        {
            bool shouldBeVisible = !AnyVisibles(player);
            player.myRend.enabled = shouldBeVisible;
            player.MyPhysics.Skin.Visible = shouldBeVisible;
            player.HatRenderer.gameObject.SetActive(shouldBeVisible);
            if (player.CurrentPet)
            {
                player.CurrentPet.Visible = shouldBeVisible;
            }
            player.nameText.gameObject.SetActive(shouldBeVisible);
        }
    }
}