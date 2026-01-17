using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator playerController;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform camTransform;

    [Header("Movement")]
    [SerializeField] private float originalwalkSpeed = 2f;
    private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float turnSpeed = 10f; // rotation speed
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    private float inputX;
    private float inputY;

    private Vector3 velocity; // vertical velocity

    void Start()
    {
       

        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        InputManager();
        MovementMode();
        GroundMovement();
        Jump();
        RotateCharacter();
    }

    void InputManager()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        
        

           
    }

    void GroundMovement()
    {
        // Camera relative movement
        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * inputY + camRight * inputX;
        move.Normalize(); // consistent diagonal speed

        // Move character
        characterController.Move(move * walkSpeed * Time.deltaTime);
        playerController.SetFloat("Velocity",Mathf.Abs(move.magnitude));
        // Gravity
        if (characterController.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void RotateCharacter()
    {
        // Only rotate if there is movement input
        Vector3 move = camTransform.forward * inputY + camTransform.right * inputX;
        move.y = 0;

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

       
    }

    public void MovementMode()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerController.SetBool("isRunning", true);
            walkSpeed = runSpeed;
        }
        else
        {
            playerController.SetBool("isRunning", false); 
            walkSpeed = originalwalkSpeed;
        }
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&characterController.isGrounded)
        {
            playerController.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (characterController.isGrounded == false)
        {
            playerController.SetBool("isFalling", true);
        }
        else if (characterController.isGrounded == true)
        {
           // playerController.SetBool("isFalling", false);
        }


    }
}
