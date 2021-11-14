using Laboratory.Player.Managers;

namespace Laboratory.Effects;

public interface IPlayerEffect : IEffect
{
    public PlayerManager Owner { get; set; }
}
