using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class Movement : MonoBehaviour
{
    

    [Header("Adjust Speed")]
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float currentSpeed ;


    //velocity movment
    private Vector3 direction;
    //component
    [SerializeField] Transform cameraTransform;
    Rigidbody rb;
    MovementChecker checker;
    InputHandler input;
    //interfaces
    

    private void Awake()
    {   currentSpeed = walkSpeed;
        rb = GetComponent<Rigidbody>();
        checker = GetComponent<MovementChecker>();
        input = GetComponent<InputHandler>();
        cameraTransform= Camera.main.transform;

    }
    private void Update()
    {
        Vector3 camFoward=cameraTransform.forward;
        Vector3 camRight=cameraTransform.right;
        camFoward.y = 0;
        camRight.y = 0;
        camFoward.Normalize();
        camRight.Normalize();
        direction= camFoward * input.moveInput.y + camRight * input.moveInput.x;
        direction.Normalize();

    }
    private void FixedUpdate()
    {
        if(!checker.CanMove())
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;

        }
        currentSpeed= input.isRun ? runSpeed : walkSpeed;
        Vector3 targetVelocity = direction * currentSpeed;

        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

    }


}
