using UnityEngine;

public class MovementChecker : MonoBehaviour
{
    IMovementInterrupt[] interrupts;

    private void Awake()
    {
        interrupts = GetComponents<IMovementInterrupt>();
    }
    public bool CanMove()//chack if any interrupt is active
    {
        
        foreach (var I in interrupts)
        {
            if (I.IsMovementInterrupted)
            {
                return false;
            }
        }
        return true;
    }
}
