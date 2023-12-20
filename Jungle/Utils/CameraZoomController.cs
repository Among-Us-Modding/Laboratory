using System;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Jungle.Utils;

/// <summary>
/// Camera allowing for zooming while in game without interacting with other Hud elements
/// </summary>
[RegisterInIl2Cpp, HarmonyPatch]
public class CameraZoomController : MonoBehaviour
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    [HarmonyPostfix]
    public static void AddCameraPatch()
    {
        Camera.main!.gameObject.AddComponent<CameraZoomController>();
    }
    
    /// <summary>
    /// Current instance of the CameraZoomController
    /// </summary>
    public static CameraZoomController Instance { get; set; }
        
    public CameraZoomController(IntPtr ptr) : base(ptr) { }

    /// <summary>
    /// Zoom controller's camera
    /// </summary>
    public Camera Cam { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
        
    private ShadowCollab ShadowCollab { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
        
    /// <summary>
    /// Orthographic size of the zoom camera
    /// </summary>
    public float OrthographicSize
    {
        [HideFromIl2Cpp] get => _orthographicSize;
        [HideFromIl2Cpp] set
        {
            ShadowCollab!.ShadowCamera.aspect = Cam!.aspect;
            ShadowCollab.ShadowCamera.orthographicSize = Cam.orthographicSize = _orthographicSize = value;
            ShadowCollab.ShadowQuad.transform.localScale = new Vector3(value * Cam.aspect, value) * 2f;
        }
    }

    /// <summary>
    /// Aspect of the zoom camera
    /// </summary>
    public float Aspect
    {
        [HideFromIl2Cpp] get => _aspect;
        [HideFromIl2Cpp] set
        {
            if (Math.Abs(_aspect - value) > 0.05f)
            {
                _aspect = value;
                Cam!.aspect = value;
                OrthographicSize = OrthographicSize;
            }
        }
    }
        
    private float _orthographicSize = 3;
    private float _aspect;
        
    private void Awake()
    {
        Instance = this;

        ShadowCollab = FindObjectOfType<ShadowCollab>();
        ShadowCollab.StopAllCoroutines();
            
        HudManager.Instance.FullScreen.transform.localScale *= 50;

        var mainCam = Camera.main!;
        GameObject newCamObj = new("ZoomCamera");
        var newCamTransform = newCamObj.transform;
        newCamTransform.parent = mainCam.transform;
        newCamTransform.localPosition = new Vector3(0, 0, 0);
        newCamTransform.localScale = new Vector3(1, 1, 1);
        Cam = newCamObj.AddComponent<Camera>();
        Cam.CopyFrom(mainCam);
        Cam.depth += 1;
        _aspect = mainCam.aspect;
        mainCam.ResetReplacementShader();
    }
        
    private void Update()
    {
        var mainCam = Camera.main!;
        if (!mainCam) return;
            
        Cam!.backgroundColor = mainCam.backgroundColor;
        Aspect = mainCam.aspect;
    }
        
    private void OnDestroy()
    {
        Instance = null;
    }
}