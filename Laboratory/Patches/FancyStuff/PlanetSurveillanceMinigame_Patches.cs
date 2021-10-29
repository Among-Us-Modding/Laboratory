using HarmonyLib;

namespace Laboratory.Patches.FancyStuff
{
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Begin))]
    public static class PlanetSurveillanceMinigame_Begin_Patch
    {
        /// <summary>
        /// Forces render textures to be size of screen in minigame
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix()
        {
            RenderTexture_GetTemporary_Patch.Enabled = true;
        }

        /// <summary>
        /// Stops forcing render textures to be size of screen in minigame
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix()
        {
            RenderTexture_GetTemporary_Patch.Enabled = false;
        }
    }
}