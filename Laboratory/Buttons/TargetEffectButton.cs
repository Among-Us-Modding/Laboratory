using System;
using Reactor;
using UnityEngine;

namespace Laboratory.Buttons;

[RegisterInIl2Cpp]
public class TargetEffectButton : EffectButton
{
    public TargetEffectButton(IntPtr ptr) : base(ptr) { }
        
    public PlayerControl? Target { get; set; }
        
    public virtual float TargetRange() => GameOptionsData.KillDistances[Mathf.Clamp(PlayerControl.GameOptions.KillDistance, 0, 2)];

    public virtual PlayerControl? GetClosest()
    {
        PlayerControl? current = null;
        var maxDistance = TargetRange();
        var myPosition = PlayerControl.LocalPlayer.GetTruePosition();
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.AmOwner || player.Data.IsDead) continue;
            var distance = Vector2.Distance(myPosition, player.GetTruePosition());
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