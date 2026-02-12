using UnityEngine;

public class Attack : MonoBehaviour,IMovementInterrupt
{
    public bool isAttacking;
    public bool IsMovementInterrupted => isAttacking;
}
