using UnityEngine;
using UnityEngine.Windows;

public class PlayerAnimationControl : MonoBehaviour
{

   
    Animator animator;
    
    Rigidbody rb;
    InputHandler input;

    static readonly int attackHash = Animator.StringToHash("Attack");
    static readonly int isRunningHash = Animator.StringToHash("isRunning");
    static readonly int velocityHash = Animator.StringToHash("Velocity");


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        input = GetComponent<InputHandler>();
    }
    void PlayAttack()
    {
        animator.SetTrigger(attackHash);
    }
    private void Update()
    {
       
       if(input.isAttack)
        {
            PlayAttack();
            
        }
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        animator.SetFloat(velocityHash, speed);
        animator.SetBool(isRunningHash, input.isRun);
       
        
    }



}
