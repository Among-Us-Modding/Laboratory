using System;
using System.Collections.Generic;
using Hazel;
using Laboratory.CustomMap;
using Laboratory.Enums;
using Reactor;
using Reactor.Networking.MethodRpc;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using IntPtr = System.IntPtr;
using Object = Il2CppSystem.Object;

namespace Laboratory.Systems;

[RegisterInIl2Cpp(typeof(ISystemType))]
public class CounterSystem : Object, ICustomSystemType
{
    public static int GetCount(uint key)
    {
        Instance!.Counters.TryGetValue(key, out var res);
        return res;
    }

    [MethodRpc((uint)CustomRpcs.SetCount)]
    public static void SetCount(PlayerControl player, uint key, int value)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            Instance!.Counters[key] = value;
            Instance.IsDirty = true;
        }
    }

    [MethodRpc((uint)CustomRpcs.ChangeCount)]
    public static void ChangeCount(PlayerControl player, uint key, int change)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            SetCount(player, key, GetCount(key) + change);
        }
    }
    
    private static CounterSystem? _instance;
    public static CounterSystem? Instance => ShipStatus.Instance ? _instance : null;
    
    public static CustomSystemType SystemType { get; } = CustomSystemType.Register<CounterSystem>();

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
        var count = reader.ReadInt32();
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