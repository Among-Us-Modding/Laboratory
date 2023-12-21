using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Il2CppInterop.Runtime.Attributes;
using Jungle.Effects;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Jungle.Buttons;

[RegisterInIl2Cpp]
public class EffectButton : CooldownButton
{
    public EffectButton(IntPtr ptr) : base(ptr) { }

    public IEffect Effect;
    public Coroutine EffectRoutine;
        
    public override void PerformClick()
    {
        if (!ShouldCooldown() || !ShouldBeVisible() || !CanUse()) return;

        CurrentTime = int.MaxValue;
        StopAllCoroutines();
        Effect = null;
        EffectRoutine = StartCoroutine(ShowEffectDuration().WrapToIl2Cpp());
        OnClickAction?.Invoke();
    }

    public override bool CanUse()
    {
        return EffectRoutine == null && base.CanUse();
    }

    [HideFromIl2Cpp]
    public IEnumerator ShowEffectDuration()
    {
        while (Effect == null)
        {
            CurrentTime = int.MaxValue;
            yield return null;
        }

        float duration = Effect.Timer;

        while (Effect is { Timer: > 0 })
        {
            CurrentTime = int.MaxValue;
            TimerText!.text = Mathf.CeilToInt(Effect.Timer).ToString();
            TimerText.color = Effect.Timer / duration < 0.5 
                ? Color.Lerp(new Color32(255, 0, 0, 255), new Color32(255, 242, 0, 255), (Effect.Timer / duration) * 2) 
                : Color.Lerp(new Color32(255, 242, 0, 255), new Color32(30, 150, 0, 255), ((Effect.Timer / duration) - 0.5f) * 2);

            yield return null;
        }

        CurrentTime = Cooldown;
        TimerText!.color = Color.white;
        EffectRoutine = null;
        Effect = null;
    }
}