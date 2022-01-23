using System;
using Laboratory.Player.Managers;
using PowerTools;
using UnityEngine;

namespace Laboratory.Player.AnimationControllers;

public class DefaultController : IAnimationController
{
    public PlayerManager Owner { get; }
    public PlayerPhysics Physics => Owner.Physics;

    public DefaultController(PlayerManager owner, MaterialType? materialType)
    {
        Owner = owner;
        var player = owner.Player;
        Anim = player.MyAnim;

        if (materialType != null)
        {
            player.MyRend.material = new Material(Shader.Find(materialType switch
            {
                MaterialType.Player => "Unlit/PlayerShader",
                MaterialType.Sprite => "Sprites/Default",
                _ => throw new ArgumentOutOfRangeException(nameof(materialType), materialType, null),
            }));

            if (materialType == MaterialType.Player)
            {
                player.SetPlayerMaterialColors(player.MyRend);
            }
        }
    }

    public virtual AnimationClip SpawnAnim => Physics.SpawnAnim;
    public virtual AnimationClip ClimbDownAnim => Physics.ClimbDownAnim;
    public virtual AnimationClip ClimbAnim => Physics.ClimbAnim;
    public virtual AnimationClip IdleAnim => Physics.IdleAnim;
    public virtual AnimationClip GhostIdleAnim => Physics.GhostIdleAnim;
    public virtual AnimationClip RunAnim => Physics.RunAnim;
    public virtual AnimationClip EnterVentAnim => Physics.EnterVentAnim;
    public virtual AnimationClip ExitVentAnim => Physics.ExitVentAnim;

    public SpriteAnim Anim { get; }
    public AnimationClip Current => Anim.Clip;

    public virtual bool HideHat => HideCosmetics;
    public virtual bool HideSkin => HideCosmetics;
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
