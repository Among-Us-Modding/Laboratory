using System;
using System.Linq;
using Laboratory.Mods.Extensions;
using Laboratory.Mods.Player.MonoBehaviours;
using Laboratory.Utils;
using Reactor.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Laboratory.Mods.Buttons
{
    public class CooldownButton
    {
        public static float XDistFromEdge = 1.45f;
        public static float YDistFromEdge = 1.4f;
        
        #region Cached Shader Stuff

        private static readonly int Percent = Shader.PropertyToID("_Percent");
        private static readonly int Desat = Shader.PropertyToID("_Desat");

        #endregion

        #region Button Properties

        public float CurrentTime { get; set; } = 0;
        public float Cooldown { get; set; } = 5f;
        public Action OnClickAction { get; set; }

        #endregion

        #region Physical References

        public GameObject GameObject { get; }
        public KillButtonManager KillButtonManager { get; }
        public AspectPosition AspectPosition { get; }
        public PassiveButton PassiveButton { get; }
        public Button.ButtonClickedEvent ClickEvent { get; }

        #endregion

        #region Setup

        public CooldownButton()
        {
            // Setup References
            GameObject = Object.Instantiate(HudManager.Instance.KillButton.gameObject, HudManager.Instance.transform);
            KillButtonManager = GameObject.GetComponent<KillButtonManager>();
            AspectPosition = GameObject.GetComponent<AspectPosition>();
            PassiveButton = GameObject.GetComponent<PassiveButton>();
            ClickEvent = PassiveButton.OnClick = new Button.ButtonClickedEvent();
            
            // GameObject
            GameObject.name = GetType().Name;

            // Kill Button
            KillButtonManager.killText.enabled = false;
            
            // Aspect Position
            AspectPosition.updateAlways = false; // I changed this from true to false in order to make it work with the Zoom. Please dont kill me.
            SetPosition(AspectPosition.EdgeAlignments.LeftBottom);
            AspectPosition.AdjustPosition();

            // Passive Button
            ClickEvent.AddListener((Action) InvokePerformClick);
            
            PlayerControl.LocalPlayer.GetPlayerComponent<PlayerManager>().Buttons.Add(this);
        }
        
        public void SetPosition(AspectPosition.EdgeAlignments alignment, int buttonIndex = Int32.MaxValue)
        {
            if (buttonIndex == Int32.MaxValue)
            {
                buttonIndex = PlayerControl.LocalPlayer.GetPlayerComponent<PlayerManager>().Buttons.Count(c => c.AspectPosition.Alignment == alignment);
            }
            Vector3 distanceFromEdge = new(0.7f, 0.7f, -5f);
            
            /*
            // This is dnf broken shit
            distanceFromEdge.x += 0.9f * Mathf.FloorToInt(buttonIndex / 3f);
            distanceFromEdge.y += 0.9125f * (buttonIndex % 3);
            */
            
            distanceFromEdge.x += XDistFromEdge * Mathf.FloorToInt(buttonIndex / 3f);
            distanceFromEdge.y += YDistFromEdge * (buttonIndex % 3);
            
            distanceFromEdge.z = -5f;
            AspectPosition.Alignment = alignment;
            AspectPosition.DistanceFromEdge = distanceFromEdge;
        }

        public void SetSprite(string spriteName, float ppu = 100) => SetSprite(spriteName, Mathf.RoundToInt(ppu));
        
        public void SetSprite(string spriteName, int ppu = 100)
        {
            KillButtonManager.renderer.sprite = AssetManager.LoadSprite(spriteName, ppu);
            CooldownHelpers.SetCooldownNormalizedUvs(KillButtonManager.renderer);
        }

        #endregion

        #region Usability

        public virtual bool Unlocked() => true;
        
        public virtual bool ShouldBeVisible() => HudManager.Instance.UseButton.gameObject.active && 
                                         PlayerControl.LocalPlayer != null && 
                                         PlayerControl.LocalPlayer.Data is {IsDead: false} && 
                                         ShipStatus.Instance != null;
        public bool ShouldCooldown() => PlayerControl.LocalPlayer.CanMove;

        public virtual bool CantUse() => CurrentTime > 0 || !Unlocked()||
                                        PlayerControl.LocalPlayer.inVent || 
                                        !ShipStatus.Instance || 
                                        MeetingHud.Instance || 
                                        Minigame.Instance || 
                                        !KillButtonManager.isActiveAndEnabled;

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

        public void Remove()
        {
            PlayerControl.LocalPlayer.GetPlayerComponent<PlayerManager>().Buttons.Remove(this);
            GameObject.Destroy();
        }

        #endregion

        #region Update Loop

        public virtual void Update()
        {
            KillButtonManager.renderer.enabled = true;
            GameObject.SetActive(ShouldBeVisible());
            if (MeetingHud.Instance || ExileController.Instance) CurrentTime = Cooldown;

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
        
        public void SetButtonCooldownLevel(float amount)
        {
            KillButtonManager.renderer.material.SetFloat(Percent, amount);
        }

        public void SetButtonSaturation(bool saturated)
        {
            KillButtonManager.renderer.material.SetFloat(Desat, saturated ? 0 : 1);
            KillButtonManager.renderer.color = saturated ? Palette.EnabledColor : Palette.DisabledClear;
        }

        #endregion
    }
}