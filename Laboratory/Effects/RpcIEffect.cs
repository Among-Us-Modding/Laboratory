using System;
using Hazel;
using InnerNet;
using Laboratory.Effects.Interfaces;
using Laboratory.Effects.MonoBehaviours;
using Laboratory.Enums;
using Reactor;
using Reactor.Networking;

namespace Laboratory.Effects;

[RegisterCustomRpc((uint)CustomRpcs.RpcIEffect)]
public class RpcIEffect : PlayerCustomRpc<LaboratoryPlugin, RpcIEffect.EffectInfo>
{
    public RpcIEffect(LaboratoryPlugin plugin, uint id) : base(plugin, id)
    {
    }

    public readonly struct EffectInfo
    {
        public readonly string FullName;
        public readonly bool Primary;
        public readonly Type? EffectType;
        public readonly PlayerControl? TargetPlayer;
        public readonly IEffect? Effect;

        public EffectInfo(PlayerControl? targetPlayer, IEffect? effect, bool primary)
        {
            Primary = primary;
            EffectType = effect?.GetType();
            FullName = (effect == null || EffectType == null ? "" : EffectType.AssemblyQualifiedName) ?? string.Empty;
            TargetPlayer = targetPlayer;
            Effect = effect;
        }

        public EffectInfo(PlayerControl? targetPlayer, string fullName, bool primary)
        {
            Primary = primary;
            FullName = fullName;
            EffectType = fullName == "" ? null : Type.GetType(fullName);
            if (EffectType is null) throw new Exception($"Bad IPlayerEffect Type Null: {EffectType}");
            TargetPlayer = targetPlayer;
            Effect = null;
        }
    }

    public override RpcLocalHandling LocalHandling => RpcLocalHandling.After;

    public override void Write(MessageWriter writer, EffectInfo effectInfo)
    {
        writer.Write(effectInfo.TargetPlayer != null);
        if (effectInfo.TargetPlayer != null)
            writer.WriteNetObject(effectInfo.TargetPlayer);
        writer.Write(effectInfo.Primary);
        writer.Write(effectInfo.FullName);
    }

    public override EffectInfo Read(MessageReader reader)
    {
        var player = reader.ReadBoolean() ? reader.ReadNetObject<PlayerControl>() : null;
        var primary = reader.ReadBoolean();
        return new EffectInfo(player, reader.ReadString(), primary);
    }

    public override void Handle(PlayerControl innerNetObject, EffectInfo effectInfo)
    {
        var effect = effectInfo.Effect ?? (effectInfo.EffectType == null ? null : (IEffect) Activator.CreateInstance(effectInfo.EffectType));
        if (effectInfo.TargetPlayer != null)
        {
            effectInfo.TargetPlayer.GetComponent<PlayerEffectManager>().AddEffect(effect, effectInfo.Primary);
        }
        else
        {
            if (GlobalEffectManager.Instance != null) GlobalEffectManager.Instance.AddEffect(effect, effectInfo.Primary);
        }
    }
}