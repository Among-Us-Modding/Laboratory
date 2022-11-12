using System;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Laboratory.Utilities;

[RegisterInIl2Cpp]
public class DestroyEventListener : MonoBehaviour
{
    public DestroyEventListener(IntPtr ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp]
    public event Action OnDestroyEvent;

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }
}
