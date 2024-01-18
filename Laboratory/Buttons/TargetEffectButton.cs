using System;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Laboratory.Buttons;

[RegisterInIl2Cpp]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TargetEffectButton : EffectButton
{
    public TargetEffectButton(IntPtr ptr) : base(ptr) { }
        
    public PlayerControl Target { get; set; }
        
    public virtual float TargetRange() => GameManager.Instance.LogicOptions.GetKillDistance();

    public virtual PlayerControl GetClosest()
    {
        PlayerControl current = null;
        float maxDistance = TargetRange();
        Vector2 myPosition = PlayerControl.LocalPlayer.GetTruePosition();
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (player.AmOwner || player.Data.IsDead) continue;
            float distance = Vector2.Distance(myPosition, player.GetTruePosition());
            if (distance <= maxDistance)
            {
                current = player;
                maxDistance = distance;
            }
        }

        return current;
    }
        
    public override void PerformClick()
    {
        if (!Target) return;
        base.PerformClick();
    }
        
    public override bool CanUse()
    {
        if (!Target) return false;
        return base.CanUse();
    }
        
    public override void Update()
    {
        base.Update();
        if (CurrentTime != int.MaxValue)
        {
            Target = GetClosest();
            SetButtonSaturation(Target);
        }
    }
}