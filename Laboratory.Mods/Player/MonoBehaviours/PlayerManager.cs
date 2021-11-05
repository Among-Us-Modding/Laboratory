using System;
using Laboratory.Mods.Player.AnimationControllers;
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
        
        public bool AmOwner => MyPlayer!.AmOwner;
        public PlayerControl? MyPlayer { get; set; }
        public PlayerPhysics? MyPhysics { get; set; }
        public CustomNetworkTransform? MyNetTransform { get; set; }
        
        private IAnimationController? _animationController;
        public IAnimationController? AnimationController
        {
            [HideFromIl2Cpp] get => _animationController;
            [HideFromIl2Cpp] set
            {
                _animationController = value;
                value?.Initialize(this);
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
            if (MyPhysics != null)
            {
                if (MyPhysics.body) MyPhysics.EnableInterpolation();
                MyPhysics.Skin.transform.Find("SpawnInGlow").gameObject.SetActive(false);
            }
            
            AnimationController = new DefaultController();
        }

        public void Update()
        {
            AnimationController?.Update();
        }

        public void LateUpdate()
        {
            if (AnimationController == null || MyPlayer == null || MyPhysics == null) return;

            var data = MyPlayer.Data;
            if (data == null) return;
            MyPlayer.nameText.enabled = !AnimationController.HideName;
            MyPhysics.Skin.gameObject.SetActive(!AnimationController.HideCosmetics & MyPlayer.Visible);
            MyPlayer.HatRenderer.gameObject.SetActive(!AnimationController.HideCosmetics && !data.IsDead & MyPlayer.Visible);
            if (MyPlayer.CurrentPet) MyPlayer.CurrentPet.gameObject.SetActive(!AnimationController.HideCosmetics & MyPlayer.Visible);

            MyPlayer.Collider.offset = new Vector2(0, AnimationController.ColliderOffset);
            MyPlayer.myRend.transform.localPosition = new Vector3(0, AnimationController.RendOffset, 0);
        }
    }
}