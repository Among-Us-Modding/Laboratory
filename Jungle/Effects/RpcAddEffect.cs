using System;
using Hazel;
using InnerNet;
using Jungle.Effects.Managers;
using Jungle.Enums;
using Jungle.Player.Extensions;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;

namespace Jungle.Effects;

[RegisterCustomRpc((uint)CustomRPCs.AddEffect)]
public class RpcAddEffect : PlayerCustomRpc<JunglePlugin, RpcAddEffect.EffectInfo>
{
    public RpcAddEffect(JunglePlugin plugin, uint id) : base(plugin, id)
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

        writer.Write(effectInfo.Effect is null ? "null" : effectInfo.Effect.GetType().AssemblyQualifiedName);
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

        string effectName = reader.ReadString();
        IEffect effect = null;
        if (effectName != "null") effect = (IEffect) Activator.CreateInstance(Type.GetType(effectName)!);

        bool isPrimary = reader.ReadBoolean();

        return new EffectInfo(effectManager, effect, isPrimary);
    }

    public override void Handle(PlayerControl innerNetObject, EffectInfo effectInfo)
    {
        effectInfo.EffectManager.AddEffect(effectInfo.Effect, effectInfo.IsPrimary);
    }
}
