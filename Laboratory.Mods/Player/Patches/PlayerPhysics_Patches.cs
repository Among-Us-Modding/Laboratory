using System.Collections;
using System.Linq;
using HarmonyLib;
using Hazel;
using Laboratory.Mods.Extensions;
using Laboratory.Mods.Player.Interfaces;
using Laboratory.Mods.Player.MonoBehaviours;
using PowerTools;
using Reactor;
using UnityEngine;

namespace Laboratory.Mods.Player.Patches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.StartClimb))]
    public static class PlayerPhysics_StartClimb_Patch
    {
        public static bool Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] bool down)
        {
            IAnimationController? anim = __instance.GetPlayerComponent<PlayerManager>().AnimationController;
            if (anim == null) return true;
            __instance.rend.flipX = false;
            __instance.Skin.Flipped = false;
            __instance.Animator.Play(down ? anim.ClimbDownAnim : anim.ClimbAnim, 1f);
            __instance.Animator.Time = 0f;
            __instance.Skin.SetClimb(down);
            __instance.myPlayer.HatRenderer.SetClimbAnim();
            __instance.myPlayer.CurrentPet.Visible = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetSkin))]
    public static class PlayerPhysics_SetSkin_Patch
    {
        public static bool Prefix(PlayerPhysics __instance, uint skinId)
        {
            IAnimationController? anim = __instance.GetPlayerComponent<PlayerManager>().AnimationController;
            if (anim == null) return true;
            __instance.Skin.SetSkin(skinId, __instance.rend.flipX);
            if (__instance.Animator.IsPlaying(anim.SpawnAnim))
            {
                __instance.Skin.SetSpawn(__instance.rend.flipX, __instance.Animator.Time);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetAnimState))]
    public static class PlayerPhysics_ResetAnimState_Patch
    {
        public static bool Prefix(PlayerPhysics __instance)
        {
            IAnimationController? anim = __instance.GetPlayerComponent<PlayerManager>().AnimationController;
            if (anim == null) return true;

            __instance.myPlayer.FootSteps.Stop();
            __instance.myPlayer.FootSteps.loop = false;
            __instance.myPlayer.HatRenderer.SetIdleAnim();
            GameData.PlayerInfo data = __instance.myPlayer.Data;
            if (data != null)
            {
                __instance.myPlayer.HatRenderer.SetColor(__instance.myPlayer.Data.ColorId);
            }

            if (data == null || !data.IsDead)
            {
                __instance.Skin.SetIdle(__instance.rend.flipX);
                __instance.Animator.Play(anim.IdleAnim, 1f);
                __instance.myPlayer.Visible = true;
                __instance.myPlayer.SetHatAlpha(1f);
                return false;
            }

            __instance.Skin.SetGhost();
            __instance.Animator.Play(anim.GhostIdleAnim, 1f);
            __instance.myPlayer.SetHatAlpha(0.5f);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PCFI_Patch
    {
        public static bool[] States = new bool[150];

        public static bool Prefix(PlayerControl __instance)
        {
            var pid = __instance.PlayerId;
            return States[pid] = !States[pid];
        }
    }
    
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    public static class PlayerPhysics_HandleAnimation_Patch
    {
        public static bool[] States = new bool[150];
        
        public static bool Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] bool amDead)
        {
            var pid = __instance.myPlayer.PlayerId;
            var res = States[pid] = !States[pid];
            if (!res) return false;
            if (!GameData.Instance) return false;
            IAnimationController? anim = __instance.GetPlayerComponent<PlayerManager>().AnimationController;
            if (anim == null) return true;

            if (__instance.Animator.IsPlaying(anim.SpawnAnim)) return false;

            Vector2 velocity = __instance.body.velocity;
            AnimationClip currentAnimation = __instance.Animator.GetCurrentAnimation();
            if (anim.PlayingCustomAnimation(currentAnimation, __instance.Animator)) return false;
            
            if (!amDead)
            {
                if (velocity.sqrMagnitude >= 0.05f)
                {
                    bool flipX = __instance.rend.flipX;
                    if (velocity.x < -0.01f) __instance.rend.flipX = true;
                    else if (velocity.x > 0.01f) __instance.rend.flipX = false;
                    
                    if (currentAnimation != anim.RunAnim || flipX != __instance.rend.flipX)
                    {
                        __instance.Animator.Play(anim.RunAnim, 1f);
                        __instance.Animator.Time = 0.45833334f;
                        __instance.Skin.SetRun(__instance.rend.flipX);
                    }
                }
                else if (currentAnimation != anim.IdleAnim)
                {
                    __instance.Skin.SetIdle(__instance.rend.flipX);
                    __instance.Animator.Play(anim.IdleAnim, 1f);
                    __instance.myPlayer.SetHatAlpha(1f);
                }
            }
            else
            {
                __instance.Skin.SetGhost();
                if (currentAnimation != anim.GhostIdleAnim)
                {
                    __instance.Animator.Play(anim.GhostIdleAnim, 1f);
                    __instance.myPlayer.SetHatAlpha(0.5f);
                }

                if (velocity.x < -0.01f) __instance.rend.flipX = true;
                else if (velocity.x > 0.01f) __instance.rend.flipX = false;
            }

            __instance.Skin.Flipped = __instance.rend.flipX;
            return false;
        }
    }

    // [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
    public static class PlayerPhysics_LateUpdate_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerPhysics __instance)
        {
            var anim = __instance.GetPlayerComponent<PlayerManager>().AnimationController;
            if (anim == null) return;
            __instance.transform.position += new Vector3(0, 0, anim.ZOffset);
        }
    }
    
    #region Disabled Patches
    
    public static class PlayerPhysics_Coroutines
    {
        public static IEnumerator CoEnterVent(PlayerPhysics playerPhysics, int id)
        {
            IAnimationController? anim = playerPhysics.GetPlayerComponent<PlayerManager>().AnimationController;
            if (anim == null) yield break;
            Vent vent = ShipStatus.Instance.AllVents.First(v => v.Id == id);
            if (playerPhysics.myPlayer.AmOwner)
            {
                playerPhysics.inputHandler.enabled = true;
            }

            playerPhysics.myPlayer.moveable = false;
            yield return playerPhysics.WalkPlayerTo(vent.transform.position + vent.Offset, 0.01f, 1f);
            vent.EnterVent(playerPhysics.myPlayer);
            playerPhysics.Skin.SetEnterVent(playerPhysics.rend.flipX);
            yield return new WaitForAnimationFinish(playerPhysics.Animator, anim.EnterVentAnim);
            playerPhysics.Skin.SetIdle(playerPhysics.rend.flipX);
            playerPhysics.Animator.Play(anim.IdleAnim, 1f);
            playerPhysics.myPlayer.Visible = false;
            playerPhysics.myPlayer.inVent = true;
            if (playerPhysics.myPlayer.AmOwner)
            {
                VentilationSystem.Update(VentilationSystem.Operation.Enter, id);
                playerPhysics.inputHandler.enabled = false;
            }
        }

        public static IEnumerator CoExitVent(PlayerPhysics playerPhysics, int id)
        {
            IAnimationController? anim = playerPhysics.GetPlayerComponent<PlayerManager>().AnimationController;
            if (anim == null) yield break;
            Vent vent = ShipStatus.Instance.AllVents.First(v => v.Id == id);
            if (playerPhysics.myPlayer.AmOwner)
            {
                playerPhysics.inputHandler.enabled = true;
                VentilationSystem.Update(VentilationSystem.Operation.Exit, id);
            }

            playerPhysics.myPlayer.Visible = true;
            playerPhysics.myPlayer.myRend.enabled = true;
            playerPhysics.myPlayer.inVent = false;
            vent.ExitVent(playerPhysics.myPlayer);
            playerPhysics.myPlayer.Visible = true;
            playerPhysics.myPlayer.myRend.enabled = true;
            playerPhysics.Skin.SetExitVent(playerPhysics.rend.flipX);
            yield return new WaitForAnimationFinish(playerPhysics.Animator, anim.ExitVentAnim);
            playerPhysics.Skin.SetIdle(playerPhysics.rend.flipX);
            playerPhysics.Animator.Play(anim.IdleAnim, 1f);
            playerPhysics.myPlayer.moveable = true;
            playerPhysics.myPlayer.Visible = true;
            playerPhysics.myPlayer.myRend.enabled = true;
            if (playerPhysics.myPlayer.AmOwner)
            {
                playerPhysics.inputHandler.enabled = false;
            }
        }
    }

    // [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleRpc))]
    public static class PlayerPhysics_HandleRpc_Patch
    {
        public static bool Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch ((RpcCalls) callId)
            {
                case RpcCalls.EnterVent:
                {
                    int id = reader.ReadPackedInt32();
                    __instance.StopAllCoroutines();
                    __instance.StartCoroutine(PlayerPhysics_Coroutines.CoEnterVent(__instance, id));
                    return false;
                }
                case RpcCalls.ExitVent:
                {
                    int id = reader.ReadPackedInt32();
                    __instance.StopAllCoroutines();
                    __instance.StartCoroutine(PlayerPhysics_Coroutines.CoEnterVent(__instance, id));
                    return false;
                }
            }

            return true;
        }
    }

    // [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
    public static class PlayerPhysics_RpcEnterVent_Patch
    {
        public static bool Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] int id)
        {
            if (AmongUsClient.Instance.AmClient)
            {
                __instance.StopAllCoroutines();
                __instance.StartCoroutine(PlayerPhysics_Coroutines.CoEnterVent(__instance, id));
            }

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(__instance.NetId, (byte) RpcCalls.EnterVent, SendOption.Reliable);
            messageWriter.WritePacked(id);
            messageWriter.EndMessage();
            return false;
        }
    }

    // [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcExitVent))]
    public static class PlayerPhysics_RpcExitVent_Patch
    {
        public static bool Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] int id)
        {
            if (AmongUsClient.Instance.AmClient)
            {
                __instance.StopAllCoroutines();
                __instance.StartCoroutine(PlayerPhysics_Coroutines.CoExitVent(__instance, id));
            }

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(__instance.NetId, (byte) RpcCalls.ExitVent, SendOption.Reliable);
            messageWriter.WritePacked(id);
            messageWriter.EndMessage();
            return false;
        }
    }

    // [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.BootFromVent))]
    public static class PlayerPhysics_BootFromVent_Patch
    {
        public static bool Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] int ventId)
        {
            if (AmongUsClient.Instance.AmClient)
            {
                __instance.BootFromVent(ventId);
            }

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(__instance.NetId, (byte) RpcCalls.BootFromVent, SendOption.Reliable);
            messageWriter.WritePacked(ventId);
            messageWriter.EndMessage();
            return false;
        }
    }


    // [HarmonyPatch(typeof(PlayerPhysics._CoSpawnPlayer_d__41), nameof(PlayerPhysics._CoSpawnPlayer_d__41.MoveNext))]
    public static class PlayerPhysics_CoSpawnPlayer_MoveNext_Patch
    {
        public static bool Prefix(PlayerPhysics._CoSpawnPlayer_d__41 __instance, ref bool __result)
        {
            int num = __instance.__1__state;
            if (num != 1) return true;
            PlayerPhysics playerPhysics = __instance.__4__this;
            var anim = playerPhysics.GetPlayerComponent<PlayerManager>().AnimationController;
            if (anim == null) return true;
            __instance.__1__state = -1;
            __instance._amFlipped_5__4 = (__instance._spawnSeatId_5__2 > 4);
            playerPhysics.myPlayer.MyRend.flipX = __instance._amFlipped_5__4;
            playerPhysics.myPlayer.transform.position = __instance._spawnPos_5__3;
            SoundManager.Instance.PlaySound(__instance.lobby.SpawnSound, false, 1f).volume = 0.75f;
            playerPhysics.Skin.SetSpawn(playerPhysics.rend.flipX, 0f);
            playerPhysics.Skin.Flipped = __instance._amFlipped_5__4;
            playerPhysics.GlowAnimator.GetComponent<SpriteRenderer>().flipX = playerPhysics.rend.flipX;
            playerPhysics.GlowAnimator.Play(playerPhysics.SpawnGlowAnim, 1f);
            __instance.__2__current = new WaitForAnimationFinish(playerPhysics.Animator, anim.SpawnAnim);
            __instance.__1__state = 2;
            __result = true;
            return false;
        }
    }
    
    #endregion
}