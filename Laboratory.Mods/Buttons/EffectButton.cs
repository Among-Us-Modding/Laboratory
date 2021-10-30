using System;
using System.Collections;
using Laboratory.Mods.Effects.Interfaces;
using Reactor;
using UnityEngine;

namespace Laboratory.Mods.Buttons
{
    public class EffectButton : CooldownButton
    {
        public IEffect Effect { get; set; }
        public IEnumerator EffectRoutine { get; set; }
        
        public override void PerformClick()
        {
            if (!ShouldCooldown() || !ShouldBeVisible() || CantUse()) return;

            CurrentTime = int.MaxValue;
            if (EffectRoutine is not null) Coroutines.Stop(EffectRoutine);
            EffectRoutine = Coroutines.Start(ShowEffectDuration());
            OnClickAction?.Invoke();
        }

        public override void Update()
        {
            KillButtonManager.renderer.enabled = true;
            GameObject.SetActive(ShouldBeVisible());

            if (Math.Abs(CurrentTime - int.MaxValue) < 1)
            {
                KillButtonManager.TimerText.gameObject.SetActive(true);
                KillButtonManager.renderer.color = Palette.DisabledClear;
                KillButtonManager.renderer.material.SetFloat("_Desat", 1f);
                return;
            }
            
            if (!Unlocked())
            {
                SetButtonSaturation(false);
                SetButtonCooldownLevel(1);
                KillButtonManager.TimerText.gameObject.SetActive(false);
                return;
            }
            
            CurrentTime -= Time.deltaTime;

            SetButtonCooldownLevel(Cooldown == 0 ? 0 : Mathf.Clamp01(CurrentTime / Cooldown));
            SetButtonSaturation(true);
            
            if (CurrentTime <= 0)
            {
                CurrentTime = 0;
                KillButtonManager.TimerText.gameObject.SetActive(false);
                return;
            }
            KillButtonManager.TimerText.text = Mathf.CeilToInt(CurrentTime).ToString();
            KillButtonManager.TimerText.gameObject.SetActive(true);
        }
        
        public virtual IEnumerator ShowEffectDuration()
        {
            float duration = 0;
            while (Effect == null && !GameObject.WasCollected)
            {
                CurrentTime = int.MaxValue;
                yield return null;
            }
            if (GameObject.WasCollected) yield break;
            
            duration = Effect.Timer;

            while (Effect is { Timer: > 0 })
            {
                if (GameObject.WasCollected) yield break;
                
                CurrentTime = int.MaxValue;
                KillButtonManager.TimerText.text = Mathf.CeilToInt(Effect.Timer).ToString();
                Color lerpedColor;
                if (Effect.Timer / duration < 0.5) lerpedColor = Color.Lerp(new Color32(255, 0, 0, 255), new Color32(255, 242, 0, 255), (Effect.Timer / duration) * 2);
                else lerpedColor = Color.Lerp(new Color32(255, 242, 0, 255), new Color32(30, 150, 0, 255), ((Effect.Timer / duration) - 0.5f) * 2);

                KillButtonManager.TimerText.color = lerpedColor;
                yield return null;
            }

            if (GameObject.WasCollected) yield break;
            CurrentTime = Cooldown;
            KillButtonManager.TimerText.color = Color.white;
            EffectRoutine = null;
            Effect = null;
        }
    }
}