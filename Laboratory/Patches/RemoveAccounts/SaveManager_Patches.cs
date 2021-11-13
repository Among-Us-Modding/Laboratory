using HarmonyLib;
using InnerNet;

namespace Laboratory.Patches.RemoveAccounts;

[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.BirthDateYear), MethodType.Getter)]
public static class SaveManager_get_BirthDateYear_Patch
{
    /// <summary>
    /// Disables asking for date of birth
    /// </summary>
    [HarmonyPrefix]
    public static bool Prefix(out int __result)
    {
        __result = 1990;
        return false;
    }
}
    
[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.ChatModeType), MethodType.Getter)]
public static class SaveManager_get_ChatModeType_Patch
{
    /// <summary>
    /// Always allows any kind of chat type
    /// </summary>
    [HarmonyPrefix]
    public static bool Prefix(out QuickChatModes __result)
    {
        __result = QuickChatModes.FreeChatOrQuickChat;
        return false;
    }
}