using System;
using Reactor;
using UnityEngine;

namespace Laboratory.Mods.Player.MonoBehaviours
{
    [RegisterInIl2Cpp()]
    public class PlayerComponent : MonoBehaviour
    {
        public PlayerComponent(IntPtr ptr) : base(ptr) { }

        public PlayerControl MyPlayer;
        public PlayerPhysics MyPhysics;
        public CustomNetworkTransform MyNetTransform;

        public bool AmOwner => MyPlayer.AmOwner;

        public virtual void Awake()
        {
            MyPlayer = GetComponent<PlayerControl>();
            MyPhysics = GetComponent<PlayerPhysics>();
            MyNetTransform = GetComponent<CustomNetworkTransform>();
        }
    }
}