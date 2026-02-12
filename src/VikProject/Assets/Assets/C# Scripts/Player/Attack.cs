using UnityEngine;

public class Attack : MonoBehaviour,IMovementInterrupt,ICombatState
{
    public event System.Action OnAttackStarted;
    InputHandler input;
    public bool isAttacking;
    public bool IsMovementInterrupted => isAttacking;

    public string IsAttacked => "Attack";

    public bool IsDead => throw new System.NotImplementedException();

    public bool IsTakingDamage => throw new System.NotImplementedException();

    private void Awake()
    {
        input = GetComponent<InputHandler>();
    }

    private void Update()
    {
        if (input.isAttack && !isAttacking)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        OnAttackStarted?.Invoke(); // 🔥 ยิงครั้งเดียวตรงนี้
    }

    public void EndAttack()
    {
        isAttacking = false;
    }
}
