using System;
using System.Collections.Generic;
using Laboratory.Extensions;
using Laboratory.Player.Attributes;
using Laboratory.Player.Managers;
using Reactor;
using Reactor.Networking;
using UnhollowerBaseLib.Attributes;
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

    private IEffect? _primaryEffect;

    [HideFromIl2Cpp]
    public IEffect? PrimaryEffect
    {
        get => GlobalEffectManager.Instance != null ? GlobalEffectManager.Instance.PrimaryEffect ?? _primaryEffect : _primaryEffect;
        set
        {
            var current = PrimaryEffect;
            if (current is not null)
            {
                if (current == GlobalEffectManager.Instance!.PrimaryEffect)
                {
                    Logger<LaboratoryPlugin>.Error("You cannot set a player's effect during a primary global effect");
                    return;
                }
                current.Cancel();
                RemoveEffect(current);
            }

            _primaryEffect = value;
        }
    }

    [HideFromIl2Cpp]
    public List<IEffect> Effects { get; } = new();

    [HideFromIl2Cpp]
    public void RpcAddEffect(IEffect? effect, bool primary = false)
    {
        Rpc<RpcAddEffect>.Instance.Send(new RpcAddEffect.EffectInfo(this, effect, primary), true);
    }

    [HideFromIl2Cpp]
    public void AddEffect(IEffect? effect, bool primary)
    {
        if (primary) PrimaryEffect = effect;

        if (PrimaryEffect == effect && effect != null)
        {
            if (effect is IPlayerEffect playerEffect)
            {
                playerEffect.Owner = Manager;
            }

            effect.Awake();
            Effects.Add(effect);
        }
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
        foreach (var effect in new List<IEffect>(Effects))
        {
            RemoveEffect(effect);
        }
    }

    private void Start()
    {
        Manager = this.GetPlayerManager();
    }

    private void FixedUpdate()
    {
        foreach (var effect in Effects) effect.FixedUpdate();
    }

    private void Update()
    {
        foreach (var effect in Effects) effect.Update();
    }

    private void LateUpdate()
    {
        List<IEffect> effects = new();
        foreach (var effect in Effects)
        {
            effect.LateUpdate();
            if (effect.Timer < 0) effects.Add(effect);
        }

        foreach (var effect in effects)
        {
            if (_primaryEffect == effect) _primaryEffect = null;
            RemoveEffect(effect);
        }
    }

    private void OnDestroy()
    {
        foreach (var effect in Effects) effect.Cancel();
    }
}
