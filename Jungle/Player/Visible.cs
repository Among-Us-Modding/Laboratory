using System.Collections.Generic;
using HarmonyLib;
using Reactor.Utilities;

namespace Jungle.Player;

/// <summary>
/// Race condition-free invisible players
/// </summary>
// [HarmonyPatch]
public static class Visible
{
    private static readonly Dictionary<PlayerControl, HashSet<object>> _invisible = new(Il2CppEqualityComparer<PlayerControl>.Instance);

    private static void SetVisible(this PlayerControl player, bool value, object key)
    {
        var set = _invisible[player];
        var changed = value ? set.Remove(key) : set.Add(key);

        if (changed) UpdateVisible(player);
    }

    public static void SetVisible<T>(this PlayerControl player, bool value)
    {
        player.SetVisible(value, typeof(T));
    }

    private static bool IsVisible(this PlayerControl player, object key)
    {
        return !_invisible[player].Contains(key);
    }

    public static bool IsVisible<T>(this PlayerControl player)
    {
        return player.IsVisible(typeof(T));
    }

    private static void UpdateVisible(PlayerControl player)
    {
        player.Visible = player.Visible;
    }

    internal static void Clear(PlayerControl player)
    {
        _invisible[player].Clear();
        UpdateVisible(player);
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    [HarmonyPostfix]
    public static void VisibleOnAwakePatch(PlayerControl __instance)
    {
        if (__instance.notRealPlayer) return;
        _invisible[__instance] = new HashSet<object>();
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool GetVisiblePatch(PlayerControl __instance, out bool __result)
    {
        __result = _invisible[__instance].Count <= 0;
        return false;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    [HarmonyPrefix]
    public static bool SetVisiblePatch(PlayerControl __instance, [HarmonyArgument(0)] bool value)
    {
        __instance.SetVisible(value, null);
        UpdateVisible(__instance);
        return false;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    [HarmonyPostfix]
    public static void RemoveOnDestroyPatch(PlayerControl __instance)
    {
        _invisible.Remove(__instance);
    }
}
