using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using Laboratory.Enums;
using PowerTools;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Laboratory.Utilities;

public static class CustomMurder
{
    [MethodRpc((uint)CustomRPCs.CustomMurder)]
    public static void RpcCustomMurder(this PlayerControl murderer, PlayerControl target, bool silent = false, bool leaveBody = true)
    {
        if (AmongUsClient.Instance.IsGameOver)
        {
            return;
        }

        GameData.PlayerInfo data = target.Data;
        if (data == null || data.IsDead || data.Disconnected)
        {
            return;
        }

        if (murderer.AmOwner)
        {
            if (Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.PlaySound(murderer.KillSfx, false, 0.8f);
            }

            murderer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
        }

        target.gameObject.layer = LayerMask.NameToLayer("Ghost");
        if (target.AmOwner)
        {
            if ((bool)Minigame.Instance)
            {
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                    // ignored
                }
            }

            if (!silent)
            {
                DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(murderer.Data, data);
            }

            DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
            target.cosmetics.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
            target.RpcSetScanner(false);
            ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
            importantTextTask.transform.SetParent(target.transform, false);
            if (!PlayerControl.GameOptions.GhostsDoTasks)
            {
                target.ClearTasks();
                importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostIgnoreTasks);
            }
            else
            {
                importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostDoTasks);
            }

            target.myTasks.Insert(0, importantTextTask);
        }

        murderer.MyPhysics.StartCoroutine(CoPerformKill(murderer.KillAnimations.Random()!, murderer, target, silent, leaveBody));
    }

    private static IEnumerator CoPerformKill(KillAnimation killAnimation, PlayerControl murderer, PlayerControl target, bool silent, bool leaveBody)
    {
        FollowerCamera cam = Camera.main!.GetComponent<FollowerCamera>();
        bool isParticipant = PlayerControl.LocalPlayer == murderer || PlayerControl.LocalPlayer == target;

        if (!silent)
        {
            KillAnimation.SetMovement(murderer, false);
            KillAnimation.SetMovement(target, false);
        }

        DeadBody deadBody = null;
        if (leaveBody)
        {
            deadBody = Object.Instantiate(killAnimation.bodyPrefab);
            deadBody.enabled = false;
            deadBody.ParentId = target.PlayerId;
            target.SetPlayerMaterialColors(deadBody.bodyRenderer);
            target.SetPlayerMaterialColors(deadBody.bloodSplatter);
            Vector3 position = target.transform.position + killAnimation.BodyOffset;
            position.z = position.y / 1000f;
            deadBody.transform.position = position;
        }

        if (isParticipant)
        {
            cam.Locked = true;
            ConsoleJoystick.SetMode_Task();
            PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = true;
        }

        target.Die(DeathReason.Kill);

        if (!silent)
        {
            SpriteAnim sourceAnim = murderer.MyAnim;
            yield return new WaitForAnimationFinish(sourceAnim, killAnimation.BlurAnim);
            murderer.NetTransform.SnapTo(target.transform.position);
            sourceAnim.Play(murderer.MyPhysics.CurrentAnimationGroup.IdleAnim);

            KillAnimation.SetMovement(murderer, true);
            KillAnimation.SetMovement(target, true);
        }

        if (leaveBody)
        {
            deadBody.enabled = true;
        }
        
        if (isParticipant)
        {
            cam.Locked = false;
        }
    }
}
