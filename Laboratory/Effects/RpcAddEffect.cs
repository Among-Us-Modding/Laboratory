using System;
using Hazel;
using InnerNet;
using Laboratory.Effects.Managers;
using Laboratory.Enums;
using Laboratory.Extensions;
using Reactor;
using Reactor.Networking;

namespace Laboratory.Effects;

[RegisterCustomRpc((uint)CustomRpcs.RpcAddEffect)]
public class RpcAddEffect : PlayerCustomRpc<LaboratoryPlugin, RpcAddEffect.EffectInfo>
{
    public RpcAddEffect(LaboratoryPlugin plugin, uint id) : base(plugin, id)
    {
    }

    public readonly struct EffectInfo
    {
        public readonly IEffectManager EffectManager;
        public readonly IEffect Effect;
        public readonly bool IsPrimary;

        public EffectInfo(IEffectManager effectManager, IEffect effect, bool isPrimary)
        {
            EffectManager = effectManager;
            Effect = effect;
            IsPrimary = isPrimary;
        }
    }

    public override RpcLocalHandling LocalHandling => RpcLocalHandling.After;

    public override void Write(MessageWriter writer, EffectInfo effectInfo)
    {
        switch (effectInfo.EffectManager)
        {
            case GlobalEffectManager:
                writer.Write((byte)0);
                break;

            case PlayerEffectManager playerEffectManager:
                writer.Write((byte)1);
                writer.WriteNetObject(playerEffectManager.Manager.Player);
                break;
        }

        writer.Write(effectInfo.Effect.GetType().AssemblyQualifiedName);
        writer.Write(effectInfo.IsPrimary);
    }

    public override EffectInfo Read(MessageReader reader)
    {
        IEffectManager effectManager = reader.ReadByte() switch
        {
            0 => GlobalEffectManager.Instance!,
            1 => reader.ReadNetObject<PlayerControl>().GetEffectManager(),
            _ => throw new ArgumentOutOfRangeException(),
        };

        var effect = (IEffect)Activator.CreateInstance(Type.GetType(reader.ReadString())!);

        var isPrimary = reader.ReadBoolean();

        return new EffectInfo(effectManager, effect, isPrimary);
    }

    public override void Handle(PlayerControl innerNetObject, EffectInfo effectInfo)
    {
        effectInfo.EffectManager.AddEffect(effectInfo.Effect, effectInfo.IsPrimary);
    }
}
