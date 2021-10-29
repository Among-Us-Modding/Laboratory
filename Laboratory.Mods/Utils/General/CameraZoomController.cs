using System;
using Reactor;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.Mods.Utils.General
{
    /// <summary>
    /// Camera allowing for zooming while in game without interacting with other Hud elements
    /// </summary>
    [RegisterInIl2Cpp]
    public class CameraZoomController : MonoBehaviour
    {
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
            [HideFromIl2Cpp] get => m_OrthographicSize;
            [HideFromIl2Cpp] set
            {
                ShadowCollab.ShadowCamera.aspect = Cam.aspect;
                ShadowCollab.ShadowCamera.orthographicSize = Cam.orthographicSize = m_OrthographicSize = value;
                ShadowCollab.ShadowQuad.transform.localScale = new Vector3(value * Cam.aspect, value) * 2f;
            }
        }

        /// <summary>
        /// Aspect of the zoom camera
        /// </summary>
        public float Aspect
        {
            [HideFromIl2Cpp] get => m_Aspect;
            [HideFromIl2Cpp] set
            {
                if (Math.Abs(m_Aspect - value) > 0.05f)
                {
                    m_Aspect = value;
                    Cam.aspect = value;
                    OrthographicSize = OrthographicSize;
                }
            }
        }
        
        private float m_OrthographicSize = 3;
        private float m_Aspect = 0;
        
        private void Awake()
        {
            Instance = this;

            ShadowCollab = FindObjectOfType<ShadowCollab>();
            ShadowCollab.StopAllCoroutines();
            
            HudManager.Instance.FullScreen.transform.localScale *= 50;

            Camera mainCam = Camera.main;
            GameObject newCamObj = new("ZoomCamera");
            Transform newCamTransform = newCamObj.transform;
            newCamTransform.parent = mainCam!.transform;
            newCamTransform.localPosition = new Vector3(0, 0, 0);
            newCamTransform.localScale = new Vector3(1, 1, 1);
            Cam = newCamObj.AddComponent<Camera>();
            Cam.CopyFrom(mainCam);
            Cam.depth += 1;
            m_Aspect = mainCam.aspect;
            mainCam.ResetReplacementShader(); // TODO Review This
        }
        
        private void Update()
        {
            Camera mainCam = Camera.main;
            if (!mainCam) return;
            
            Cam.backgroundColor = mainCam.backgroundColor;
            Aspect = mainCam.aspect;
        }
        
        private void OnDestroy()
        {
            Instance = null;
        }
    }
}