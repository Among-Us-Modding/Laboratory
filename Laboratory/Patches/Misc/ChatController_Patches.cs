using HarmonyLib;

namespace Laboratory.Patches.Misc;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatController_Update_Patch
{
    /// <summary>
    /// Allows sending messages at any rate
    /// </summary>
    [HarmonyPrefix]
    public static void Prefix(ChatController __instance)
    {
        __instance.TimeSinceLastMessage = 5;
    }
}