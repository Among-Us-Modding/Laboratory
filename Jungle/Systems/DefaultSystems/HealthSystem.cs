using System;
using System.Collections.Generic;
using Hazel;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Jungle.Enums;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using IntPtr = System.IntPtr;
using Math = System.Math;
using Object = Il2CppSystem.Object;

namespace Jungle.Systems.DefaultSystems;

/// <summary>
/// Generic system to manage player healths
/// </summary>
[RegisterInIl2Cpp(typeof(ISystemType))]
public class HealthSystem : Object, ICustomSystemType
{
    public static CustomSystemType SystemType { get; } = CustomSystemType.Register<HealthSystem>();

    /// <summary>
    /// The starting and maximum health of each player
    /// </summary>
    public static int MaxHealth { get; set; } = 100;

    /// <summary>
    /// Set if the players name should be updated when their health changes
    /// </summary>
    public static bool UpdateNameText { get; set; } = true;
    
    /// <summary>
    /// Should a player be killed when their health hits zero
    /// </summary>
    public static bool KillWhenNoHealth { get; set; } = true;
    
    /// <summary>
    /// Action invoked when a player's health is changed with their old and new health values
    /// </summary>
    public static Action<PlayerControl, (int previous, int current)> OnHealthChange { get; set; }

    /// <summary>
    /// The current instance of the health system
    /// </summary>
    public static HealthSystem Instance => ShipStatus.Instance ? _instance : null;

    private static HealthSystem _instance;

    /// <summary>
    /// Changes the damage of a specified player
    /// </summary>
    /// <param name="player">The player to change the health of</param>
    /// <param name="change">The amount of health to be added to a player (can be negative)</param>
    [MethodRpc((uint)CustomRPCs.ChangeHealth)]
    public static void CmdChangeHealth(PlayerControl player, int change)
    {
        if (Instance is not null)
        {
            byte pid = player.PlayerId;
            Instance.SetHealth(pid, Instance.GetHealth(pid) + change);
        }
    }

    public HealthSystem(IntPtr ptr) : base(ptr)
    {
    }

    public HealthSystem() : base(ClassInjector.DerivedConstructorPointer<HealthSystem>())
    {
        ClassInjector.DerivedConstructorBody(this);
        _instance = this;

        foreach (PlayerControl playerInfo in PlayerControl.AllPlayerControls)
        {
            SetHealth(playerInfo.PlayerId, GetMaxHealth(playerInfo.PlayerId));
        }
    }

    public int GetMaxHealth(byte playerId) => MaxHealth;

    [HideFromIl2Cpp]
    internal Dictionary<byte, int> PlayerHealths { get; } = new();

    /// <summary>
    /// Sets the health of a player and updated their name
    /// </summary>
    /// <param name="playerId">The player id of the player being changed</param>
    /// <param name="newHealth">The new amount of heath to set the player with</param>
    public void SetHealth(byte playerId, int newHealth)
    {
        GameData.PlayerInfo data = GameData.Instance.GetPlayerById(playerId);
        int oldHealth = GetHealth(playerId);
        int playerHealth = PlayerHealths[playerId] = Math.Clamp(newHealth, 0, GetMaxHealth(playerId));
        IsDirty = true;

        if (data != null && data.Object)
        {
            PlayerControl player = data.Object;
            OnHealthChange?.Invoke(player, (oldHealth, newHealth));
            if (UpdateNameText) UpdateHealthText(player, data, playerHealth);

            if (!data.Role.IsImpostor && AmongUsClient.Instance.AmHost && playerHealth <= 0)
            {
                if (KillWhenNoHealth) throw new NotImplementedException(); // player.RpcCustomMurder(player, true);
            }
        }
    }

    public void UpdateHealthText(PlayerControl player, GameData.PlayerInfo data, int playerHealth)
    {
        player.cosmetics.nameText.text = data.Role.IsImpostor 
            ? data.PlayerName 
            : $"<color=#{Palette.PlayerColors[(data.DefaultOutfit.ColorId + Palette.PlayerColors.Length) % Palette.PlayerColors.Length].ToHtmlStringRGBA()}>{playerHealth}</color>\n{data.PlayerName}";
    }

    /// <summary>
    /// Get the current health of a player
    /// </summary>
    /// <param name="playerId">The player id of the player being checked</param>
    /// <returns></returns>
    public int GetHealth(byte playerId)
    {
        if (!PlayerHealths.ContainsKey(playerId)) return PlayerHealths[playerId] = GetMaxHealth(playerId);
        return PlayerHealths[playerId];
    }

    #region ISystemType Implementation

    public bool IsDirty { get; set; } = true;

    public void Deserialize(MessageReader reader, bool initialState)
    {
        byte length = reader.ReadByte();
        for (int i = 0; i < length; i++)
        {
            bool dirty = IsDirty;
            SetHealth(reader.ReadByte(), reader.ReadInt32());
            IsDirty = dirty;
        }
    }

    public void Serialize(MessageWriter writer, bool initialState)
    {
        writer.Write((byte)PlayerHealths.Count);
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
        throw new NotSupportedException();
    }

    public void RepairDamage(PlayerControl player, byte amount)
    {
        throw new NotSupportedException();
    }

    #endregion
}
