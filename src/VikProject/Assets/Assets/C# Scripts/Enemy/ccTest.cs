using UnityEngine;
using System.Collections;

public class ccTest : MonoBehaviour,IKnockbackable
{
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.25f;
    

    private Rigidbody rb;

    private bool isKnockedBack;

    // ===== Interface Properties =====
    public bool IsKnockBack => isKnockedBack;

    public bool IsMovementInterrupt => isKnockedBack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Knockback(float knockback)
    {
        if (isKnockedBack) return;

        StartCoroutine(KnockbackRoutine(knockback));
    }

    private IEnumerator KnockbackRoutine(float force)
    {
        isKnockedBack = true;

        // 🔥 ทิศถอยหลัง (จากด้านหน้า)
        Vector3 direction = -transform.forward;
        direction.y = 0f;

        Vector3 finalForce =
            direction.normalized * force;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(finalForce, ForceMode.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
    }


}
