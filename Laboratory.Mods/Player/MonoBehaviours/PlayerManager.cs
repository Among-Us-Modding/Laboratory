using System;
using System.Collections.Generic;
using Laboratory.Mods.Buttons;
using Reactor;
using UnhollowerBaseLib.Attributes;

namespace Laboratory.Mods.Player.MonoBehaviours
{
    [RegisterInIl2Cpp()]
    public class PlayerManager : PlayerComponent
    {
        public PlayerManager(IntPtr ptr) : base(ptr)
        {
        }
        
        public List<CooldownButton> Buttons { [HideFromIl2Cpp] get; } = new();

        public void Start()
        {
            if (MyPhysics)
            {
                if (MyPhysics.body) MyPhysics.EnableInterpolation();
                MyPhysics.Skin.transform.Find("SpawnInGlow").gameObject.SetActive(false);
            }
        }
        
        public void Update()
        {
            if (!AmOwner) return;
            
            foreach (CooldownButton button in new List<CooldownButton>(Buttons)) button.Update();
        }
    }
}