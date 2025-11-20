using Unity.VisualScripting;
using UnityEngine;

public class Syetem_Test : MonoBehaviour
{
    private CharacterController test;
    [SerializeField]private Animator animator;
    void Start()
    {
        
        test = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (test.collisionFlags == CollisionFlags.Below)
        {
            Debug.Log("On the ground");
        }

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Attack");
        }
    }
}
