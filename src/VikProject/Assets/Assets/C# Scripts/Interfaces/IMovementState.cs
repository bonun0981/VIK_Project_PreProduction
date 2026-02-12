using UnityEngine;

public interface IMovementState
{
    bool IsRunning { get; }
    bool IsWalking { get; }
    float GetSpeed { get; }

}
public interface IDirectionMovement
{
    bool IsMoveForward { get; }
    bool IsMoveBackward { get; }
    bool IsMoveLeft { get; }
    bool IsMoveRight { get; }

}
public interface ICombatState
{
    string IsAttacked { get; }
    bool IsDead { get; }
    bool IsTakingDamage { get; }
}


public interface IReactState
{
    bool IsKnockBack { get; }
    bool IsLift { get; }
    bool IsKnockDown { get; }
}




