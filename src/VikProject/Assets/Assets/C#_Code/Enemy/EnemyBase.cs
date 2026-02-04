using UnityEditor.Rendering;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        SeekingPassive,
        PassivePositioning, // ⭐ NEW
        Passive,
        SeekingActive,
        Active,
        Recover
    }
    public EnemyState state;
    public Transform player;
    Color passiveColor = Color.yellow;
    Color activeColor = Color.red;
    Color idleColor = Color.white;
    Color recoverColor = Color.cyan;
    Renderer[] renderers;
    MaterialPropertyBlock mpb;
    public bool aiActive;
    float thinkTimer;
    Vector3 idleTarget;
    float idleTimer;
    float circleAngle;
    //float circleRadius;
    bool attacking;
    float attackCooldown;
    Animator animator;
    Vector3 velocity;
    float attackTimeout;
    float activeDelay;
    bool readyToAttack;
    float preAttackDelay;
    bool waitingToAttack;
    int circleDir;              // -1 = left, +1 = right
    float circleSwitchTimer;    // นับเวลาจนเปลี่ยนทิศ
    //float circleSpeed;
    float desiredOrbitAngle;   // where this enemy SHOULD be on the circle
    //float orbitAngleSpeed;     // how fast it corrects
    float circleSpeed     = 0.6f;   // slow circling
