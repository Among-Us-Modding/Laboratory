using System;
using System.Collections.Generic;
using Laboratory.Effects.Interfaces;
using Reactor;
using Reactor.Networking;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Effects.MonoBehaviours;

[RegisterInIl2Cpp]
public class GlobalEffectManager : MonoBehaviour
{
    public GlobalEffectManager(IntPtr ptr) : base(ptr) { }

    public static GlobalEffectManager? Instance { get; set; }

    private IEffect? _primaryEffect;
        
    public IEffect? PrimaryEffect
    {
        [HideFromIl2Cpp] get => _primaryEffect;
        [HideFromIl2Cpp] set
        {
            var current = PrimaryEffect;
            if (current is not null)
            {
                current.Cancel();
                RemoveEffect(current);
            }
            _primaryEffect = value;
        }
    }
        
    private List<IEffect> Effects { [HideFromIl2Cpp] get; } = new();

    [HideFromIl2Cpp]
    public void RpcAddEffect(IEffect effect, bool primary = false)
    {
        Rpc<RpcIEffect>.Instance.Send(new RpcIEffect.EffectInfo(null, effect, primary), true);
    }

    [HideFromIl2Cpp]
    public void AddEffect(IEffect? effect, bool primary)
    {
        if (effect != null)
        {
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

    private void Awake()
    {
        Instance = this;
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
        Instance = null;
    }
}