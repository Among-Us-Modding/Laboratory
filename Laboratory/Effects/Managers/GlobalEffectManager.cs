using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities.Attributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Laboratory.Effects.Managers;

[RegisterInIl2Cpp]
public class GlobalEffectManager : MonoBehaviour, IEffectManager
{
    public GlobalEffectManager(IntPtr ptr) : base(ptr)
    {
    }

    public static GlobalEffectManager Instance { get; set; }

    public static List<PlayerEffectManager> PlayerEffectManagers { get; } = new();

    public IEffect _primaryEffect;

    [HideFromIl2Cpp]
    public IEffect PrimaryEffect
    {
        get => _primaryEffect;
        set
        {
            foreach (PlayerEffectManager playerEffectManager in PlayerEffectManagers) playerEffectManager.PrimaryEffect = null;

            IEffect current = PrimaryEffect;
            if (current is not null)
            {
                try
                {
                    current.Cancel();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
                
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
        if (primary) PrimaryEffect = effect;

        if (effect != null)
        {
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
        foreach (IEffect effect in Effects)
        {
            RemoveEffect(effect);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        foreach (IEffect effect in Effects)
        {
            try
            {
                effect.FixedUpdate();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

    private void Update()
    {
        foreach (IEffect effect in Effects)
        {
            try
            {
                effect.Update();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

    private void LateUpdate()
    {
        List<IEffect> effects = new();
        foreach (IEffect effect in Effects)
        {
            try
            {
                effect.LateUpdate();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
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
        Instance = null;
    }
}
