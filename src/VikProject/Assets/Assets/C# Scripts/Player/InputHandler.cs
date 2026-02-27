using UnityEngine;

public class InputHandler : MonoBehaviour
{
    
    public Vector2 moveInput { get; private set; }
    public bool isRun { get; private set; }
    public bool isAttackA { get; private set; }
    public bool isAttackB { get; private set; }
    public bool isSkill { get; private set; }
    public bool isUltimate { get; private set; }

    private void Awake()
    {
        
    }
    private void Update()
    {
       
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        isRun=Input.GetKey(KeyCode.LeftShift);

        isAttackA=Input.GetMouseButtonDown(0);
        isAttackB = Input.GetMouseButtonDown(1);
        isSkill = Input.GetKeyDown(KeyCode.Q);
        isUltimate = Input.GetKeyDown(KeyCode.E);
    }
}
