using System.Collections.Generic;
using HarmonyLib;
using Reactor;

namespace Laboratory.Player;

/// <summary>
/// Race condition-free invisible players
/// </summary>
public static class Visible
{
    private static readonly Dictionary<PlayerControl, HashSet<object?>> _invisible = new(Il2CppEqualityComparer<PlayerControl>.Instance);

    public static void SetVisible(this PlayerControl player, bool value, object? key)
    {
        HashSet<object?>? set = _invisible[player];
        bool changed = value ? set.Remove(key) : set.Add(key);

        if (changed)
        {
            UpdateVisible(player);
        }
    }

    public static void SetVisible<T>(this PlayerControl player, bool value)
    {
        player.SetVisible(value, typeof(T));
    }

    public static bool IsVisible(this PlayerControl player, object? key)
    {
        return !_invisible[player].Contains(key);
    }

    public static bool IsVisible<T>(this PlayerControl player)
    {
        return player.IsVisible(typeof(T));
    }

    public static void UpdateVisible(PlayerControl player)
    {
        bool isVisible = player.Visible;

        player.myRend.enabled = isVisible;
        player.MyPhysics.Skin.Visible = isVisible;
        player.HatRenderer.gameObject.SetActive(isVisible);
        if (player.CurrentPet)
        {
            player.CurrentPet.Visible = isVisible;
        }

        player.nameText.gameObject.SetActive(isVisible);
    }

    internal static void Clear(PlayerControl player)
    {
        _invisible[player].Clear();
        UpdateVisible(player);
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    private static class AwakePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance.notRealPlayer) return;
            _invisible[__instance] = new HashSet<object?>();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Getter)]
    private static class GetVisiblePatch
    {
        public static bool Prefix(PlayerControl __instance, out bool __result)
        {
            __result = _invisible[__instance].Count <= 0;
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    private static class SetVisiblePatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] bool value)
        {
            __instance.SetVisible(value, null);
            UpdateVisible(__instance);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    public static class OnDestroyPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            _invisible.Remove(__instance);
        }
    }
}
