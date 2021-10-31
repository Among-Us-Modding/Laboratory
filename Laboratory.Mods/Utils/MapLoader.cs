using System;
using System.Collections;
using System.Collections.Generic;
using Reactor;
using Reactor.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Laboratory.Mods.Utils
{
    [RegisterInIl2Cpp]
    public class MapLoader : MonoBehaviour
    {
        public static bool LoadSkeld;
        public static bool LoadMira;
        public static bool LoadPolus;
        public static bool LoadDleks;
        public static bool LoadAirship;

        public static SkeldShipStatus Skeld => Instance.Maps[0].TryCast<SkeldShipStatus>();
        public static MiraShipStatus Mira => Instance.Maps[1].TryCast<MiraShipStatus>();
        public static PolusShipStatus Polus => Instance.Maps[2].TryCast<PolusShipStatus>();
        public static SkeldShipStatus Dleks => Instance.Maps[3].TryCast<SkeldShipStatus>();
        public static AirshipStatus Airship => Instance.Maps[4].TryCast<AirshipStatus>();
        
        public MapLoader(IntPtr ptr) : base(ptr) { }

        public static MapLoader Instance { get; set; }

        private ShipStatus[] Maps;
        private static bool Initialized;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (Initialized) return;
            Initialized = true;
            this.StartCoroutine(CoLoadMaps());
        }

        public IEnumerator CoLoadMaps()
        {
            Maps = new ShipStatus[5];
            
            while (AmongUsClient.Instance == null) yield return null;

            List<int> ToLoad = new();
            if (LoadSkeld) ToLoad.Add(0);
            if (LoadMira) ToLoad.Add(1);
            if (LoadPolus) ToLoad.Add(2);
            if (LoadDleks) ToLoad.Add(3);
            if (LoadAirship) ToLoad.Add(4);

            foreach (int i in ToLoad)
            {
                AssetReference shipPrefab = AmongUsClient.Instance.ShipPrefabs.ToArray()[i];
                AsyncOperationHandle<GameObject> shipAsset = shipPrefab.LoadAsset<GameObject>();
                while (!shipAsset.IsDone) yield return null;
                Maps[i] = shipAsset.Result.GetComponent<ShipStatus>();
                shipAsset.Result.DontUnload();
            }
        }
    }
}