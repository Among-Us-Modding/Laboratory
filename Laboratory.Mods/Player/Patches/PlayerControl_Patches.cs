using System;
using HarmonyLib;
using Laboratory.Mods.Player.Attributes;
using UnhollowerRuntimeLib;

namespace Laboratory.Mods.Player.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    public static class PlayerControl_Awake_Patch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance.notRealPlayer) return;
            foreach (Type playerComponentType in PlayerComponentAttribute.PlayerComponentTypes)
            {
                Il2CppSystem.Type il2cppType = Il2CppType.From(playerComponentType);
                if (!__instance.GetComponent(il2cppType)) __instance.gameObject.AddComponent(il2cppType);
            }
        }
    }
}