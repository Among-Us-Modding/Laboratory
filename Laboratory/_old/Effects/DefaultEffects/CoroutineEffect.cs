using System.Collections;
using BepInEx.IL2CPP.Utils;
using Laboratory.Player.Managers;
using Reactor;
using UnityEngine;

namespace Laboratory.Effects.DefaultEffects;

public abstract class CoroutineEffect : IPlayerEffect
{
    public float Timer { get; set; } = 1;
    public PlayerManager Owner { get; set; } = null!;
    public Coroutine EffectRoutine { get; set; } = null!;
    
    public virtual void Awake()
    {
        EffectRoutine = Owner.StartCoroutine(CoEffect());
    }

    public abstract IEnumerator CoEffect();
    
    public virtual void Cancel()
    {
        Owner.StopCoroutine(EffectRoutine);
    }
}