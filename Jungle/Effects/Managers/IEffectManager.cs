using System.Collections.Generic;

namespace Jungle.Effects.Managers;

public interface IEffectManager
{
    List<IEffect> Effects { get; }

    void AddEffect(IEffect effect, bool primary);
    void RemoveEffect(IEffect effect);
    void ClearEffects();
}
