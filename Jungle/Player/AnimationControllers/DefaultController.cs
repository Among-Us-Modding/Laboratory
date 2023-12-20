using System;
using Jungle.Player.Managers;
using PowerTools;
using UnityEngine;

namespace Jungle.Player.AnimationControllers;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DefaultController : IAnimationController
{
    public PlayerManager Owner { get; }
    public PlayerPhysics Physics => Owner.Physics;

    public DefaultController(PlayerManager owner, MaterialType? materialType)
    {
        Owner = owner;
        var player = owner.Player;
        Anim = player.MyPhysics.Animations.Animator;

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

    public virtual AnimationClip SpawnAnim => Physics.Animations.group.SpawnAnim;
    public virtual AnimationClip ClimbDownAnim => Physics.Animations.group.ClimbDownAnim;
    public virtual AnimationClip ClimbAnim => Physics.Animations.group.ClimbUpAnim;
    public virtual AnimationClip IdleAnim => Physics.Animations.group.IdleAnim;
    public virtual AnimationClip GhostIdleAnim => Physics.Animations.group.GhostIdleAnim;
    public virtual AnimationClip RunAnim => Physics.Animations.group.RunAnim;
    public virtual AnimationClip EnterVentAnim => Physics.Animations.group.EnterVentAnim;
    public virtual AnimationClip ExitVentAnim => Physics.Animations.group.ExitVentAnim;

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
