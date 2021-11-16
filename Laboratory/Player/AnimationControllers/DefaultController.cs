using Laboratory.Player.Managers;
using PowerTools;
using UnityEngine;

namespace Laboratory.Player.AnimationControllers;

public class DefaultController : IAnimationController
{
    public PlayerManager Owner { get; }
    public PlayerPhysics Physics => Owner.Physics;

    public DefaultController(PlayerManager owner)
    {
        Owner = owner;
    }

    public virtual AnimationClip SpawnAnim => Physics.SpawnAnim;
    public virtual AnimationClip ClimbDownAnim => Physics.ClimbDownAnim;
    public virtual AnimationClip ClimbAnim => Physics.ClimbAnim;
    public virtual AnimationClip IdleAnim => Physics.IdleAnim;
    public virtual AnimationClip GhostIdleAnim => Physics.GhostIdleAnim;
    public virtual AnimationClip RunAnim => Physics.RunAnim;
    public virtual AnimationClip EnterVentAnim => Physics.EnterVentAnim;
    public virtual AnimationClip ExitVentAnim => Physics.ExitVentAnim;

    public virtual bool HideCosmetics => false;
    public virtual bool HideName => false;
    
    public virtual Vector2 RendererOffset => Vector2.zero;
    public virtual float ColliderYOffset => -0.3636f;
    public virtual float ZOffset => 0;
    
    public virtual bool IsPlayingCustomAnimation(AnimationClip animationClip, SpriteAnim anim)
    {
        return false;
    }

    public virtual void Update()
    {
    }
}
