using System;
using System.Collections.Generic;
using Hazel;
using Laboratory.Mods.Enums;
using Reactor;
using Reactor.Networking.MethodRpc;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;

namespace Laboratory.Mods.Systems
{
    /// <summary>
    /// Generic system to manage player healths
    /// </summary>
    [RegisterInIl2Cpp(typeof(ISystemType))]
    public class HealthSystem : Il2CppSystem.Object, ICustomSystemType
    {
        /// <summary>
        /// The starting and maximum health of each player
        /// </summary>
        public static int MaxHealth { get; set; } = 100;

        /// <summary>
        /// The current instance of hte health system
        /// </summary>
        public static HealthSystem Instance { get; set; }

        /// <summary>
        /// Changes the damage of a specified player
        /// </summary>
        /// <param name="player">The player to change the health of</param>
        /// <param name="change">The amount of health to be added to a player (can be negative)</param>
        [MethodRpc(MethodRpcIds.ChangeHealth)]
        public static void RpcChangeDamage(PlayerControl player, int change)
        {
            if (AmongUsClient.Instance.AmHost && Instance is not null)
            {
                var pid = player.PlayerId;
                Instance.SetHealth(pid, Instance.GetHealth(pid) + change);
            }
        }
        
        public HealthSystem(IntPtr ptr) : base(ptr) { }
        
        public HealthSystem() : base(ClassInjector.DerivedConstructorPointer<HealthSystem>())
        {
            ClassInjector.DerivedConstructorBody(this);
            Instance = this;
        }

        internal Dictionary<byte, int> PlayerHealths { [HideFromIl2Cpp] get; } = new();

        /// <summary>
        /// Sets the health of a player
        /// </summary>
        /// <param name="playerId">The player id of the player being changed</param>
        /// <param name="newHealth">The new amount of heath to set the player with</param>
        public void SetHealth(byte playerId, int newHealth)
        {
            PlayerHealths[playerId] = Math.Clamp(newHealth, 0, MaxHealth);
            IsDirty = true;
        }

        /// <summary>
        /// Get the current health of a player
        /// </summary>
        /// <param name="playerId">The player id of the player being checked</param>
        /// <returns></returns>
        public int GetHealth(byte playerId)
        {
            if (!PlayerHealths.ContainsKey(playerId)) return PlayerHealths[playerId] = MaxHealth;
            return PlayerHealths[playerId];
        }

        #region ISystemType Implementation

        public bool IsDirty { get; set; } = true;

        public void Deserialize(MessageReader reader, bool initialState)
        {
            byte length = reader.ReadByte();
            for (int i = 0; i < length; i++)
            {
                PlayerHealths[reader.ReadByte()] = reader.ReadInt32();
            }
        }

        public void Serialize(MessageWriter writer, bool initialState)
        {
            if (initialState)
            {
                foreach (GameData.PlayerInfo playerInfo in GameData.Instance.AllPlayers)
                {
                    if (!PlayerHealths.ContainsKey(playerInfo.PlayerId)) PlayerHealths[playerInfo.PlayerId] = MaxHealth;
                }
            }

            writer.Write((byte) PlayerHealths.Count);
            foreach ((byte playerId, int health) in PlayerHealths)
            {
                writer.Write(playerId);
                writer.Write(health);
            }

            IsDirty = false;
        }

        public void Detoriorate(float deltaTime)
        {
        }
        
        public void UpdateSystem(PlayerControl player, MessageReader msgReader)
        {
            throw new NotImplementedException();
        }

        public void RepairDamage(PlayerControl player, byte amount)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}