using Jungle.Player.Managers;

namespace Jungle.Effects;

public interface IPlayerEffect : IEffect
{
    public PlayerManager Owner { get; set; }
}
