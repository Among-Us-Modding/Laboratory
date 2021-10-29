using HarmonyLib;

namespace Laboratory.Patches.UI
{
    [HarmonyPatch(typeof(ActionMapGlyphDisplay), nameof(ActionMapGlyphDisplay.Awake))]
    public static class ActionMapGlyphDisplay_Awake_Patch
    {
        /// <summary>
        /// Hides controller glyphs
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        public static void Postfix(ActionMapGlyphDisplay __instance)
        {
            __instance.sr.gameObject.SetActive(false);
        }
    }
}