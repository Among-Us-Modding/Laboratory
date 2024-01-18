using UnityEngine;

namespace Laboratory.Player.AnimationControllers;

public interface IAnimationController
{
    public PlayerAnimationGroup Group { get; set; }
    
    public bool HideHat { get; }
    public bool HideSkin { get; }
    public bool HideVisor { get; }
    public bool HidePet { get; }
    public bool HideName { get; }
    public bool IsPlayingCustomAnimation { get; }

    public Vector2 RendererOffset => Vector2.zero;
    public float ColliderYOffset => -0.3636f;

    public virtual float ZOffset => 0;

    public void Update()
    {
    }
}
