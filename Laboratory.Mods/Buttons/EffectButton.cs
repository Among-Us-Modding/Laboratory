using System;
using System.Collections;
using Laboratory.Mods.Effects.Interfaces;
using Reactor;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Mods.Buttons
{
    [RegisterInIl2Cpp]
    public class EffectButton : CooldownButton
    {
        public EffectButton(IntPtr ptr) : base(ptr) { }

        public IEffect? Effect;
        public Coroutine? EffectRoutine;
        
        public override void PerformClick()
        {
            if (!ShouldCooldown() || !ShouldBeVisible() || !CanUse()) return;

            CurrentTime = int.MaxValue;
            StopAllCoroutines();
            
            EffectRoutine = StartCoroutine(new CoroutineWrapper(ShowEffectDuration()));
            OnClickAction?.Invoke();
        }
        
        [HideFromIl2Cpp]
        public IEnumerator ShowEffectDuration()
        {
            while (Effect == null)
            {
                CurrentTime = int.MaxValue;
                yield return null;
            }

            var duration = Effect.Timer;

            while (Effect is { Timer: > 0 })
            {
                CurrentTime = int.MaxValue;
                TimerText!.text = Mathf.CeilToInt(Effect.Timer).ToString();
                Color lerpedColor;
                if (Effect.Timer / duration < 0.5) lerpedColor = Color.Lerp(new Color32(255, 0, 0, 255), new Color32(255, 242, 0, 255), (Effect.Timer / duration) * 2);
                else lerpedColor = Color.Lerp(new Color32(255, 242, 0, 255), new Color32(30, 150, 0, 255), ((Effect.Timer / duration) - 0.5f) * 2);

                TimerText.color = lerpedColor;
                yield return null;
            }

            CurrentTime = Cooldown;
            TimerText!.color = Color.white;
            EffectRoutine = null;
            Effect = null;
        }
    }
}