using System;
using Jungle.Buttons;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Jungle;

[RegisterInIl2Cpp]
public class ClimbButton : CooldownButton
{
    public ClimbButton(IntPtr ptr) : base(ptr)
    {
    }

    public void Start()
    {
        SetSprite("Climb", 300);
        Cooldown = 5f;
        OnClickAction += () =>
        {
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position + Vector3.up * 2f);
        };
    }
    public override bool Unlocked()
    {
        return true;
    }
    
    public override bool ShouldBeVisible() => base.ShouldBeVisible() && Unlocked();
}