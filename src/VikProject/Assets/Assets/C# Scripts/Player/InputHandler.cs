using UnityEngine;

public class InputHandler : MonoBehaviour
{
    
    public Vector2 moveInput { get; private set; }
    public bool isRun { get; private set; }
    public bool isAttack { get; private set; }

    private void Awake()
    {
        
    }
    private void Update()
    {
       
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        isRun=Input.GetKey(KeyCode.LeftShift);

        isAttack=Input.GetMouseButtonDown(0);
    }
}
