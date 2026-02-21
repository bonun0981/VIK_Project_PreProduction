using UnityEngine;
using System;

public interface IDamageable
{
    event Action<float> OnDamaged;
    bool IsDead { get; }
    void TakeDamage(float damage);
}
public interface IKnockbackable:IMovementInterrupt
{
    void Knockback(float knockback);
    bool IsKnockBack { get; }
}
public interface IStunnable:IMovementInterrupt
{
    void Stun(float stunDuration);
    bool IsStunned { get; }
}
public interface IKnockUpable : IMovementInterrupt
{
    void KnockUp(float knockUpForce);
    bool IsKnockUp { get; }
}
