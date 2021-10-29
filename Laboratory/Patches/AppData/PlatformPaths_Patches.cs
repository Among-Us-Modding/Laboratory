using System.IO;
using HarmonyLib;

namespace Laboratory.Patches.AppData
{
    [HarmonyPatch(typeof(PlatformPaths), nameof(PlatformPaths.persistentDataPath), MethodType.Getter)]
    public static class PlatformPaths_get_persistentDataPath_Patch
    {
        /// <summary>
        /// Ensures the specified subfolder actually exists
        /// </summary>
        [HarmonyPrepare]
        public static void Prepare()
        {
            Directory.CreateDirectory(Path.Combine(PlatformPaths.persistentDataPath, LaboratoryPlugin.Instance.AppDataSubFolderName));
        }
        
        /// <summary>
        /// Changes the AppData folder to be a custom subfolder
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(ref string __result)
        {
            __result = Path.Combine(__result, LaboratoryPlugin.Instance.AppDataSubFolderName);
        }
    }
}