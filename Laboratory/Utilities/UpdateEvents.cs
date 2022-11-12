using System;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Laboratory.Utilities;

[RegisterInIl2Cpp]
public class UpdateEvents : MonoBehaviour
{
    /// <summary>
    /// Action which is called according to the update function of unity's event loop
    /// </summary>
    public static event Action UpdateEvent;

    public UpdateEvents(IntPtr ptr) : base(ptr)
    {
    }

    private void Update()
    {
        UpdateEvent?.Invoke();
    }
}
