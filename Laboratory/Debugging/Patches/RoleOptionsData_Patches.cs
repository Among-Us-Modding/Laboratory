using System.Linq;
using HarmonyLib;
using Laboratory.Patches;

namespace Laboratory.Debugging.Patches;

[HarmonyPatch]
public static class DisableOtherRoles
{
    [HarmonyPatch(typeof(RoleOptionsData), nameof(RoleOptionsData.GetNumPerGame))]
    [HarmonyPrefix]
    public static bool GetNumPerGame(RoleOptionsData __instance, RoleTypes role, ref int __result)
    {
        if (!new[] {RoleTypes.Scientist, RoleTypes.Engineer, RoleTypes.GuardianAngel, RoleTypes.Shapeshifter}.Contains(role)) return true;
        __result = 0;
        return false;
    }
}