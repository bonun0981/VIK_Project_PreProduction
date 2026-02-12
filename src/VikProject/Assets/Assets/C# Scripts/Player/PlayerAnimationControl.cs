using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour
{
    Animator animator;
    IMovementState state;

    static readonly int isRunningHash = Animator.StringToHash("isRunning");
    static readonly int isWalkingHash = Animator.StringToHash("isWalking");
    static readonly int velocityHash = Animator.StringToHash("Velocity");
    private void Awake()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<IMovementState>();
    }
    private void Update()
    {
        animator.SetBool(isRunningHash, state.IsRunning);
        animator.SetBool(isWalkingHash, state.IsWalking);
        animator.SetFloat(velocityHash, state.GetSpeed);
    }



}
