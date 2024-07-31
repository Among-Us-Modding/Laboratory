using System;
using System.Collections.Generic;
using Laboratory.Extensions;
using Laboratory.Player.Managers;
using UnityEngine;

namespace Laboratory.Player.AnimationControllers;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DefaultController : IAnimationController
{
    public PlayerManager Owner { get; }
    public PlayerAnimations Animations => Owner.Physics.Animations;
    public PlayerPhysics Physics => Owner.Physics;

    public DefaultController(PlayerManager owner, MaterialType? materialType)
    {
        Owner = owner;

        PlayerControl player = owner.Player;
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

    private PlayerAnimationGroup _group;
    public PlayerAnimationGroup Group
    {
        get => _group;
        set
        {
            _group = value;

            var groups = new List<PlayerAnimationGroup>();
            foreach (var animationsAnimationGroup in Animations.animationGroups)
            {
                if (animationsAnimationGroup.BodyType != value.BodyType)
                {
                    groups.Add(animationsAnimationGroup);
                }
            }
            groups.Add(value);

            Animations.animationGroups = groups.ToIl2CppList();
            Physics.bodyType = value.BodyType;
            Physics.myPlayer.cosmetics.EnsureInitialized(value.BodyType);
            Animations.SetBodyType(value.BodyType, Physics.myPlayer.cosmetics.FlippedCosmeticOffset, Physics.myPlayer.cosmetics.NormalCosmeticOffset);
            Animations.PlayIdleAnimation();
        }
    }

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
