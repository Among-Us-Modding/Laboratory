using System;
using Laboratory.Player.Managers;
using PowerTools;
using UnityEngine;

namespace Laboratory.Player.AnimationControllers;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DefaultController : IAnimationController
{
    public PlayerManager Owner { get; }
    public PlayerPhysics Physics => Owner.Physics;

    public DefaultController(PlayerManager owner, MaterialType? materialType)
    {
        Owner = owner;
        PlayerControl player = owner.Player;
        Anim = player.MyAnim;

        if (materialType != null)
        {
            player.cosmetics.currentBodySprite.BodySprite.material = new Material(Shader.Find(materialType switch
            {
                MaterialType.Player => "Unlit/PlayerShader",
                MaterialType.Sprite => "Sprites/Default",
                _ => throw new ArgumentOutOfRangeException(nameof(materialType), materialType, null),
            }));

            if (materialType == MaterialType.Player)
            {
                player.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
            }
        }
    }

    public virtual AnimationClip SpawnAnim => Physics.CurrentAnimationGroup.SpawnAnim;
    public virtual AnimationClip ClimbDownAnim => Physics.CurrentAnimationGroup.ClimbDownAnim;
    public virtual AnimationClip ClimbAnim => Physics.CurrentAnimationGroup.ClimbAnim;
    public virtual AnimationClip IdleAnim => Physics.CurrentAnimationGroup.IdleAnim;
    public virtual AnimationClip GhostIdleAnim => Physics.CurrentAnimationGroup.GhostIdleAnim;
    public virtual AnimationClip RunAnim => Physics.CurrentAnimationGroup.RunAnim;
    public virtual AnimationClip EnterVentAnim => Physics.CurrentAnimationGroup.EnterVentAnim;
    public virtual AnimationClip ExitVentAnim => Physics.CurrentAnimationGroup.ExitVentAnim;

    public SpriteAnim Anim { get; }
    public AnimationClip Current => Anim.Clip;

    public virtual bool HideHat => HideCosmetics;
    public virtual bool HideSkin => HideCosmetics;
    public virtual bool HideVisor => HideCosmetics;
    public virtual bool HidePet => HideCosmetics;
    public virtual bool HideCosmetics => false;
    public virtual bool HideName => false;
    public virtual bool IsPlayingCustomAnimation => false;

    public virtual Vector2 RendererOffset => Vector2.zero;
    public virtual float ColliderYOffset => -0.3636f;
    public virtual float ZOffset => 0;

    public virtual void Update()
    {
    }

    public enum MaterialType
    {
        Player,
        Sprite,
    }
}
