using System;
using System.Collections.Generic;
using Laboratory.Effects.Interfaces;
using Laboratory.Extensions;
using Laboratory.Player.Attributes;
using Laboratory.Player.Managers;
using Reactor;
using Reactor.Networking;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Effects.MonoBehaviours;

[RegisterInIl2Cpp, PlayerComponent]
public class PlayerEffectManager : MonoBehaviour
{
    public PlayerEffectManager(IntPtr ptr) : base(ptr)
    {
    }

    private PlayerManager? _myManager;
    private IEffect? _primaryEffect;

    public IEffect? PrimaryEffect
    {
        [HideFromIl2Cpp]
        get => GlobalEffectManager.Instance != null ? GlobalEffectManager.Instance.PrimaryEffect ?? _primaryEffect : _primaryEffect;
        [HideFromIl2Cpp]
        set
        {
            var current = PrimaryEffect;
            if (current is not null)
            {
                if (current.Owner != _myManager) throw new InvalidOperationException("You cannot set a player's effect during a primary global effect");
                current.Cancel();
                RemoveEffect(current);
            }

            _primaryEffect = value;
        }
    }

    public List<IEffect> Effects
    {
        [HideFromIl2Cpp]
        get;
    } = new();

    [HideFromIl2Cpp]
    public void RpcAddEffect(IEffect effect, bool primary = false)
    {
        Rpc<RpcIEffect>.Instance.Send(new RpcIEffect.EffectInfo(_myManager!.Player, effect, primary), true);
    }

    [HideFromIl2Cpp]
    public void AddEffect(IEffect? effect, bool primary)
    {
        if (effect != null)
        {
            effect.Owner = _myManager!;
            effect.Awake();
            Effects.Add(effect);
        }

        if (primary) PrimaryEffect = effect;
    }

    [HideFromIl2Cpp]
    public void RemoveEffect(IEffect effect)
    {
        if (_primaryEffect == effect) _primaryEffect = null;
        Effects.Remove(effect);
        effect.OnDestroy();
    }

    public void ClearEffects()
    {
        foreach (var effect in Effects)
        {
            RemoveEffect(effect);
        }
    }

    private void Start()
    {
        _myManager = this.GetPlayerManager();
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
            RemoveEffect(effect);
        }
    }

    private void OnDestroy()
    {
        foreach (var effect in Effects) effect.Cancel();
    }
}
