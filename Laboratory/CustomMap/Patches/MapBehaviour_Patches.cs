using HarmonyLib;
using Laboratory.Extensions;

namespace Laboratory.CustomMap.Patches;

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Awake))]
public static class MapBehaviour_Awake_Patch
{
    public static void Postfix(MapBehaviour __instance)
    {
        __instance.gameObject.EnsureComponent<CustomMapBehaviour>();
    }
}