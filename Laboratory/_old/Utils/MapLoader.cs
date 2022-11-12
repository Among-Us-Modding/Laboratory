using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.IL2CPP.Utils;
using Reactor;
using Reactor.Extensions;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Laboratory.Utils;

/// <summary>
/// Utility that preloads maps so you can use their assets later
/// </summary>
[RegisterInIl2Cpp]
public class MapLoader : MonoBehaviour
{
    public static MapLoader? Instance { get; set; }

    public static bool LoadSkeld { get; set; }
    public static bool LoadMira { get; set; }
    public static bool LoadPolus { get; set; }
    public static bool LoadDleks { get; set; }
    public static bool LoadAirship { get; set; }

    public static SkeldShipStatus? Skeld { get; private set; }
    public static MiraShipStatus? Mira { get; private set; }
    public static PolusShipStatus? Polus { get; private set; }
    public static SkeldShipStatus? Dleks { get; private set; }
    public static AirshipStatus? Airship { get; private set; }

    public MapLoader(IntPtr ptr) : base(ptr)
    {
    }

    private void Start()
    {
        if (Instance)
        {
            Warning($"{nameof(MapLoader)} spawned twice");
            this.Destroy();
            return;
        }

        Instance = this;
        this.StartCoroutine(CoLoadMaps());
    }

    [HideFromIl2Cpp]
    public IEnumerator CoLoadMaps()
    {
        while (AmongUsClient.Instance == null) yield return null;

        List<int>? toLoad = new List<int>();
        if (LoadSkeld) toLoad.Add(0);
        if (LoadMira) toLoad.Add(1);
        if (LoadPolus) toLoad.Add(2);
        if (LoadDleks) toLoad.Add(3);
        if (LoadAirship) toLoad.Add(4);

        foreach (int i in toLoad)
        {
            AssetReference? shipPrefab = AmongUsClient.Instance.ShipPrefabs.ToArray()[i];
            AsyncOperationHandle<GameObject>? shipAsset = shipPrefab.LoadAsset<GameObject>();

            yield return shipAsset;

            var shipStatus = shipAsset.Result.DontUnload();

            switch (i)
            {
                case 0:
                    Skeld = shipStatus.GetComponent<SkeldShipStatus>();
                    break;

                case 1:
                    Mira = shipStatus.GetComponent<MiraShipStatus>();
                    break;

                case 2:
                    Polus = shipStatus.GetComponent<PolusShipStatus>();
                    break;

                case 3:
                    Dleks = shipStatus.GetComponent<SkeldShipStatus>();
                    break;

                case 4:
                    Airship = shipStatus.GetComponent<AirshipStatus>();
                    break;
            }
        }
    }
}
