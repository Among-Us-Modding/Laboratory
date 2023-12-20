using System;
using Il2CppInterop.Runtime.Attributes;
using Jungle.Player.AnimationControllers;
using Jungle.Player.Attributes;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Jungle.Player.Managers;

[RegisterInIl2Cpp, PlayerComponent]
public class PlayerManager : MonoBehaviour
{
    public PlayerManager(IntPtr ptr) : base(ptr)
    {
    }

    public PlayerControl Player { get; private set; } = null!;
    public PlayerPhysics Physics { get; private set; } = null!;
    public CustomNetworkTransform NetTransform { get; private set; } = null!;

    public bool AmOwner => Player.AmOwner;

    [HideFromIl2Cpp]
    public IAnimationController AnimationController { get; set; }

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        Physics = GetComponent<PlayerPhysics>();
        NetTransform = GetComponent<CustomNetworkTransform>();
    }

    public void Start()
    {
        if (Physics.body) Physics.EnableInterpolation();
        Player.cosmetics.skin.transform.Find("SpawnInGlow").gameObject.SetActive(false);

        AnimationController = new DefaultController(this, DefaultController.MaterialType.Player);
    }

    public void Update()
    {
        AnimationController?.Update();
    }

    public void LateUpdate()
    {
        if (AnimationController == null) return;

        var data = Player.Data;
        if (data == null) return;

        Player.cosmetics.nameText.enabled = !AnimationController.HideName;
        Player.cosmetics.skin.gameObject.SetActive(!AnimationController.HideSkin && Player.Visible);
        Player.cosmetics.hat.gameObject.SetActive(!AnimationController.HideHat && !data.IsDead && Player.Visible);
        Player.cosmetics.visor.gameObject.SetActive(!AnimationController.HideVisor && !data.IsDead && Player.Visible);
        if (Player.cosmetics.currentPet) Player.cosmetics.currentPet.gameObject.SetActive(!AnimationController.HidePet && Player.Visible);

        Player.Collider.offset = new Vector2(0, AnimationController.ColliderYOffset);
        Player.cosmetics.currentBodySprite.BodySprite.transform.localPosition = new Vector3(AnimationController.RendererOffset.x * (Player.cosmetics.FlipX ? -1 : 1), AnimationController.RendererOffset.y, 0);
    }
}
