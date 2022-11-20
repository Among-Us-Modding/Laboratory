using System.IO;
using HarmonyLib;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class SaveDataPatches
{
    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
    [HarmonyPostfix]
    public static void AlwaysNotBannedPatch(out bool __result)
    {
        __result = false;
    }
    
    /*[HarmonyPatch(typeof(PlatformPaths), nameof(PlatformPaths.persistentDataPath), MethodType.Getter)]
    [HarmonyPrepare]
    public static void PreparePersistentDataPatch()
    {
        Directory.CreateDirectory(Path.Combine(PlatformPaths.persistentDataPath, LaboratoryPlugin.Instance.AppDataSubFolderName));
    }
        
    [HarmonyPatch(typeof(PlatformPaths), nameof(PlatformPaths.persistentDataPath), MethodType.Getter)]
    [HarmonyPostfix]
    public static void PersistentDataPatch(ref string __result)
    {
        __result = Path.Combine(__result, LaboratoryPlugin.Instance.AppDataSubFolderName);
    }
    
    [HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.GetPurchase))]
    [HarmonyPrefix]
    public static bool GetPurchasePatch(out bool __result)
    {
        __result = true;
        return false;
    }
        
    [HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.SetPurchased))]
    [HarmonyPrefix]
    public static bool SetPurchasedPatch()
    {
        return false;
    }*/
}