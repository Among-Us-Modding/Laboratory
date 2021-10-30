using System;
using System.Collections.Generic;
using Laboratory.Mods.Effects.Interfaces;
using Laboratory.Mods.Player.Attributes;
using Laboratory.Mods.Player.MonoBehaviours;
using Reactor;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Mods.Effects.MonoBehaviours
{
    [RegisterInIl2Cpp, PlayerComponent]
    public class PlayerEffectManager : MonoBehaviour
    {
        public PlayerEffectManager(IntPtr ptr) : base(ptr) { }

        private PlayerManager m_MyManager;
        private IEffect m_PrimaryEffect;
        
        public IEffect PrimaryEffect
        {
            [HideFromIl2Cpp] get => GlobalEffectManager.Instance ? GlobalEffectManager.Instance.PrimaryEffect ?? m_PrimaryEffect : m_PrimaryEffect;
            [HideFromIl2Cpp] set
            {
                var current = PrimaryEffect;
                if (current is not null)
                {
                    if (current.Owner != m_MyManager) throw new InvalidOperationException("You cannot set a player's effect during a primary global effect");
                    current.Cancel();
                    RemoveEffect(current);
                }
                m_PrimaryEffect = value;
            }
        }

        private List<IEffect> Effects { [HideFromIl2Cpp] get; } = new();

        [HideFromIl2Cpp]
        public void AddEffect(IEffect effect, bool primary)
        {
            effect.Owner = m_MyManager;
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

        public void Start()
        {
            m_MyManager = GetComponent<PlayerManager>();
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
        }
    }
}