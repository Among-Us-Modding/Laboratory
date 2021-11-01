using System;
using Reactor;
using UnityEngine;

namespace Laboratory.Utils
{
    [RegisterInIl2Cpp]
    public class UnityEvents : MonoBehaviour
    {
        /// <summary>
        /// Action which is called according to the update function of unity's event loop
        /// </summary>
        public static Action? UpdateEvent;
        
        public UnityEvents(IntPtr ptr) : base(ptr) { }
        
        private void Update()
        {
            UpdateEvent?.Invoke();
        }
    }
}