using UnityEngine;

public class InputHandler : MonoBehaviour
{
    MovementChecker movementChecker;
    public Vector2 moveInput { get; private set; }
    public bool isRun { get; private set; }
    public bool isAttack { get; private set; }

    private void Awake()
    {
        movementChecker = GetComponent<MovementChecker>();
    }
    private void Update()
    {
        if (movementChecker.CanMove())
        {
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
            
        isRun=Input.GetKey(KeyCode.LeftShift);
        isAttack=Input.GetMouseButtonDown(0);
    }
}
