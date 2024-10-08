using System;
using System.Collections.Generic;
using Hazel;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Laboratory.Config;
using Laboratory.Enums;
using Laboratory.Map;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Attributes;
using IntPtr = System.IntPtr;
using Object = Il2CppSystem.Object;

namespace Laboratory.Systems.DefaultSystems;

[RegisterInIl2Cpp(typeof(ISystemType))]
public class CounterSystem : Object, ICustomSystemType
{
    public static int GetCount(uint key)
    {
        Instance!.Counters.TryGetValue(key, out int res);
        return res;
    }

    [MethodRpc((uint)CustomRPCs.SetCount)]
    public static void SetCount(PlayerControl player, uint key, int value)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            Instance!.Counters[key] = value;
            Instance.IsDirty = true;
        }
    }

    [MethodRpc((uint)CustomRPCs.ChangeCount)]
    public static void ChangeCount(PlayerControl player, uint key, int change)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            SetCount(player, key, GetCount(key) + change);
        }
    }

    private static CounterSystem _instance;
    public static CounterSystem Instance => ShipStatus.Instance ? _instance : null;

    public CounterSystem(IntPtr ptr) : base(ptr)
    {
    }

    public CounterSystem() : base(ClassInjector.DerivedConstructorPointer<CounterSystem>())
    {
        ClassInjector.DerivedConstructorBody(this);
        _instance = this;
    }

    public bool IsDirty { get; set; }

    [HideFromIl2Cpp]
    public Dictionary<uint, int> Counters { get; } = new();

    public void Serialize(MessageWriter writer, bool initialState)
    {
        writer.Write(Counters.Count);
        foreach ((uint key, int value) in Counters)
        {
            writer.Write(key);
            writer.Write(value);
        }

        IsDirty = false;
    }

    public void Deserialize(MessageReader reader, bool initialState)
    {
        Counters.Clear();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            Counters[reader.ReadUInt32()] = reader.ReadInt32();
        }
    }

    public void Detoriorate(float deltaTime) { }

    public void RepairDamage(PlayerControl player, byte amount)
    {
        throw new InvalidOperationException();
    }

    public void UpdateSystem(PlayerControl player, MessageReader msgReader)
    {
        throw new InvalidOperationException();
    }
}
