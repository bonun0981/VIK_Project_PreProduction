using UnityEngine;
using UnityEngine.Windows;

public class Rotation : MonoBehaviour
{
    InputHandler input;

    [SerializeField] float turnSpeed = 10f;
    [SerializeField] Transform cameraObject;
    private void Awake()
    {
        input = GetComponent<InputHandler>();
        cameraObject = Camera.main.transform;
    }
    private void Update()
    {
        FaceCamera();
    }
    public void FaceCamera()
    {
        Vector3 move = cameraObject.forward * input.moveInput.y + cameraObject.right * input.moveInput.x;
        move.y = 0;

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }
}
