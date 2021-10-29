using System;
using Reactor;
using UnityEngine;

namespace Laboratory.Utils
{
    [RegisterInIl2Cpp]
    public class UnityEvents : MonoBehaviour
    {
        public static Action UpdateEvent;
        
        public UnityEvents(IntPtr ptr) : base(ptr) { }
        
        private void Update()
        {
            UpdateEvent?.Invoke();
        }
    }
}