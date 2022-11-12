using System;
using Reactor;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Utils;

[RegisterInIl2Cpp]
public class UnityEventListener : MonoBehaviour
{
    public UnityEventListener(IntPtr ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp]
    public event Action? OnDestroyEvent;

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }
}
