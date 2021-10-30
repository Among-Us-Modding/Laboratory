using System;
using System.Collections.Generic;
using Laboratory.Mods.Buttons;
using Laboratory.Mods.Player.Attributes;
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
        }
    }
}