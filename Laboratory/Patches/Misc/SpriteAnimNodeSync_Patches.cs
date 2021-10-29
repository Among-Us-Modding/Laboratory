using System;
using HarmonyLib;
using PowerTools;

namespace Laboratory.Patches.Misc
{
    /// <summary>
    /// This should allow SpriteAnimNodeSync components to default to the first node if it is trying to reference a missing node
    /// </summary>
    [HarmonyPatch(typeof(SpriteAnimNodeSync), nameof(SpriteAnimNodeSync.LateUpdate))]
    public static class SpriteAnimNodeSync_LateUpdate_Patch
    {
        public static void Prefix(SpriteAnimNodeSync __instance, out int __state)
        {
            __state = -1;
            if (!__instance.Parent) return;
            if (__instance.NodeId != 1) return;
            if (__instance.Parent.m_node0 == default && Math.Abs(__instance.Parent.m_ang0) < 0.05) return;
            if (__instance.Parent.m_node1 != default || Math.Abs(__instance.Parent.m_ang1) > 0.05) return;
            
            __state = 1;
            __instance.NodeId = 0;
        }

        public static void Postfix(SpriteAnimNodeSync __instance, int __state)
        {
            if (__state < 0) return;
            __instance.NodeId = __state;
        }
    }
}