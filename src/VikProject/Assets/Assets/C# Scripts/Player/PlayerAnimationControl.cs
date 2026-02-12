using UnityEngine;
using UnityEngine.Windows;

public class PlayerAnimationControl : MonoBehaviour
{

    public event System.Action OnAttack;
    Animator animator;
    IMovementState state;
    Attack attack;
    static readonly int attackHash = Animator.StringToHash("Attack");
    static readonly int isRunningHash = Animator.StringToHash("isRunning");
    static readonly int isWalkingHash = Animator.StringToHash("isWalking");
    static readonly int velocityHash = Animator.StringToHash("Velocity");
    private void Awake()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<IMovementState>();
        attack = GetComponent<Attack>();
        attack.OnAttackStarted += PlayAttack; // subscribe
    }
    void PlayAttack()
    {
        animator.SetTrigger(attackHash);
    }
    private void Update()
    {
        if (attack.isAttacking)
        {
            OnAttack?.Invoke();
        }
        animator.SetBool(isRunningHash, state.IsRunning);
        animator.SetBool(isWalkingHash, state.IsWalking);
        animator.SetFloat(velocityHash, state.GetSpeed);
    }



}
