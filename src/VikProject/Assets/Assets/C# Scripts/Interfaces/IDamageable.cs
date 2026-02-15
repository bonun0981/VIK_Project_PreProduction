using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
    bool IsDead { get; }
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
