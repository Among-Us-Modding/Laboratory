using System.IO;
using HarmonyLib;
using Laboratory.Config;

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

    [HarmonyPatch(typeof(PlatformPaths), nameof(PlatformPaths.persistentDataPath), MethodType.Getter)]
    [HarmonyPrepare]
    public static void PreparePersistentDataPatch()
    {
        if (GameConfig.CustomSaveData) Directory.CreateDirectory(Path.Combine(PlatformPaths.persistentDataPath, LaboratoryPlugin.Instance.AppDataSubFolderName));
    }

    [HarmonyPatch(typeof(PlatformPaths), nameof(PlatformPaths.persistentDataPath), MethodType.Getter)]
    [HarmonyPostfix]
    public static void PersistentDataPatch(ref string __result)
    {
        if (GameConfig.CustomSaveData) __result = Path.Combine(__result, LaboratoryPlugin.Instance.AppDataSubFolderName);
    }

    [HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.GetPurchase))]
    [HarmonyPrefix]
    public static bool GetPurchasePatch(ref bool __result)
    {
        if (GameConfig.CustomSaveData)
        {
            __result = true;
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.SetPurchased))]
    [HarmonyPrefix]
    public static bool SetPurchasedPatch()
    {
        return !GameConfig.CustomSaveData;
    }
}
