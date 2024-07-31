using System;
using HarmonyLib;
using Laboratory.Effects.Managers;
using Laboratory.Enums;
using Laboratory.HUDMap;
using Laboratory.Player;
using Laboratory.Player.AnimationControllers;
using Laboratory.Player.Extensions;
using Laboratory.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Laboratory.Patches;

[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public static class KeyboardJoystick_Update_Patch
{
    [MethodRpc((uint)CustomRPCs.ForceNoEffect)]
    public static void NoEffect(PlayerControl player)
    {
        try
        {
            GlobalEffectManager.Instance.PrimaryEffect = null;
        }
        catch
        {
            // ignored
        }

        GlobalEffectManager.Instance._primaryEffect = null;
        foreach (var allPlayerControl in PlayerControl.AllPlayerControls)
        {
            var effectManager = allPlayerControl.GetEffectManager();
            try
            {
                effectManager.PrimaryEffect = null;
            }
            catch
            {
                // ignored
            }

            effectManager._primaryEffect = null;
        }
    }

    [MethodRpc((uint)CustomRPCs.HardReset)]
    public static void HardReset(PlayerControl player)
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
        SizeModifer.Clear(player.MyPhysics, o =>
        {
            if (o is not Type t) return false;
            return t == typeof(IAnimationController);
        });
        player.moveable = true;
        player.Visible = true;
        player.Collider.enabled = true;
        player.transform.localEulerAngles = Vector3.zero;
        HudManager.Instance.SetHudActive(true);

        player.MyPhysics.DoingCustomAnimation = false;
    }

    [MethodRpc((uint)CustomRPCs.ResetMovement)]
    public static void ResetMovement(PlayerControl player)
    {
        player.Collider.enabled = true;
        Moveable.Clear(player);
        SpeedModifier.Clear(player.MyPhysics);
        player.moveable = true;
        player.Visible = true;
        player.NetTransform.enabled = true;
    }

    [MethodRpc((uint)CustomRPCs.EndGame)]
    public static void RpcEndGame(PlayerControl player)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            GameManager.Instance.RpcEndGame(GameOverReason.HumansByTask, false);
        }
    }

    public static void Postfix(KeyboardJoystick __instance)
    {
        if (Input.GetKey(KeyCode.F11) && Input.GetKeyDown(KeyCode.F10) || Input.GetKey(KeyCode.F10) && Input.GetKeyDown(KeyCode.F11))
        {
            RpcEndGame(PlayerControl.LocalPlayer);
        }

        if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.F3) || Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.F1))
        {
            HardReset(PlayerControl.LocalPlayer);
        }

        if (Input.GetKey(KeyCode.F2) && Input.GetKeyDown(KeyCode.F4) || Input.GetKey(KeyCode.F4) && Input.GetKeyDown(KeyCode.F2))
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
            void MouseUpEvent(CustomMapBehaviour instance, int mousebutton, Vector2 worldposition)
            {
                if (mousebutton != 1) return;
                instance.MouseUpEvent -= MouseUpEvent;
                instance.Parent!.Close();
                PlayerControl.LocalPlayer.moveable = true;
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(worldposition);
            }
            CustomMapBehaviour.ShowWithAllPlayers(new Color32(158, 240, 103, 255), MouseUpEvent);
        }
    }
}