float orbitAngleSpeed = 0.8f;   // gentle correction
    float orbitFlow;
    float circleRadius    = 3.2f;
    float orbitFlowAngle;
    [SerializeField] bool debugStateColor = true;
    float localOrbitOffset;
    // tangential speed (slow)
    private void Start()
    {
        localOrbitOffset = Random.Range(0f, 360f);
        renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
        orbitFlowAngle = Random.Range(0f, 360f);
        circleRadius = Random.Range(2.8f, 4.2f);
        circleAngle = Random.Range(0f, 360f);

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        animator = GetComponent<Animator>();
        circleDir = Random.value > 0.5f ? 1 : -1;
        circleSwitchTimer = Random.Range(2f, 5f);
        circleSpeed = Random.Range(0.8f, 1.4f);
    }
    void ApplyDebugColor(Color c)
    {
        if (!debugStateColor) return;

        mpb.SetColor("_BaseColor", c); // URP/Lit
                                       // mpb.SetColor("_Color", c);  // Built-in fallback

        foreach (var r in renderers)
            r.SetPropertyBlock(mpb);
    }
    void Update()
    {
        if (!aiActive)
            return;

        thinkTimer -= Time.deltaTime;
        if (thinkTimer <= 0f)
        {
            thinkTimer = Random.Range(0.4f, 0.8f);
            Think();
        }
        circleSwitchTimer -= Time.deltaTime;
        if (circleSwitchTimer <= 0f)
        {
            circleDir *= -1;
            circleSwitchTimer = Random.Range(2f, 5f);
        }
        if (attacking)
        {
            attackTimeout -= Time.deltaTime;
            if (attackTimeout <= 0f)
            {
                FinishAttack(); // fallback
            }
        }
        Act();
    }
    public void SetOrbitAngle(float angle)
    {
        desiredOrbitAngle = angle;
    }
    // ---------- AI ACTIVE ----------
    public void SetAIActive(bool active)
    {
        if (aiActive == active)
            return;

        aiActive = active;

        if (!aiActive)
        {
            EnemyManager.Instance.ReleaseAll(this);
            state = EnemyState.Idle;
            ApplyDebugColor(idleColor); // ⭐ HERE
        }
        else
        {
            // ✅ DO NOTHING ELSE
            // Do NOT force Passive
            // Do NOT force slot request
            state = EnemyState.Idle;
        }
    }


    // ---------- THINK ----------
    void Think()
    {
        if (!aiActive)
            return;

        switch (state)
        {
            case EnemyState.Idle:
                // Only request passive if manager allows it
                TryGetPassive();
                break;

            case EnemyState.Passive:
                TryGetActive();
                break;
        }
    }
    Vector3 GetOrbitPosition()
    {
        float rad = desiredOrbitAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(rad),
            0,
            Mathf.Sin(rad)
        ) * circleRadius;

        return player.position + offset;
    }
    void PassivePositioning()
    {
        Vector3 targetPos = GetOrbitPosition();

        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0;

        float dist = toTarget.magnitude;

        // 🔥 SPEED SCALING BASED ON DISTANCE
        float speed = Mathf.Lerp(3.5f, 6f, Mathf.Clamp01(dist / 6f));
        MoveTowards(targetPos, speed);

        RotateTo(player.position - transform.position);

        // ✅ Close enough → start circling
        if (dist < 0.4f)
        {
            state = EnemyState.Passive;
            ApplyDebugColor(passiveColor); // ⭐ HERE
            circleSwitchTimer = Random.Range(2f, 4f);
        }
    }
    void TryGetPassive()
    {
        if (EnemyManager.Instance.RequestPassive(this))
        {
            state = EnemyState.PassivePositioning;
        }
    }

    void TryGetActive()
    {
        if (EnemyManager.Instance.RequestActive(this))
        {
            state = EnemyState.Active;
            ApplyDebugColor(activeColor); // ⭐ HERE

            waitingToAttack = true;
            attacking = false;
            preAttackDelay = Random.Range(1.0f, 4.0f);
        }
    }

    // ---------- ACT ----------
    void Act()
    {
        switch (state)
        {
            case EnemyState.Idle:
                IdleMove();
                break;

            case EnemyState.PassivePositioning:
                PassivePositioning();
                break;

            case EnemyState.Passive:
                CirclePlayer();
                break;

            case EnemyState.Active:
                AttackPlayer();
                break;

            case EnemyState.Recover:
                RecoverMove();
                break;
        }
    }
    void RecoverMove()
    {
        // ถอยออกจาก player นิดนึง
        Vector3 away = transform.position - player.position;
        away.y = 0;

        if (away.sqrMagnitude < 0.01f)
            away = Random.insideUnitSphere;

        Vector3 moveDir = away.normalized;

        MoveByVelocity(moveDir, 2.5f);
        RotateTo(player.position - transform.position);
    }
    void IdleMove() 
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            // เลือกจุดใหม่ใกล้ตำแหน่งเดิม
            Vector2 rand = Random.insideUnitCircle * 2f;
            idleTarget = transform.position + new Vector3(rand.x, 0, rand.y);

            idleTimer = Random.Range(1.5f, 3f);
        }

        MoveTowards(idleTarget, 1.5f);

    }
    void RotateTo(Vector3 lookDir, float speed = 10f)
    {
        lookDir.y = 0;
        if (lookDir.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            Time.deltaTime * speed
        );
    }
    void CirclePlayer()
    {
        localOrbitOffset += Time.deltaTime * circleSpeed * circleDir;
        orbitFlowAngle += Time.deltaTime * circleSpeed * circleDir;
        // Vector from player to enemy
        Vector3 toEnemy = transform.position - player.position;
        toEnemy.y = 0;

        if (toEnemy.sqrMagnitude < 0.01f)
            return;

        float currentAngle = Mathf.Atan2(toEnemy.z, toEnemy.x);

        // ⭐ MOVING TARGET ANGLE (slot + flow)
        float targetAngle =
    desiredOrbitAngle + localOrbitOffset;

        // Angle difference (smooth correction)
        float angleDelta = Mathf.DeltaAngle(
            currentAngle * Mathf.Rad2Deg,
            targetAngle * Mathf.Rad2Deg
        ) * Mathf.Deg2Rad;

        // Tangent direction
        Vector3 tangent = new Vector3(
            -Mathf.Sin(currentAngle),
             0,
             Mathf.Cos(currentAngle)
        );

        // --- movement parts ---
        Vector3 orbitMove = tangent * circleSpeed;

        Vector3 angleCorrection =
            tangent * angleDelta * orbitAngleSpeed;

        float dist = toEnemy.magnitude;
        float distError = dist - circleRadius;
        Vector3 radial =
            -toEnemy.normalized * distError * 2f;

        Vector3 finalMove =
            orbitMove +
            angleCorrection +
            radial;

        MoveByVelocity(finalMove, 2.2f);
        RotateTo(player.position - transform.position);
    }
    void MoveByVelocity(Vector3 dir, float maxSpeed)
    {
        velocity = Vector3.Lerp(
            velocity,
            dir.normalized * maxSpeed,
            Time.deltaTime * 4f
        );

        transform.position += velocity * Time.deltaTime;
    }
    void AttackPlayer()
    {
        float disengageDist = 6f;

        if ((player.position - transform.position).sqrMagnitude > disengageDist * disengageDist)
        {
            waitingToAttack = false;
            EnemyManager.Instance.ReleaseAll(this);
            state = EnemyState.Passive;
            return;
        }
        // -------------------------
        // PRE-ATTACK (ข่ม / เล็ง)
        // -------------------------
        if (waitingToAttack)
        {
            preAttackDelay -= Time.deltaTime;

            // หันหน้าหาผู้เล่นตลอด
            RotateTo(player.position - transform.position);

            // ขยับแบบ pressure (ไม่วิ่งชน)
            Vector3 toPlayer = player.position - transform.position;
            toPlayer.y = 0;

            float dist = toPlayer.magnitude;

            if (dist > 2.2f)
            {
                // ค่อย ๆ เข้า
                MoveByVelocity(toPlayer, 1.8f);
            }
            else if (dist < 1.6f)
            {
                // ถอยนิด ๆ ไม่ยืนติด
                MoveByVelocity(-toPlayer, 1.5f);
            }
            else
            {
                // อยู่ใน sweet spot → ขยับซ้ายขวา
                Vector3 strafe = Vector3.Cross(Vector3.up, toPlayer.normalized);
                MoveByVelocity(strafe, 1.2f);
            }

            if (preAttackDelay <= 0f)
            {
                waitingToAttack = false;
            }

            return;
        }

        // -------------------------
        // ATTACK
        // -------------------------
        if (attacking) return;

        float distSq = (player.position - transform.position).sqrMagnitude;

        if (distSq > 2.5f * 2.5f)
        {
            MoveTowards(player.position, 4f);
            return;
        }

        attacking = true;
        attackTimeout = 1.5f; // กัน animation event พัง

        animator.SetTrigger("Attack"); // 🔥 เล่น animation ตรงนี้
    }
    void PerformAttack()
    {
        // damage logic / hitbox
    }
    public void FinishAttack()
    {
        attacking = false;
        waitingToAttack = false;

        state = EnemyState.Recover;
        ApplyDebugColor(recoverColor);

        Invoke(nameof(NotifyAttackFinished), Random.Range(0.6f, 1.2f));
    }

    void NotifyAttackFinished()
    {
        EnemyManager.Instance.OnEnemyFinishedAttack(this);
    }
    public void EnterActiveState()
    {
        state = EnemyState.Active;
        ResetAttackDelay();
    }
    
    public void ResetAttackDelay()
    {
        waitingToAttack = true;
        attacking = false;
        preAttackDelay = Random.Range(1.0f, 4.0f);
    }
    public bool IsReadyToAttack()
    {
        return state == EnemyState.Passive && !attacking;
    }
    void ReturnToPassive()
    {
        if (aiActive)
            state = EnemyState.Passive;
    }
    void MoveTowards(Vector3 target, float maxSpeed)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;

        float dist = dir.magnitude;
        if (dist < 0.1f)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 8f);
            return;
        }

        Vector3 desiredVelocity = dir.normalized * maxSpeed;

        // smooth acceleration
        velocity = Vector3.Lerp(
            velocity,
            desiredVelocity,
            Time.deltaTime * 5f
        );

        transform.position += velocity * Time.deltaTime;

        // rotate toward movement direction
        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                Time.deltaTime * 10f
            );
        }
    }
    void LookAtPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;

        if (lookDir.sqrMagnitude < 0.001f) return;

        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            Time.deltaTime * 10f
        );
    }
}
