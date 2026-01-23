using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]Animator attackAnimation;
    [SerializeField]BoxCollider hitBox;

    [SerializeField] private float attackAssiteRadius = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    public float castDistance = 10f;
    [SerializeField] LayerMask enemyLayer;
    public bool isAttacking = false;
    private Transform currentTarget;
    [SerializeField]private bool targetLocked = false;
    [SerializeField] private float targetGraceTime = 0.6f; // seconds
    [SerializeField] private float breakTargetDistance = 4f;

    [SerializeField]private float targetGraceTimer = 0.6f;
    CharacterController controller;
    private Vector3 pendingPush;
    [SerializeField] private float enemyPushStrength = 0.08f;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] attackClips;

    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    [SerializeField] private Vector2 volumeRange = new Vector2(0.9f, 1f);


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            attackAnimation.SetTrigger("Attack");
            
        }
        if (isAttacking && currentTarget != null)
        {
            RotateToEnemy(currentTarget);
        }

        HandleTargetGrace();
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }


    private void Start()
    {
          attackAnimation= GetComponent<Animator>();
    }
    public void FirstAttack()
    {
       
        attackAnimation.SetInteger("Combo", 1);
    }

    public void SecondAttack()
    {
        
        attackAnimation.SetInteger("Combo", 2);
    }

    public void ThirdAttack()
    {
        
        attackAnimation.SetInteger("Combo", 3);
    }

    public void ResetAttack()
    {
        attackAnimation.SetInteger("Combo", 0);
        attackAnimation.SetBool("CanAttack", false);

        isAttacking = false;

        // Start grace period instead of clearing instantly
        targetGraceTimer = targetGraceTime;
    }
    public void StartAttack ()
    {
        isAttacking = true;
        attackAnimation.SetBool("CanAttack", true);

        // Only pick a new target if lock was released
        if (!targetLocked)
        {
            EnemyCheck();
            targetLocked = currentTarget != null;
        }
        attackAnimation.deltaPosition.Set(0, 0, 0);
    }

    public void EnableHitBox()
    {
        hitBox.enabled = true;
    }
    public void DisableHitBox()
    {
        hitBox.enabled = false;
    }


    public void DebugTest()
    {
        Debug.Log("Animaiton event has triggers");
    }
    public void DebugTestend()
    {
        Debug.Log("second Animaiton event has triggers ");
    }

    public void EnemyCheck()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
        transform.position,
        attackAssiteRadius,
        transform.forward,
        castDistance,
        enemyLayer
    );

        if (hits.Length == 0)
        {
            currentTarget = null;
            return;
        }

        RaycastHit closestHit = hits[0];
        float closestDistance = hits[0].distance;

        foreach (RaycastHit hit in hits)
        {
            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestHit = hit;
            }
        }

        currentTarget = closestHit.transform;
        Debug.Log("Locked onto: " + currentTarget.name);
    }

    public void RotateToEnemy(Transform enemy)
    {
        Vector3 direction = (enemy.position - transform.position).normalized;
        direction.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position + transform.forward * castDistance,
            attackAssiteRadius
        );
    }

    private void HandleTargetGrace()
    {
        if (!targetLocked || currentTarget == null) return;

        // If attacking again, keep lock
        if (isAttacking)
        {
            targetGraceTimer = targetGraceTime;
            return;
        }

        // Countdown grace timer
        targetGraceTimer -= Time.deltaTime;

        // Break lock if player walks away
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance > breakTargetDistance)
        {
            ClearTargetLock();
            return;
        }

        // Time expired → unlock
        if (targetGraceTimer <= 0f)
        {
            ClearTargetLock();
        }


    }

    private void ClearTargetLock()
    {
        targetLocked = false;
        currentTarget = null;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            Vector3 horizontalNormal = hit.normal;
            horizontalNormal.y = 0f;

            pendingPush += horizontalNormal.normalized * enemyPushStrength;
        }
    }

    public void PlayAttackSFX()
    {
        if (attackClips.Length == 0) return;

        AudioClip clip = attackClips[Random.Range(0, attackClips.Length)];

        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);

        audioSource.PlayOneShot(clip);
    }
}
