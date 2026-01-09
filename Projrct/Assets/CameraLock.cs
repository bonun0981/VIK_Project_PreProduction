using System.Net;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Rendering;


public class CameraLock : MonoBehaviour
{
   [SerializeField] CinemachineCamera freeLookCamera;
    [SerializeField] CinemachineCamera lockCamera;


    public float lockOnRange = 10f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float originalXAisSpeed;
    [SerializeField] float originalYAisSpeed;
    private Transform currentTarget;
    [SerializeField]private Transform lockTarget;
    private bool isLock;
    CinemachineInputAxisController playerInputController;
    CinemachineInputAxisController enemyInputController;






    public float lookHeight = 1.2f;
    public float followSpeed = 10f;

    private void Start()
    {
        playerInputController = freeLookCamera.GetComponent<CinemachineInputAxisController>();
        enemyInputController = lockCamera.GetComponent<CinemachineInputAxisController>();

    } 
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!isLock)
            {
                LockTarget();
            }
            else
            {
                UnlockTarget(); 
            }
        }
        if(isLock && currentTarget==null)
        {
            UnlockTarget();
        }

    }
    private void LateUpdate()
    {
        if (!isLock || currentTarget == null) return;
        
            Vector3 midPoint = (transform.position + currentTarget.position)*0.5f;
            midPoint.y += lookHeight;

            lockTarget.position = Vector3.Lerp(
            lockTarget.position,
            midPoint,
            Time.deltaTime * followSpeed);
        RotateAtEnemy();

    }
    public void LockTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position,lockOnRange,enemyLayer);
        if (enemies.Length == 0) return;
        
        Debug.Log("Locking Target: " + enemies[0].name);
        currentTarget = enemies[0].transform;
        lockCamera.LookAt = lockTarget;
        playerInputController.enabled = false;
        enemyInputController.enabled = false;
        isLock = true;
        freeLookCamera.Priority = 0;
        lockCamera.Priority = 10;

    }

    public void UnlockTarget()
    {
        freeLookCamera.LookAt = transform;
        playerInputController.enabled = true;
        enemyInputController.enabled = true;
        currentTarget = null;
        isLock = false;
        freeLookCamera.Priority = 10;
        lockCamera.Priority = 0;
    }
    public void RotateAtEnemy()
    {
        if (!isLock || currentTarget == null) return;

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 12f
        );
    }
}
