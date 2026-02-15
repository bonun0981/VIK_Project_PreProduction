using UnityEngine;

public class PlayerAttack : MonoBehaviour,IMovementInterrupt
{
    
    
    private bool isAttacking = false;
    public bool IsMovementInterrupt => isAttacking;

    
    public void StartAttack()
    {
        isAttacking = true;
        
    }
    public void EndAttack()
    {
        
        isAttacking = false;
        
    }
}
