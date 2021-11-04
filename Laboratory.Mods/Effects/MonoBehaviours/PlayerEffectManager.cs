using System;
using System.Collections.Generic;
using Laboratory.Mods.Effects.Interfaces;
using Laboratory.Mods.Player.Attributes;
using Laboratory.Mods.Player.MonoBehaviours;
using Reactor;
using Reactor.Networking;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Mods.Effects.MonoBehaviours
{
    [RegisterInIl2Cpp, PlayerComponent]
    public class PlayerEffectManager : MonoBehaviour
    {
        public PlayerEffectManager(IntPtr ptr) : base(ptr) { }

        private PlayerManager? m_MyManager;
        private IEffect? m_PrimaryEffect;
        
        public IEffect? PrimaryEffect
        {
            [HideFromIl2Cpp] get => GlobalEffectManager.Instance != null ? GlobalEffectManager.Instance.PrimaryEffect ?? m_PrimaryEffect : m_PrimaryEffect;
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
        public void RpcAddEffect(IEffect effect, bool primary = false)
        {
            Rpc<RpcIEffect>.Instance.Send(new RpcIEffect.EffectInfo(m_MyManager!.MyPlayer, effect, primary), true);
        }
        
        [HideFromIl2Cpp]
        public void AddEffect(IEffect? effect, bool primary)
        {
            if (effect != null)
            {
                effect.Owner = m_MyManager!;
                effect.Awake();
                Effects.Add(effect);
            }

            if (primary) PrimaryEffect = effect;
        }
        
        [HideFromIl2Cpp]
        public void RemoveEffect(IEffect effect)
        {
            if (m_PrimaryEffect == effect) m_PrimaryEffect = null;
            Effects.Remove(effect);
            effect.OnDestroy();
        }

        private void Start()
        {
            m_MyManager = GetComponent<PlayerManager>();
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
}