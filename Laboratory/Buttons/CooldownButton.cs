using System;
using System.Collections.Generic;
using System.Linq;
using Laboratory.Utilities;
using Reactor.Utilities.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Laboratory.Buttons;

[RegisterInIl2Cpp]
public class CooldownButton : MonoBehaviour
{
    public static float XDistFromEdge = 1.1f;
    public static float YDistFromEdge = 1.1f;

    private static readonly int Percent = Shader.PropertyToID("_Percent");
    private static readonly int Desat = Shader.PropertyToID("_Desat");

    public static List<CooldownButton> Buttons { get; } = new();

    public static T Create<T>() where T : CooldownButton
    {
        GameObject buttonObj = new GameObject(typeof(T).Name) {layer = 5};
        buttonObj.transform.parent = HudManager.Instance.transform;
        return buttonObj.AddComponent<T>();
    }

    public CooldownButton(IntPtr ptr) : base(ptr) { }

    public SpriteRenderer Renderer;
    public TextMeshPro TimerText;
    public AspectPosition Aspect;
    public PassiveButton Button;
    public Action OnClickAction;
        
    public float CurrentTime;
    public float Cooldown;

    #region Event Loop

    public void Awake()
    {
        Buttons.Add(this);

        GameObject buttonObj = gameObject;
            
        // SpriteRenderer
        Renderer = buttonObj.AddComponent<SpriteRenderer>();
        Renderer.material = new Material(Shader.Find("Unlit/CooldownShader"));
            
        // TextMeshPro
        TimerText = Instantiate(HudManager.Instance.KillButton.cooldownTimerText, buttonObj.transform);
        TimerText.transform.localPosition = new Vector3(0, 0.07f, -0.001f);
        TimerText.gameObject.SetActive(true);
        
        // AspectPosition
        Aspect = buttonObj.AddComponent<AspectPosition>();
        Aspect.parentCam = HudManager.Instance.UICamera;
            
        // PassiveButton
        BoxCollider2D buttonColl = buttonObj.AddComponent<BoxCollider2D>();
        buttonColl.size = new Vector2(1.15f, 1.15f);

        Button = buttonObj.AddComponent<PassiveButton>();
        Button.OnMouseOut = Button.OnMouseOver = new Button.ButtonClickedEvent();
        Button.Colliders = new[] { buttonColl };
        Button.OnClick = new Button.ButtonClickedEvent();
        Button.OnClick.AddListener((Action) InvokePerformClick);
            
        SetPosition(AspectPosition.EdgeAlignments.LeftBottom);
    }
        
    public virtual void Update()
    {
        Button!.enabled = Renderer!.enabled = TimerText!.enabled = ShouldBeVisible();
        // ReSharper disable once CompareOfFloatsByEqualityOperator -> Its set to this value manually
        if (CurrentTime == int.MaxValue) return;
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
        int myHashCode = GetHashCode();
        Buttons.RemoveAll(b => b.GetHashCode() == myHashCode);
    }

    #endregion
        
    #region Appearance

    public void SetPosition(AspectPosition.EdgeAlignments alignment, int buttonIndex = Int32.MaxValue)
    {
        int myHashCode = GetHashCode();
        if (buttonIndex == Int32.MaxValue) buttonIndex = Buttons.Count(c => c.GetHashCode() != myHashCode && c.Aspect!.Alignment == alignment);
            
        Vector3 distanceFromEdge = new(0.7f, 0.7f, -5f);
            
        distanceFromEdge.x += XDistFromEdge * Mathf.FloorToInt(buttonIndex / 3f);
        distanceFromEdge.y += YDistFromEdge * (buttonIndex % 3);
        distanceFromEdge.z = -5f;
            
        Aspect!.Alignment = alignment;
        Aspect.DistanceFromEdge = distanceFromEdge;
        Aspect.AdjustPosition();
    }

    public void SetSprite(string spriteName, float ppu = 100) => SetSprite(AssetManager.LoadSprite(spriteName, ppu));

    public void SetSprite(Sprite sprite)
    {
        Renderer!.sprite = sprite;
        // ReSharper disable once InvokeAsExtensionMethod
        CooldownHelpers.SetCooldownNormalizedUvs(Renderer);
    }

    public void SetButtonCooldownLevel(float amount)
    {
        Renderer!.material.SetFloat(Percent, amount);
    }

    public void SetButtonSaturation(bool saturated)
    {
        Renderer!.material.SetFloat(Desat, saturated ? 0 : 1);
        Renderer.color = saturated ? Palette.EnabledColor : Palette.DisabledClear;
    }
        
    #endregion

    #region Usability

    public virtual bool Unlocked() => true;

    public virtual bool ShouldBeVisible()
    {
        if (!ShipStatus.Instance) return false;
        if (!HudManager.Instance.UseButton.gameObject.active && !HudManager.Instance.PetButton.gameObject.active) return false;
        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        if (!localPlayer || localPlayer.Data == null) return false;
        if (localPlayer.Data.IsDead) return false;
        return true;
    }

    public virtual bool ShouldCooldown() => PlayerControl.LocalPlayer.CanMove;

    public virtual bool CanUse()
    {
        if (!isActiveAndEnabled || CurrentTime > 0) return false;
        if (!Unlocked()) return false;
        if (!ShipStatus.Instance || MeetingHud.Instance || Minigame.Instance) return false;
        if (PlayerControl.LocalPlayer.inVent) return false;
        return true;
    }

    public void InvokePerformClick()
    {
        PerformClick();
    }
        
    public virtual void PerformClick()
    {
        if (!ShouldCooldown() || !ShouldBeVisible() || !CanUse()) return;

        CurrentTime = Cooldown;
        OnClickAction?.Invoke();
    }
    #endregion
}