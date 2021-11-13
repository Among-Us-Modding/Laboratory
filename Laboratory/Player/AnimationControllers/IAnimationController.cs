using PowerTools;
using UnityEngine;

namespace Laboratory.Player.AnimationControllers;

public interface IAnimationController
{
    public AnimationClip SpawnAnim { get; }
    public AnimationClip ClimbDownAnim { get; }
    public AnimationClip ClimbAnim { get; }
    public AnimationClip IdleAnim { get; }
    public AnimationClip GhostIdleAnim { get; }
    public AnimationClip RunAnim { get; }
    public AnimationClip EnterVentAnim { get; }
    public AnimationClip ExitVentAnim { get; }

    public bool HideCosmetics { get; }
    public bool HideName { get; }

    public Vector2 RendererOffset => Vector2.zero;
    public float ColliderYOffset => -0.3636f;

    public virtual float ZOffset => 0;

    public bool IsPlayingCustomAnimation(AnimationClip animationClip, SpriteAnim anim);

    public void Update()
    {
    }
}
