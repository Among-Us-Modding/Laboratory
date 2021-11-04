using System;
using Hazel;
using InnerNet;
using Laboratory.Mods.Effects.Interfaces;
using Laboratory.Mods.Effects.MonoBehaviours;
using Laboratory.Mods.Enums;
using Reactor;
using Reactor.Networking;

namespace Laboratory.Mods.Effects
{
    [RegisterCustomRpc(CustomRpcIds.RpcIEffect)]
    public class RpcIEffect : PlayerCustomRpc<ModPlugin, RpcIEffect.EffectInfo>
    {
        public RpcIEffect(ModPlugin plugin, uint id) : base(plugin, id)
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
                FullName = (effect == null || EffectType == null ? "" : EffectType.FullName) ?? string.Empty;
                TargetPlayer = targetPlayer;
                Effect = effect;
            }

            public EffectInfo(PlayerControl targetPlayer, string fullName, bool primary)
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
            writer.Write((InnerNetObject?) effectInfo.TargetPlayer);
            writer.Write(effectInfo.Primary);
            writer.Write(effectInfo.FullName);
        }

        public override EffectInfo Read(MessageReader reader)
        {
            var player = MessageExtensions.ReadNetObject<PlayerControl>(reader);
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
}