using HarmonyLib;
using Jungle.Effects.Managers;
using Jungle.Enums;
using Jungle.HUDMap;
using Jungle.Player;
using Jungle.Player.Extensions;
using Jungle.Utils;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Jungle.Debugging;

[HarmonyPatch]
public static class DebugKeybinds
{
    [MethodRpc((uint)CustomRPCs.HardReset)]
    private static void HardReset(PlayerControl player)
    {
        if (player.AmOwner)
        {
            HudManager.Instance.FullScreen.enabled = false;
            CameraZoomController.Instance!.OrthographicSize = 3;
            HudManager.Instance.PlayerCam.Target = player;
            HudManager.Instance.PlayerCam.Locked = false;
            Transform camTransform = Camera.main!.transform;
            for (int i = 5; i < camTransform.childCount; i++) camTransform.GetChild(i).gameObject.Destroy();
        }

        PlayerEffectManager playerEffectManager = player.GetEffectManager();
        playerEffectManager.ClearEffects();
        Moveable.Clear(player);
        Visible.Clear(player);
        SpeedModifier.Clear(player.MyPhysics);
        SizeModifer.Clear(player.MyPhysics);
        player.moveable = true;
        player.Visible = true;
        player.Collider.enabled = true;
        player.transform.localEulerAngles = Vector3.zero;
        HudManager.Instance.SetHudActive(true);
    }

    [MethodRpc((uint)CustomRPCs.ResetMovement)]
    private static void ResetMovement(PlayerControl player)
    {
        player.Collider.enabled = true;
        Moveable.Clear(player);
        SpeedModifier.Clear(player.MyPhysics);
        player.moveable = true;
        player.Visible = true;
        player.NetTransform.enabled = true;
    }

    [MethodRpc((uint)CustomRPCs.EndGame)]
    private static void RpcEndGame(PlayerControl _)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            GameManager.Instance.RpcEndGame(GameOverReason.HumansByTask, false);
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static void WatchForKeyboardInputPatch()
    {
        if (Input.GetKey(KeyCode.F11) && Input.GetKeyDown(KeyCode.F10) ||
            Input.GetKey(KeyCode.F10) && Input.GetKeyDown(KeyCode.F11))
        {
            RpcEndGame(PlayerControl.LocalPlayer);
        }

        if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.F2) ||
            Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.F1))
        {
            HardReset(PlayerControl.LocalPlayer);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerControl.LocalPlayer.Collider.enabled = !PlayerControl.LocalPlayer.Collider.enabled;
        }

        if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.F4) ||
            Input.GetKey(KeyCode.F4) && Input.GetKeyDown(KeyCode.F3))
        {
            ResetMovement(PlayerControl.LocalPlayer);
        }

        // Teleport
        if (Input.GetKey(KeyCode.F5) && Input.GetKeyDown(KeyCode.F6) ||
            Input.GetKey(KeyCode.F6) && Input.GetKeyDown(KeyCode.F5))
        {
            void MouseUpEvent(CustomMapBehaviour instance, int mouseButton, Vector2 worldPos)
            {
                if (mouseButton != 1) return;
                instance.Parent!.Close();
                PlayerControl.LocalPlayer.moveable = true;
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(worldPos);
            }
            CustomMapBehaviour.ShowWithAllPlayers(new Color32(158, 240, 103, 255), MouseUpEvent);
        }
    }
}