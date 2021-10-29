using HarmonyLib;
using UnityEngine;

namespace Laboratory.Patches.NoBan
{
    [HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.deviceUniqueIdentifier), MethodType.Getter)]
    public static class SystemInfo_get_deviceUniqueIdentifier_Patch
    {
        /// <summary>
        /// Sets random unique id, not sure if actually needed
        /// </summary>
        /// <param name="__result"></param>
        [HarmonyPostfix]
        public static void Postfix(out string __result)
        {
            __result = $"Crewmate{Random.Range(0, uint.MaxValue)}";
        }
    }
}