using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.Attributes;
using Laboratory.Player.Attributes;
using Laboratory.Player.Extensions;
using Laboratory.Player.Managers;
using Reactor.Networking.Rpc;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Laboratory.Effects.Managers;

[RegisterInIl2Cpp, PlayerComponent]
public class PlayerEffectManager : MonoBehaviour, IEffectManager
{
    public PlayerEffectManager(IntPtr ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp]
    public PlayerManager Manager { get; private set; } = null!;

    private IEffect _primaryEffect;

    [HideFromIl2Cpp]
    public IEffect PrimaryEffect
    {
        get => GlobalEffectManager.Instance != null ? GlobalEffectManager.Instance.PrimaryEffect ?? _primaryEffect : _primaryEffect;
        set
        {
            IEffect current = PrimaryEffect;
            if (current is not null)
            {
                current.Cancel();
                RemoveEffect(current);
            }

            _primaryEffect = value;
        }
    }

    [HideFromIl2Cpp]
    public List<IEffect> Effects { get; } = new();

    [HideFromIl2Cpp]
    public void RpcAddEffect(IEffect effect, bool primary = false)
    {
        Rpc<RpcAddEffect>.Instance.Send(new RpcAddEffect.EffectInfo(this, effect, primary), true);
    }

    [HideFromIl2Cpp]
    public void AddEffect(IEffect effect, bool primary)
    {
        if (primary && PrimaryEffect != null && PrimaryEffect == GlobalEffectManager.Instance!.PrimaryEffect && effect != null)
        {
            effect.Timer = -1;
            return;
        }
        
        if (primary) PrimaryEffect = effect;

        if (effect == null) return;
        if (effect is IPlayerEffect playerEffect)
        {
            playerEffect.Owner = Manager;
        }

        effect.Awake();
        Effects.Add(effect);
    }

    [HideFromIl2Cpp]
    public void RemoveEffect(IEffect effect)
    {
        if (_primaryEffect == effect) _primaryEffect = null;
        else effect.OnDestroy();
        effect.Timer = -1;
        Effects.Remove(effect);
    }

    [HideFromIl2Cpp]
    public void ClearEffects()
    {
        foreach (IEffect effect in new List<IEffect>(Effects))
        {
            RemoveEffect(effect);
        }
    }

    private void Start()
    {
        Manager = this.GetPlayerManager();
        GlobalEffectManager.PlayerEffectManagers.Add(this);
    }

    private void FixedUpdate()
    {
        foreach (IEffect effect in Effects) effect.FixedUpdate();
    }

    private void Update()
    {
        foreach (IEffect effect in Effects) effect.Update();
    }

    private void LateUpdate()
    {
        List<IEffect> effects = new();
        foreach (IEffect effect in Effects)
        {
            effect.LateUpdate();
            if (effect.Timer < 0) effects.Add(effect);
        }

        foreach (IEffect effect in effects)
        {
            if (_primaryEffect == effect) _primaryEffect = null;
            RemoveEffect(effect);
        }
    }

    private void OnDestroy()
    {
        foreach (IEffect effect in Effects) effect.Cancel();
        GlobalEffectManager.PlayerEffectManagers.Remove(this);
    }
}
