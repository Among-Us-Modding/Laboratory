using HarmonyLib;
using Laboratory.Mods.Player.MonoBehaviours;
using Reactor.Extensions;

namespace Laboratory.Mods.Systems.Patches
{
    // [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.Update))]
    public static class PlayerManager_Patches
    {
        public static void Postfix(PlayerManager __instance)
        {
            if (!ShipStatus.Instance) return;
            GameData.PlayerInfo ownerData = __instance.MyPlayer.Data;

            if (ownerData is not {IsDead: false, IsImpostor: false}) return;

            HealthSystem healthSystem = HealthSystem.Instance;
            if (healthSystem is null) return;
            
            int playerHealth = healthSystem.GetHealth(__instance.MyPlayer.PlayerId);
            // if (playerHealth == 0 && __instance.MyPlayer.AmOwner) CustomMurders.RpcSilentMurder(__instance.MyPlayer);

            __instance.MyPlayer.nameText.text = $"<color=#{Palette.PlayerColors[(ownerData.ColorId + Palette.PlayerColors.Length) % Palette.PlayerColors.Length].ToHtmlStringRGBA()}>{playerHealth}</color>\n{ownerData.PlayerName}";
        }
    }
}