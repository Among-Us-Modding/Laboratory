using Laboratory.Mods.Player.MonoBehaviours;
using PowerTools;
using UnityEngine;

namespace Laboratory.Mods.Player.Interfaces
{
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

        public float RendOffset => 0;
        public float ColliderOffset => -0.3636f;
        
        public virtual float ZOffset => 0;

        public bool PlayingCustomAnimation(AnimationClip animationClip, SpriteAnim anim);
        public void Initialize(PlayerManager owner);
        
        public void Update() { }
    }
}