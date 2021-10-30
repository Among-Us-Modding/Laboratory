using System;
using System.Collections.Generic;
using Laboratory.Mods.Effects.Interfaces;
using Reactor;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Mods.Effects.MonoBehaviours
{
    [RegisterInIl2Cpp]
    public class GlobalEffectManager : MonoBehaviour
    {
        public GlobalEffectManager(IntPtr ptr) : base(ptr) { }

        public static GlobalEffectManager Instance { get; set; }

        private IEffect m_PrimaryEffect;

        [HideFromIl2Cpp]
        public IEffect PrimaryEffect
        {
            get => m_PrimaryEffect;
            set
            {
                var current = PrimaryEffect;
                if (current is not null)
                {
                    current.Cancel();
                    RemoveEffect(current);
                }
                m_PrimaryEffect = value;
            }
        }
        
        [HideFromIl2Cpp] 
        private List<IEffect> Effects { get; } = new();

        [HideFromIl2Cpp]
        public void AddEffect(IEffect effect, bool primary)
        {
            effect.Awake();
            Effects.Add(effect);
            
            if (primary) PrimaryEffect = effect;
        }
        
        [HideFromIl2Cpp]
        public void RemoveEffect(IEffect effect)
        {
            if (m_PrimaryEffect == effect) m_PrimaryEffect = null;
            Effects.Remove(effect);
            effect.OnDestroy();
        }

        public void Awake()
        {
            Instance = this;
        }

        public void FixedUpdate()
        {
            foreach (IEffect effect in Effects) effect.FixedUpdate();
        }
        
        public void Update()
        {
            foreach (IEffect effect in Effects) effect.Update();
        }

        public void LateUpdate()
        {
            List<IEffect> effects = new();
            foreach (IEffect effect in Effects)
            {
                effect.LateUpdate();
                if (effect.Timer < 0) effects.Add(effect);
            }
            foreach (IEffect effect in effects)
            {
                RemoveEffect(effect);
            }
        }

        public void OnDestroy()
        {
            foreach (IEffect effect in Effects) effect.Cancel();
            Instance = null;
        }
    }
}