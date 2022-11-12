using Hazel;

namespace Laboratory.Systems;

/// <summary>
/// Clone of ISystemType interface to ensure all members are implemented
/// </summary>
public interface ICustomSystemType
{
    bool IsDirty { get; }
    void Detoriorate(float deltaTime);
    void RepairDamage(PlayerControl player, byte amount);
    void UpdateSystem(PlayerControl player, MessageReader msgReader);
    void Serialize(MessageWriter writer, bool initialState);
    void Deserialize(MessageReader reader, bool initialState);
}