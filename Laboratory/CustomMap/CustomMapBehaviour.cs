using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.Attributes;
using Reactor;
using Reactor.Utilities.Attributes;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Laboratory.CustomMap;

[RegisterInIl2Cpp]
public class CustomMapBehaviour : MonoBehaviour
{
    public CustomMapBehaviour(IntPtr ptr) : base(ptr) { }

    public delegate void MouseClickEvent(CustomMapBehaviour instance, int mouseButton, Vector2 worldPosition);

    // PLS Unsubscribe from these thanks
    public MouseClickEvent? MouseDownEvent { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
    public MouseClickEvent? MouseHeldEvent { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
    public MouseClickEvent? MouseUpEvent { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }

    private Dictionary<GameData.PlayerInfo, SpriteRenderer> HerePoints { [HideFromIl2Cpp] get; } = new();

    public MapBehaviour? Parent { get; set; }
    
    public void ShowAllPlayers()
    {
        foreach ((GameData.PlayerInfo _, SpriteRenderer value) in HerePoints)
        {
            Destroy(value.gameObject);
        }
        HerePoints.Clear();
        if (Parent == null) return;
        foreach (GameData.PlayerInfo? player in GameData.Instance.AllPlayers)
        {
            if (player.Disconnected) continue;
            Transform hereTransform = Parent.HerePoint.transform;

            GameObject? newHerePoint = Instantiate(Parent.HerePoint.gameObject, hereTransform.parent, true);
            newHerePoint.transform.localScale = hereTransform.localScale;
                
            SpriteRenderer? newHerePointRend = newHerePoint.GetComponent<SpriteRenderer>();
            if (player.Object) player.Object.SetPlayerMaterialColors(newHerePointRend);
            HerePoints[player] = newHerePointRend;
        }

        FixedUpdate();
    }

    public static void ShowWithAllPlayers(Color mapColor, MouseClickEvent mouseUpEvent)
    {
        HudManager.Instance.ShowMap((Action<MapBehaviour>)(map =>
        {
            if (map.IsOpen)
            {
                map.Close();
                return;
            }
                
            map.countOverlay.gameObject.SetActive(false);
            map.infectedOverlay.gameObject.SetActive(false);
            map.taskOverlay.Hide();
            map.GenericShow();
            PlayerControl.LocalPlayer.SetPlayerMaterialColors(map.HerePoint);
            map.ColorControl.SetColor(mapColor);
            DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
            CustomMapBehaviour? customMapBehaviour = map.gameObject.GetComponent<CustomMapBehaviour>();
            customMapBehaviour.ShowAllPlayers();

            // sketchy hack to remove the delegate when done
            MouseClickEvent[] clicks = new MouseClickEvent[2];
            customMapBehaviour.MouseUpEvent += clicks[0] = mouseUpEvent;
            customMapBehaviour.MouseUpEvent += clicks[1] = delegate(CustomMapBehaviour instance, int _, Vector2 _)
            {
                foreach (MouseClickEvent mouseClickEvent in clicks)
                {
                    instance.MouseUpEvent -= mouseClickEvent;
                }
            };
        }));
    }
        
    [HideFromIl2Cpp]
    private Vector2 GetMapPosition(ref bool set, ref Vector2 mapPosition)
    {
        if (set) return mapPosition;
        set = true;

        if (Parent == null) return default;
            
        Vector2 offset = ShipStatus.Instance.Type switch
        {
            ShipStatus.MapType.Ship => new Vector2(-2.3f, -5.4f),
            ShipStatus.MapType.Hq => new Vector2(9.1f, 11.1f),
            ShipStatus.MapType.Pb => new Vector2(20.7f, -12.0f),
            (ShipStatus.MapType) 3 => new Vector2(7.3f, 0.2f),
            _ => Vector2.zero
        };

        Vector2 mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pointOnMap = Parent.transform.InverseTransformPoint(mousePos);
        pointOnMap.x /= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
        pointOnMap *= ShipStatus.Instance.MapScale;
        return mapPosition = pointOnMap + offset;
    }
        
    private void Awake()
    {
        Parent = GetComponent<MapBehaviour>();
    }
        
    private void FixedUpdate()
    {
        if (!ShipStatus.Instance)
        {
            return;
        }

        foreach ((GameData.PlayerInfo? data, SpriteRenderer rend) in HerePoints)
        {
            if (data == null || !data.Object || data.IsDead || data.Disconnected)
            {
                rend.enabled = false;
                continue;
            }
            rend.enabled = true;
            Vector3 vector = data.Object.transform.position;
            vector /= ShipStatus.Instance.MapScale;
            vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
            vector.z = -1f;
            rend.transform.localPosition = vector;
        }
    }
        
    private void Update()
    {
        Vector2 mapPosition = Vector2.zero;
        bool mapPositionSet = false;
        if (MouseDownEvent != null)
        {
            if (Input.GetMouseButtonDown(0)) MouseDownEvent(this, 0, GetMapPosition(ref mapPositionSet, ref mapPosition));
            if (Input.GetMouseButtonDown(1)) MouseDownEvent(this, 1, GetMapPosition(ref mapPositionSet, ref mapPosition));
            if (Input.GetMouseButtonDown(2)) MouseDownEvent(this, 2, GetMapPosition(ref mapPositionSet, ref mapPosition));
        }
            
        if (MouseHeldEvent != null)
        {
            if (Input.GetMouseButton(0)) MouseHeldEvent(this, 0, GetMapPosition(ref mapPositionSet, ref mapPosition));
            if (Input.GetMouseButton(1)) MouseHeldEvent(this, 1, GetMapPosition(ref mapPositionSet, ref mapPosition));
            if (Input.GetMouseButton(2)) MouseHeldEvent(this, 2, GetMapPosition(ref mapPositionSet, ref mapPosition));
        }
            
        if (MouseUpEvent != null)
        {
            if (Input.GetMouseButtonUp(0)) MouseUpEvent(this, 0, GetMapPosition(ref mapPositionSet, ref mapPosition));
            if (Input.GetMouseButtonUp(1)) MouseUpEvent(this, 1, GetMapPosition(ref mapPositionSet, ref mapPosition));
            if (Input.GetMouseButtonUp(2)) MouseUpEvent(this, 2, GetMapPosition(ref mapPositionSet, ref mapPosition));
        }
    }
        
    private void OnDisable()
    {
        foreach ((GameData.PlayerInfo _, SpriteRenderer value) in HerePoints)
        {
            Destroy(value.gameObject);
        }
        HerePoints.Clear();
    }
}