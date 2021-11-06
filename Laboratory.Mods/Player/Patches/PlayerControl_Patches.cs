using HarmonyLib;
using Laboratory.Mods.Player.Attributes;
using Laboratory.Mods.Systems;
using UnhollowerRuntimeLib;

namespace Laboratory.Mods.Player.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    public static class PlayerControl_Awake_Patch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance.notRealPlayer) return;
            foreach (var playerComponentType in PlayerComponentAttribute.PlayerComponentTypes)
            {
                var il2cppType = Il2CppType.From(playerComponentType);
                if (!__instance.GetComponent(il2cppType)) __instance.gameObject.AddComponent(il2cppType);
            }
            
            Moveable.CanMoveables[__instance.GetHashCode()] = new();
            Visible.Visibles[__instance.GetHashCode()] = new();
            Visible.SetVisibles[__instance.GetHashCode()] = true;
        }
    }
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
    public static class PlayerControl_Get_CanMove_Patch
    {
        public static void Postfix(PlayerControl __instance, ref bool __result)
        {
            if (Moveable.AnyMovables(__instance)) __result = false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Getter)]
    public static class PlayerControl_Get_Visible_Patch
    {
        public static void Postfix(PlayerControl __instance, ref bool __result)
        {
            __result = !Visible.AnyVisibles(__instance);
        }
    }
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    public static class PlayerControl_Set_Visible_Patch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] bool value)
        {
            Visible.SetVisible(__instance, value);
            Visible.UpdateVisible(__instance);
            return false;
        }
    }
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    public static class PlayerControl_OnDestroy_Patch
    {
        public static void Postfix(PlayerControl __instance)
        {
            Moveable.CanMoveables.Remove(__instance.GetHashCode());
            Visible.Visibles.Remove(__instance.GetHashCode());
            Visible.SetVisibles.Remove(__instance.GetHashCode());
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
    public static class PlayerControl_SetInfected_Patch
    {
        public static void Postfix()
        {
            var system = HealthSystem.Instance;
            if (system == null) return;
            foreach (var playerInfo in PlayerControl.AllPlayerControls)
            {
                system.SetHealth(playerInfo.PlayerId, HealthSystem.MaxHealth);                    
            }
        }
    }
}