using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using Jungle.Extensions;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Jungle.HUDMap;

[RegisterInIl2Cpp, HarmonyPatch]
public class CustomMapBehaviour : MonoBehaviour
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Awake))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    public static void Postfix(MapBehaviour __instance)
    {
        __instance.gameObject.EnsureComponent<CustomMapBehaviour>();
    }
    
    public CustomMapBehaviour(IntPtr ptr) : base(ptr) { }

    public delegate void MouseClickEvent(CustomMapBehaviour instance, int mouseButton, Vector2 worldPosition);

    // PLS Unsubscribe from these thanks
    public MouseClickEvent MouseDownEvent { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
    public MouseClickEvent MouseHeldEvent { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
    public MouseClickEvent MouseUpEvent { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }

    public Action DisableEvent { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; }
    
    private Dictionary<GameData.PlayerInfo, SpriteRenderer> HerePoints { [HideFromIl2Cpp] get; } = new();

    public MapBehaviour Parent { get; set; }
    
    public void ShowAllPlayers()
    {
        foreach (var (_, value) in HerePoints)
        {
            Destroy(value.gameObject);
        }
        HerePoints.Clear();
        if (Parent == null) return;
        foreach (var player in GameData.Instance.AllPlayers)
        {
            if (player.Disconnected) continue;
            var hereTransform = Parent.HerePoint.transform;

            var newHerePoint = Instantiate(Parent.HerePoint.gameObject, hereTransform.parent, true);
            newHerePoint.transform.localScale = hereTransform.localScale;
                
            var newHerePointRend = newHerePoint.GetComponent<SpriteRenderer>();
            if (player.Object) player.Object.SetPlayerMaterialColors(newHerePointRend);
            HerePoints[player] = newHerePointRend;
        }

        FixedUpdate();
    }

    public static void ShowWithAllPlayers(Color mapColor, MouseClickEvent mouseUpEvent)
    {
        if (MapBehaviour.Instance && MapBehaviour.Instance.gameObject.activeSelf)
        {
            MapBehaviour.Instance.Close();
            return;
        }
        if (HudManager.Instance.IsIntroDisplayed) return;
        
        if (!ShipStatus.Instance)  return;
        
        HudManager.Instance.InitMap();
        
        var action = (Action<MapBehaviour>)(map =>
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
            var customMapBehaviour = map.gameObject.GetComponent<CustomMapBehaviour>();
            customMapBehaviour.ShowAllPlayers();

            // sketchy hack to remove the delegate when done
            var clicks = new MouseClickEvent[2];
            customMapBehaviour.MouseUpEvent += clicks[0] = mouseUpEvent;
            customMapBehaviour.MouseUpEvent += clicks[1] = delegate(CustomMapBehaviour instance, int _, Vector2 _)
            {
                foreach (var mouseClickEvent in clicks)
                {
                    instance.MouseUpEvent -= mouseClickEvent;
                }
            };
            
            var disables = new Action[2];
            customMapBehaviour.DisableEvent += disables[0] = () =>
            {
                foreach (var mouseClickEvent in clicks) customMapBehaviour.MouseUpEvent -= mouseClickEvent;
            };
            customMapBehaviour.DisableEvent += disables[1] = () =>
            {
                foreach (var disable in disables) customMapBehaviour.DisableEvent -= disable;
            };
        });
        
        action(MapBehaviour.Instance);
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
            ShipStatus.MapType.Fungle => new Vector2(0.7f, 0.2f),
            (ShipStatus.MapType) 4 => new Vector2(7.3f, 0.2f),
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

        foreach (var (data, rend) in HerePoints)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (data == null || !data.Object || data.IsDead || data.Disconnected)
            {
                rend.enabled = false;
                continue;
            }
            rend.enabled = true;
            var vector = data.Object.transform.position;
            vector /= ShipStatus.Instance.MapScale;
            vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
            vector.z = -1f;
            rend.transform.localPosition = vector;
        }
    }
        
    private void Update()
    {
        var mapPosition = Vector2.zero;
        var mapPositionSet = false;

        if (MouseDownEvent != null) InvokeEvent(MouseDownEvent);
        if (MouseHeldEvent != null) InvokeEvent(MouseHeldEvent);  
        if (MouseUpEvent != null) InvokeEvent(MouseUpEvent);
        return;

        void InvokeEvent(MouseClickEvent mouseEvent)
        {
            if (Input.GetMouseButtonDown(0)) mouseEvent(this, 0, GetMapPosition(ref mapPositionSet, ref mapPosition));
            if (Input.GetMouseButtonDown(1)) mouseEvent(this, 1, GetMapPosition(ref mapPositionSet, ref mapPosition));
            if (Input.GetMouseButtonDown(2)) mouseEvent(this, 2, GetMapPosition(ref mapPositionSet, ref mapPosition));
        }
    }
        
    private void OnDisable()
    {
        DisableEvent?.Invoke();
        foreach (var (_, value) in HerePoints)
        {
            Destroy(value.gameObject);
        }
        HerePoints.Clear();
    }
}