using System;
using System.Linq;
using Hazel;
using Laboratory.CustomMap;
using Reactor;
using UnhollowerRuntimeLib;
using UnityEngine;
using IntPtr = System.IntPtr;
using Object = Il2CppSystem.Object;

namespace Laboratory.Systems;

[RegisterInIl2Cpp(typeof(ISystemType))]
public class ProgressSystem : Object, ICustomSystemType
{
    private static ProgressSystem? _instance;
    public static ProgressSystem? Instance => ShipStatus.Instance ? _instance : null;
    public static CustomSystemType SystemType { get; } = CustomSystemType.Register<ProgressSystem>();
    public static float[] Stages { get; set; } = {
        60f,
        60f,
        60f,
        60f,
        60f,
        60f,
        60f,
        60f,
        60f,
        60f,
    };
    
    public ProgressSystem(IntPtr ptr) : base(ptr)
    {
    }

    public ProgressSystem() : base(ClassInjector.DerivedConstructorPointer<ProgressSystem>())
    {
        ClassInjector.DerivedConstructorBody(this);
        _instance = this;
    }

    public float Timer { get; set; }
    public bool IsDirty { get; set; }
    
    private bool _shouldRun = true;
    public bool ShouldRun
    {
        get => _shouldRun;
        set
        {
            _shouldRun = value;
            IsDirty = true;
        }
    }

    
    private float _totalTime = -1;
    public float TotalTime => _totalTime < 0 ? _totalTime = Stages.Sum() : _totalTime;

    private int _stage;
    public int Stage
    {
        get => _stage;
        set
        {
            _stage = value;
            IsDirty = true;
        }
    }
    
    public float Progress
    {
        get
        {
            int stageLength = Stages.Length;
            return Stage == stageLength ? 1 : Mathf.Clamp01((Timer - Stages.Take(Stage).Sum()) / (Stages[Stage] * stageLength) + (float) Stage / stageLength);
        }
    }
    
    public void Detoriorate(float deltaTime)
    {
        if (ShouldRun) Timer += deltaTime;
        Timer = Mathf.Clamp(Timer, 0, TotalTime);
        if (!AmongUsClient.Instance.AmHost) return;
        float sum = 0;
        int stage = 0;
        for (int i = 0; i < Stages.Length; i++)
        {
            sum += Stages[i];
            if (Timer >= sum) stage++;
        }

        if (Stage != stage) Stage = stage;
    }
    
    public void Serialize(MessageWriter writer, bool initialState)
    {
        writer.Write((byte)Stage);
        writer.Write(Timer);
        writer.Write(ShouldRun);
        IsDirty = false;
    }

    public void Deserialize(MessageReader reader, bool initialState)
    {
        Stage = reader.ReadByte();
        Timer = reader.ReadSingle();
        ShouldRun = reader.ReadBoolean();
    }

    public void RepairDamage(PlayerControl player, byte amount)
    {
        throw new InvalidOperationException();
    }

    public void UpdateSystem(PlayerControl player, MessageReader msgReader)
    {
        throw new InvalidOperationException();
    }
}