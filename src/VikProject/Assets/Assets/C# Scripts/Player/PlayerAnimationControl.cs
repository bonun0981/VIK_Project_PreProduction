using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    InputHandler input;

    [Header("Skill Ref")]
    [SerializeField] private PlayerSkill skill;
    [SerializeField] private PlayerSkill ultimate;

    [Header("Animation State")]
    [SerializeField] private string skillState = "Skill";
    [SerializeField] private string ultimateState = "Ultimate";
    [SerializeField] private float transition = 0.1f;

    static readonly int isRunningHash = Animator.StringToHash("isRunning");
    static readonly int velocityHash = Animator.StringToHash("Velocity");
    static readonly int skillTrigger = Animator.StringToHash("Skill");
    static readonly int ultimateTrigger = Animator.StringToHash("Ultimate");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        input = GetComponent<InputHandler>();
    }

    private void Update()
    {
        HandleMovementAnimation();
        HandleActionAnimation(); // 🔥 เพิ่มตรงนี้
    }

    // ================== MOVEMENT ==================
    private void HandleMovementAnimation()
    {
        Vector3 horizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        float speed = horizontalVelocity.magnitude;

        animator.SetFloat(velocityHash, speed);
        animator.SetBool(isRunningHash, input.isRun);
    }

    // ================== ACTION ==================
    private void HandleActionAnimation()
    {
        // 🔹 Skill (Q)
        if (input.isSkill && skill != null)
        {
            if (skill.CanActivate())
            {
                animator.CrossFade(skillState, transition);
            }
        }

        // 🔹 Ultimate (E)
        if (input.isUltimate && ultimate != null)
        {
            // 🔥 เช็ค resource เต็มเท่านั้น
            if (ultimate.CanActivate())
            {
                animator.CrossFade(ultimateState, transition);
            }
        }
    }
}