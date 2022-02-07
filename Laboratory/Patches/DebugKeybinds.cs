// TODO cleanup

using System;
using HarmonyLib;
using Laboratory.CustomMap;
using Laboratory.Effects.Utils;
using Laboratory.Enums;
using Laboratory.Extensions;
using Laboratory.Player;
using Reactor.Extensions;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public static class KeyboardJoystick_Update_Patch
{
    [MethodRpc((uint)CustomRpcs.HardReset)]
    public static void HardReset(PlayerControl player)
    {
        if (player.AmOwner)
        {
            HudManager.Instance.FullScreen.enabled = false;
            CameraZoomController.Instance!.OrthographicSize = 3;
            HudManager.Instance.PlayerCam.Target = player;
            HudManager.Instance.PlayerCam.Locked = false;
            var camTransform = Camera.main!.transform;
            for (var i = 5; i < camTransform.childCount; i++) camTransform.GetChild(i).gameObject.Destroy();
        }

        var playerEffectManager = player.GetEffectManager();
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

    [MethodRpc((uint)CustomRpcs.ResetMovement)]
    public static void ResetMovement(PlayerControl player)
    {
        player.Collider.enabled = true;
        Moveable.Clear(player);
        SpeedModifier.Clear(player.MyPhysics);
        player.moveable = true;
        player.Visible = true;
        player.NetTransform.enabled = true;
    }

    [MethodRpc((uint)CustomRpcs.EndGame)]
    public static void RpcEndGame(PlayerControl player)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            ShipStatus.RpcEndGame(GameOverReason.HumansByTask, false);
        }
    }

    public static void Postfix(KeyboardJoystick __instance)
    {
        if (Input.GetKey(KeyCode.F11) && Input.GetKeyDown(KeyCode.F10) || Input.GetKey(KeyCode.F10) && Input.GetKeyDown(KeyCode.F11))
        {
            RpcEndGame(PlayerControl.LocalPlayer);
        }

        if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.F2) || Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.F1))
        {
            HardReset(PlayerControl.LocalPlayer);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerControl.LocalPlayer.Collider.enabled = !PlayerControl.LocalPlayer.Collider.enabled;
        }

        if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.F4) || Input.GetKey(KeyCode.F4) && Input.GetKeyDown(KeyCode.F3))
        {
            ResetMovement(PlayerControl.LocalPlayer);
        }


        // Teleport
        if (Input.GetKey(KeyCode.F5) && Input.GetKeyDown(KeyCode.F6) || Input.GetKey(KeyCode.F6) && Input.GetKeyDown(KeyCode.F5))
        {
            HudManager.Instance.ShowMap((Action<MapBehaviour>)(map =>
            {
                if (map.IsOpen)
                {
                    map.Close();
                    return;
                }

                if (!PlayerControl.LocalPlayer.CanMove) return;

                map.countOverlay.gameObject.SetActive(false);
                map.infectedOverlay.gameObject.SetActive(false);
                map.taskOverlay.Hide();
                map.GenericShow();
                PlayerControl.LocalPlayer.SetPlayerMaterialColors(map.HerePoint);
                map.ColorControl.SetColor(new Color32(158, 240, 103, 255));
                DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
                var customMapBehaviour = map.gameObject.GetComponent<CustomMapBehaviour>();
                customMapBehaviour.ShowAllPlayers();
                customMapBehaviour.MouseUpEvent += MouseUpEvent;
            }));

            void MouseUpEvent(CustomMapBehaviour instance, int mousebutton, Vector2 worldposition)
            {
                if (mousebutton == 1)
                {
                    instance.MouseUpEvent -= MouseUpEvent;
                    instance.Parent!.Close();
                    PlayerControl.LocalPlayer.moveable = true;
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(worldposition);
                }
            }
        }
    }
}
