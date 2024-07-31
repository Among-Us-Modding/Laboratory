using System.Collections;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils;
using Laboratory.Enums;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Laboratory.Utilities;

public static class CustomMurder
{
    [MethodRpc((uint)CustomRPCs.CustomMurder)]
    public static void RpcCustomMurder(this PlayerControl murderer, PlayerControl target, bool silent = false)
    {
        murderer.isKilling = false;
        NetworkedPlayerInfo data = target.Data;

		if (murderer.AmOwner)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(murderer.KillSfx, loop: false, 0.8f);
			}
			murderer.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown));
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
			if (!silent) DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(murderer.Data, data);
			target.cosmetics.SetNameMask(alive: false);
			target.RpcSetScanner(value: false);
		}

		murderer.MyPhysics.StartCoroutine(CoPerformKill(murderer.KillAnimations.Random(), murderer, target, silent));
    }

    private static IEnumerator CoPerformKill(KillAnimation killAnimation, PlayerControl source, PlayerControl target, bool silent)
    {
	    FollowerCamera cam = Camera.main!.GetComponent<FollowerCamera>();
	    bool isParticipant = PlayerControl.LocalPlayer == source || PlayerControl.LocalPlayer == target;
	    PlayerPhysics sourcePhys = source.MyPhysics;

	    if (!silent)
	    {
		    KillAnimation.SetMovement(source, canMove: false);
		    KillAnimation.SetMovement(target, canMove: false);
	    }

	    if (isParticipant)
	    {
		    PlayerControl.LocalPlayer.isKilling = true;
		    source.isKilling = true;
	    }
	    DeadBody deadBody = Object.Instantiate(GameManager.Instance.DeadBodyPrefab);
	    deadBody.enabled = false;
	    deadBody.ParentId = target.PlayerId;
	    foreach (var b in deadBody.bodyRenderers)
	    {
		    target.SetPlayerMaterialColors(b);
	    }
	    target.SetPlayerMaterialColors(deadBody.bloodSplatter);
	    Vector3 position = target.transform.position + killAnimation.BodyOffset;
	    position.z = position.y / 1000f;
	    deadBody.transform.position = position;
	    if (isParticipant)
	    {
		    cam.Locked = true;
		    ConsoleJoystick.SetMode_Task();
		    if (PlayerControl.LocalPlayer.AmOwner)
		    {
			    PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = true;
		    }
	    }
	    target.Die(DeathReason.Kill, assignGhostRole: true);

	    if (!silent)
	    {
		    yield return source.MyPhysics.Animations.CoPlayCustomAnimation(killAnimation.BlurAnim);
		    source.NetTransform.SnapTo(target.transform.position);
		    sourcePhys.Animations.PlayIdleAnimation();
		    KillAnimation.SetMovement(source, canMove: true);
		    KillAnimation.SetMovement(target, canMove: true);
	    }
	    deadBody.enabled = true;
	    if (isParticipant)
	    {
		    cam.Locked = false;
		    PlayerControl.LocalPlayer.isKilling = false;
		    source.isKilling = false;
	    }
    }
}
