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
            public readonly Type EffectType;
            public readonly PlayerControl TargetPlayer;
            
            public EffectInfo(PlayerControl targetPlayer, IEffect effect, bool primary)
            {
                Primary = primary;
                EffectType = effect?.GetType();
                FullName = effect == null ? "" : EffectType.FullName;
                TargetPlayer = targetPlayer;
            }

            public EffectInfo(PlayerControl targetPlayer, string fullName, bool primary)
            {
                Primary = primary;
                FullName = fullName;
                EffectType = fullName == "" ? null : Type.GetType(fullName);
                if (EffectType is null) throw new Exception($"Bad IPlayerEffect Type Null: {EffectType}");
                TargetPlayer = targetPlayer;
            }
        }

        public override RpcLocalHandling LocalHandling => RpcLocalHandling.After;

        public override void Write(MessageWriter writer, EffectInfo effectInfo)
        {
            if (effectInfo.TargetPlayer)
            {
                writer.Write(true);
                writer.WriteNetObject(effectInfo.TargetPlayer);
            }
            else writer.Write(false);
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
            var effect = (IEffect) Activator.CreateInstance(effectInfo.EffectType);
            if (effectInfo.TargetPlayer)
            {
                effectInfo.TargetPlayer.GetComponent<PlayerEffectManager>().AddEffect(effect, effectInfo.Primary);
            }
            else
            {
                GlobalEffectManager.Instance.AddEffect(effect, effectInfo.Primary);
            }
        }
    }
}