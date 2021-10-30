using System;
using Laboratory.Mods.Player.Attributes;
using Laboratory.Mods.Player.Interfaces;
using Reactor;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Mods.Player.MonoBehaviours
{
    [RegisterInIl2Cpp, PlayerComponent]
    public class PlayerManager : MonoBehaviour
    {
        public PlayerManager(IntPtr ptr) : base(ptr) { }
        
        public bool AmOwner => MyPlayer.AmOwner;
        public PlayerControl MyPlayer { get; set; }
        public PlayerPhysics MyPhysics { get; set; }
        public CustomNetworkTransform MyNetTransform { get; set; }
        
        private IAnimationController m_AnimationController;
        public IAnimationController AnimationController
        {
            [HideFromIl2Cpp] get => m_AnimationController;
            [HideFromIl2Cpp] set
            {
                m_AnimationController = value;
                value.Initialize(this);
            }
        }
        
        public void Awake()
        {
            MyPlayer = GetComponent<PlayerControl>();
            MyPhysics = GetComponent<PlayerPhysics>();
            MyNetTransform = GetComponent<CustomNetworkTransform>();
        }
        
        public void Start()
        {
            if (MyPhysics)
            {
                if (MyPhysics.body) MyPhysics.EnableInterpolation();
                MyPhysics.Skin.transform.Find("SpawnInGlow").gameObject.SetActive(false);
            }
            
            // AnimationController = new DefaultAnimationController();
        }
    }
}