using System;
using System.Collections.Generic;
using System.Linq;
using Laboratory.Mods.Extensions;
using Laboratory.Mods.Player.MonoBehaviours;
using Laboratory.Utils;
using Reactor;
using Reactor.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Laboratory.Mods.Buttons
{
    [RegisterInIl2Cpp]
    public class CooldownButton : MonoBehaviour
    {
        #region Cached Shader Stuff

        private static readonly int Percent = Shader.PropertyToID("_Percent");
        private static readonly int Desat = Shader.PropertyToID("_Desat");

        #endregion
        
        public static float XDistFromEdge = 1.45f;
        public static float YDistFromEdge = 1.4f;
        
        public static List<CooldownButton> Buttons { get; } = new();

        public static T Create<T>() where T : CooldownButton
        {
            var buttonObj = new GameObject(typeof(T).Name);
            var button = buttonObj.AddComponent<T>();

            button.Renderer = buttonObj.AddComponent<SpriteRenderer>();
            button.Renderer.material = new Material(HudManager.Instance.KillButton.renderer.material);

            button.Aspect = buttonObj.AddComponent<AspectPosition>();
            button.Aspect.parentCam = HudManager.Instance.UICamera;

            button.SetPosition(AspectPosition.EdgeAlignments.LeftBottom);
            return button;
        }
        
        public CooldownButton(IntPtr ptr) : base(ptr) { }

        public SpriteRenderer Renderer;
        public AspectPosition Aspect;
        public TextMeshPro TimerText;

        public float CurrentTime;
        public float Cooldown;

        public Action OnClickAction;
        
        #region Appearance

        public void SetPosition(AspectPosition.EdgeAlignments alignment, int buttonIndex = Int32.MaxValue)
        {
            var myHashCode = GetHashCode();
            if (buttonIndex == Int32.MaxValue) buttonIndex = Buttons.Count(c => c.GetHashCode() != myHashCode && c.Aspect.Alignment == alignment);
            
            Vector3 distanceFromEdge = new(0.7f, 0.7f, -5f);
            
            distanceFromEdge.x += XDistFromEdge * Mathf.FloorToInt(buttonIndex / 3f);
            distanceFromEdge.y += YDistFromEdge * (buttonIndex % 3);
            distanceFromEdge.z = -5f;
            
            Aspect.Alignment = alignment;
            Aspect.DistanceFromEdge = distanceFromEdge;
            Aspect.AdjustPosition();
        }

        public void SetSprite(string spriteName, float ppu = 100) => SetSprite(AssetManager.LoadSprite(spriteName, ppu));

        public void SetSprite(Sprite sprite)
        {
            Renderer.sprite = sprite;
            // ReSharper disable once InvokeAsExtensionMethod
            CooldownHelpers.SetCooldownNormalizedUvs(Renderer);
        }

        public void SetButtonCooldownLevel(float amount)
        {
            Renderer.material.SetFloat(Percent, amount);
        }

        public void SetButtonSaturation(bool saturated)
        {
            Renderer.material.SetFloat(Desat, saturated ? 0 : 1);
            Renderer.color = saturated ? Palette.EnabledColor : Palette.DisabledClear;
        }
        
        #endregion

        #region Usability

        public virtual bool Unlocked() => true;
        
        public virtual bool ShouldBeVisible() => HudManager.Instance.UseButton.gameObject.active && 
                                                 PlayerControl.LocalPlayer != null && 
                                                 PlayerControl.LocalPlayer.Data is {IsDead: false} && 
                                                 ShipStatus.Instance != null;
        public virtual bool ShouldCooldown() => PlayerControl.LocalPlayer.CanMove;

        public virtual bool CantUse() => CurrentTime > 0 || 
                                         !Unlocked()||
                                         PlayerControl.LocalPlayer.inVent || 
                                         !ShipStatus.Instance || 
                                         MeetingHud.Instance || 
                                         Minigame.Instance || 
                                         !isActiveAndEnabled;

        public void InvokePerformClick()
        {
            PerformClick();
        }
        
        public virtual void PerformClick()
        {
            if (!ShouldCooldown() || !ShouldBeVisible() || CantUse()) return;

            CurrentTime = Cooldown;
            OnClickAction?.Invoke();
        }

        #endregion

        #region Event Loop
        
        public void Awake()
        {
            Buttons.Add(this);
        }

        public void Update()
        {
            Renderer.enabled = TimerText.enabled = ShouldBeVisible();
            if (MeetingHud.Instance || ExileController.Instance) CurrentTime = Cooldown;
            
            if (!Unlocked())
            {
                SetButtonSaturation(false);
                SetButtonCooldownLevel(1);
                TimerText.enabled = false;
                return;
            }
            
            CurrentTime -= Time.deltaTime;

            SetButtonCooldownLevel(Cooldown == 0 ? 0 : Mathf.Clamp01(CurrentTime / Cooldown));
            SetButtonSaturation(true);
            
            if (CurrentTime <= 0)
            {
                CurrentTime = 0;
                TimerText.enabled = false;
                return;
            }
            
            TimerText.text = Mathf.CeilToInt(CurrentTime).ToString();
        }

        public void OnDestroy()
        {
            var myHashCode = GetHashCode();
            Buttons.RemoveAll(b => b.GetHashCode() == myHashCode);
        }

        #endregion
    }
}