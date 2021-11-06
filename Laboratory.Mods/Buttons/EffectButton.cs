using System;
using System.Collections;
using BepInEx.IL2CPP.Utils.Collections;
using Laboratory.Mods.Effects.Interfaces;
using Reactor;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Mods.Buttons
{
    [RegisterInIl2Cpp]
    public abstract class EffectButton : CooldownButton
    {
        public EffectButton(IntPtr ptr) : base(ptr) { }

        public IEffect? Effect;
        public Coroutine? EffectRoutine;
        
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