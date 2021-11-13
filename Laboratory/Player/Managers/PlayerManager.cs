using System;
using Laboratory.Player.AnimationControllers;
using Laboratory.Player.Attributes;
using Reactor;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Player.Managers;

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
    public IAnimationController? AnimationController { get; set; }

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        Physics = GetComponent<PlayerPhysics>();
        NetTransform = GetComponent<CustomNetworkTransform>();
    }

    public void Start()
    {
        if (Physics.body) Physics.EnableInterpolation();
        Physics.Skin.transform.Find("SpawnInGlow").gameObject.SetActive(false);

        AnimationController = new DefaultController(this);
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

        Player.nameText.enabled = !AnimationController.HideName;
        Physics.Skin.gameObject.SetActive(!AnimationController.HideCosmetics && Player.Visible);
        Player.HatRenderer.gameObject.SetActive(!AnimationController.HideCosmetics && !data.IsDead && Player.Visible);
        if (Player.CurrentPet) Player.CurrentPet.gameObject.SetActive(!AnimationController.HideCosmetics && Player.Visible);

        Player.Collider.offset = new Vector2(0, AnimationController.ColliderYOffset);
        Player.myRend.transform.localPosition = new Vector3(AnimationController.RendererOffset.x * (Player.myRend.flipX ? -1 : 1), AnimationController.RendererOffset.y, 0);
    }
}
