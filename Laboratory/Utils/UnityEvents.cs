using System;
using Reactor;
using UnityEngine;

namespace Laboratory.Utils;

[RegisterInIl2Cpp]
public class UnityEvents : MonoBehaviour
{
    public static event Action? OnEnableEvent;
    public static event Action? UpdateEvent;
    public static event Action? FixedUpdateEvent;

    public UnityEvents(IntPtr ptr) : base(ptr)
    {
    }

    private void OnEnable()
    {
        OnEnableEvent?.Invoke();
    }
    
    private void Update()
    {
        UpdateEvent?.Invoke();
    }
    
    private void FixedUpdate()
    {
        FixedUpdateEvent?.Invoke();
    }
}
