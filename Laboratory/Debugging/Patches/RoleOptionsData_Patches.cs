using HarmonyLib;

namespace Laboratory.Debugging.Patches;

[HarmonyPatch]
public static class DisableOtherRoles
{
    [HarmonyPatch(typeof(RoleOptionsData), nameof(RoleOptionsData.GetNumPerGame))]
    [HarmonyPrefix]
    public static bool GetNumPerGame(RoleOptionsData __instance, out int __result)
    {
        __result = 0;
        return false;
    }
}