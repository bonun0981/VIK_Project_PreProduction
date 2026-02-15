using UnityEngine;

public interface IMovement
{
    void Move();
    void SetDirection(Vector3 direction);
    void SetMode(IMovementMode currentMode);
}
