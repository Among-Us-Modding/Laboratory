using System;
using HarmonyLib;
using Laboratory.Extensions;
using Reactor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Laboratory.Patches;

[HarmonyPatch]
internal static class ModStampPatches
{
    public static ModStampManager? StampManager { get; private set; }

    public static Vector3 ComputeStampPosition(ModManager __instance)
    {
        if (!StampManager) StampManager = __instance.gameObject.EnsureComponent<ModStampManager>();

        var z = __instance.localCamera.nearClipPlane + 0.1f;
        var sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Tutorial":
            case "OnlineGame":
                if (HudManager.InstanceExists && HudManager.Instance.Chat.isActiveAndEnabled && !MeetingHud.Instance)
                {
                    return new Vector3(1.8f, 0.4f, z);
                }
                else
                {
                    return new Vector3(1.1f, 0.4f, z);
                }
            default:
                return new Vector3(0.4f, 0.4f, z);
        }
    }
    
    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    [HarmonyPrefix]
    public static bool DisableNormalModStampPatch() => false;

    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    [HarmonyPostfix]
    public static void ModStampPositionPatch(ModManager __instance)
    {
        __instance.ModStamp.enabled = true;

        if (DestroyableSingleton<HudManager>.InstanceExists) __instance.localCamera = HudManager.Instance.UICamera;

        if (!__instance.localCamera) __instance.localCamera = Camera.current;
        if (__instance.localCamera) __instance.ModStamp.transform.position = AspectPosition.ComputeWorldPosition(__instance.localCamera, AspectPosition.EdgeAlignments.RightTop, ComputeStampPosition(__instance));
    }
}

[RegisterInIl2Cpp]
public class ModStampManager : MonoBehaviour
{
    public ModStampManager(IntPtr ptr) : base(ptr)
    {
    }

    private ModManager? ModManager { get; set; }

    private void Awake()
    {
        ModManager = GetComponent<ModManager>();
    }

    private void OnPreRender()
    {
        ModManager!.LateUpdate();
    }
}