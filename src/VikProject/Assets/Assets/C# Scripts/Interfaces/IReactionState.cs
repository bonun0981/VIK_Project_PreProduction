using UnityEngine;

public interface IReactionState
{
    bool TakeHit { get; }
    bool Die { get; }
    bool Lift { get; }
}
