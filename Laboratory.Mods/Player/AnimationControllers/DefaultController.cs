using Laboratory.Mods.Player.Interfaces;
using Laboratory.Mods.Player.MonoBehaviours;
using PowerTools;
using UnityEngine;

namespace Laboratory.Mods.Player.AnimationControllers
{
    public class DefaultController : IAnimationController
    {
        private PlayerPhysics MyPhysics { get; set; }

        public virtual AnimationClip SpawnAnim => MyPhysics.SpawnAnim;
        public virtual AnimationClip ClimbDownAnim => MyPhysics.ClimbDownAnim;
        public virtual AnimationClip ClimbAnim => MyPhysics.ClimbAnim;
        public virtual AnimationClip IdleAnim => MyPhysics.IdleAnim;
        public virtual AnimationClip GhostIdleAnim => MyPhysics.GhostIdleAnim;
        public virtual AnimationClip RunAnim => MyPhysics.RunAnim;
        public virtual AnimationClip EnterVentAnim => MyPhysics.EnterVentAnim;
        public virtual AnimationClip ExitVentAnim => MyPhysics.ExitVentAnim;
        
        public virtual bool HideCosmetics => false;
        public virtual bool HideName => false;
        
        public virtual bool PlayingCustomAnimation(AnimationClip animationClip, SpriteAnim anim)
        {
            return false;
        }

        public virtual void Initialize(PlayerManager owner)
        {
            MyPhysics = owner.MyPhysics;
        }
    }
}