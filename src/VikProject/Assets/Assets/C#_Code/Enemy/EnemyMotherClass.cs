using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.PlacematContainer;

public class EnemyMotherClass : MonoBehaviour
{
    // =========================================================
    // General
    // =========================================================

    // 🔥 กันตีซ้ำทันทีหลัง Recover
    [SerializeField] float reengageDelay = 0.4f;
    float reengageTimer;

    // 🔥 Combat Ring
    [SerializeField] float combatRingDistance = 2.8f;
    [SerializeField] float combatRingTolerance = 0.4f;

   
    float postAttackDelay = 0.15f;
    float postAttackTimer;
    bool waitingAfterAttack;
    Vector3 lastDestination;
    [SerializeField] float repathThreshold = 0.3f;

    NavMeshAgent agent;
    Animator animator;

    public bool aiActive;
    public bool isAttacking;

    // =========================================================
    // State System
    // =========================================================

    public enum EnemyState { Idel, Patrol, Passive, Active, Recover }
    public EnemyState state;

    public enum PassiveBehaviour { None, MoveInOut, Circle }
    PassiveBehaviour currentPassiveBehaviour = PassiveBehaviour.None;

    [Header("State Distance Config")]
    [SerializeField] float passiveStoppingDistance = 2.5f;
    [SerializeField] float activeStoppingDistance = 2f;
    [SerializeField] float recoverStoppingDistance = 1.5f;

    [Header("State Combat Config")]
    [SerializeField] float passiveAttackRange = 0f;
    [SerializeField] float activeAttackRange = 3f;
    [SerializeField] float recoverAttackRange = 0f;

    // =========================================================
    // Passive Config
    // =========================================================

    [Header("Passive Pause Config")]
    [SerializeField] float passivePauseMin = 0.5f;
    [SerializeField] float passivePauseMax = 1.2f;

    bool hasPassiveTarget;
    bool isPassivePausing;
    float passivePauseTimer;

    [Header("Passive Distance Config")]
    [SerializeField] float passiveMinEdgeDistance = 1.5f;
    [SerializeField] float passiveMaxEdgeDistance = 3.5f;
    [SerializeField] float circleDistanceMin = 1.5f;
    [SerializeField] float circleDistanceMax = 3f;
    [SerializeField] float circleDistanceAdjustSpeed = 1.5f;
    [SerializeField] float desiredCircleDistance = 3f;
   

    [SerializeField] float passiveMoveSpeedMin = 0.3f;
    [SerializeField] float passiveMoveSpeedMax = 1.4f;

    float passiveMoveSpeed;
    int circleDirection;
    int moveInOutType;

    float personalOrbitSide;
    float personalAngleOffset;

    // =========================================================
    // Movement Smoothing
    // =========================================================

    Vector3 currentVelocity;
    [SerializeField] float acceleration = 4f;
    [SerializeField] float deceleration = 6f;

    [SerializeField] float thinkDuration = 0.4f;
    bool isThinking;
    float thinkTimer;

    // =========================================================
    // Active / Attack
    // =========================================================

    [SerializeField] float attackRange = 0.5f;
    [SerializeField] float activeMoveSpeed = 2.2f;

    bool reachedAttackRange;
    Vector3 activeTargetPosition;
    bool hasActiveTarget;

    [SerializeField] float attackOffsetRadius = 1.5f;
    [SerializeField] float recoveryDelayMin = 1f;
    [SerializeField] float recoveryDelayMax = 1.8f;
    [SerializeField] float attackCooldown = 5f;

    float attackCooldownTimer;
    float recoveryTimer;
    bool recoveryInitialized;

    // =========================================================
    // Player
    // =========================================================

    public Transform playerPositon;
    public GameObject player;

    // =========================================================
    // Debug
    // =========================================================

    [SerializeField] bool debugStateColor = true;

    Color attackColor = Color.magenta;
    Color passiveColor = Color.yellow;
    Color activeColor = Color.red;
    Color idleColor = Color.white;
    Color recoverColor = Color.cyan;

    Renderer[] renderers;
    MaterialPropertyBlock mpb;

    // =========================================================
    // Unity Methods
    // =========================================================

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = true;
        agent.acceleration = 10f;
        agent.angularSpeed = 480f;
        agent.autoBraking = true;

        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerPositon = player.transform;

        renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();

        personalOrbitSide = Random.value > 0.5f ? 1f : -1f;
        personalAngleOffset = Random.Range(-40f, 40f);
    }

    private void Update()
    {
        if (isAttacking)
        {
            agent.velocity = Vector3.zero;
        }
        if (!aiActive) return;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        Act();
    }

    // =========================================================
    // Core State Logic
    // =========================================================

    void Act()
    {
        switch (state)
        {
            case EnemyState.Idel:
                ApplyDebugColor(idleColor);
                break;

            case EnemyState.Passive:
                ApplyDebugColor(passiveColor);
                PassiveMovement();
                break;

            case EnemyState.Active:
                ApplyDebugColor(activeColor);
                MoveToward();
                break;

            case EnemyState.Recover:
                ApplyDebugColor(recoverColor);
                Recovery();
                break;
        }
    }

    void ChangeState(EnemyState newState)
    {
        state = newState;
        agent.isStopped = false;
        isThinking = false;

        ApplyStateDistance();

        switch (state)
        {
            case EnemyState.Passive:
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                agent.avoidancePriority = 80;
                break;

            case EnemyState.Active:

                // 🔥 เปิดหลบกันเอง
                agent.obstacleAvoidanceType =
                    ObstacleAvoidanceType.HighQualityObstacleAvoidance;

                // ถ้ายังไม่ได้สิทธิ์ตี → priority ปกติ
                if (!isAttacking)
                    agent.avoidancePriority = 50;
                else
                    agent.avoidancePriority = 1; // ตัวตี = ทุกตัวหลบ

                break;

            case EnemyState.Recover:
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                agent.avoidancePriority = 70;
                break;
        }
    }

    void ApplyStateDistance()
    {
        switch (state)
        {
            case EnemyState.Passive:
                agent.stoppingDistance = passiveStoppingDistance;
                attackRange = passiveAttackRange;
                agent.speed = passiveMoveSpeedMax;
                break;

            case EnemyState.Active:
                agent.stoppingDistance = activeStoppingDistance;
                attackRange = activeAttackRange;
                agent.speed = activeMoveSpeed;
                break;

            case EnemyState.Recover:
                agent.stoppingDistance = recoverStoppingDistance;
                attackRange = recoverAttackRange;
                agent.speed = passiveMoveSpeedMin;
                break;
        }
    }

    bool HasReachedDestination()
    {
        if (agent.pathPending) return false;
        if (agent.remainingDistance > agent.stoppingDistance + 0.05f) return false;
        if (agent.hasPath && agent.velocity.sqrMagnitude > 0.01f) return false;
        return true;
    }

    void SmartSetDestination(Vector3 target)
    {
        if (agent.pathPending) return;

        float distance = Vector3.Distance(agent.destination, target);
        if (distance > 0.2f)
            agent.SetDestination(target);
    }

    void ApplyDebugColor(Color c)
    {
        if (!debugStateColor) return;

        mpb.SetColor("_BaseColor", c);
        foreach (var r in renderers)
            r.SetPropertyBlock(mpb);
    }

    // =========================================================
    // Passive Behaviour
    // =========================================================

    public void PassiveMovement()
    {
        if (playerPositon == null) return;

        if (!hasPassiveTarget)
        {
            ChoosePassiveBehaviour();
            hasPassiveTarget = true;
            return;
        }

        if (isPassivePausing)
        {
            passivePauseTimer -= Time.deltaTime;
            agent.isStopped = true;
            LookAtPlayer();

            if (passivePauseTimer <= 0f)
            {
                isPassivePausing = false;
                ChoosePassiveBehaviour();
            }
            return;
        }

        if (agent.hasPath && HasReachedDestination())
        {
            isPassivePausing = true;
            passivePauseTimer = Random.Range(passivePauseMin, passivePauseMax);
            agent.isStopped = true;
            return;
        }

        ExecutePassiveBehaviour();
    }

    void ChoosePassiveBehaviour()
    {
        int randomChoice = Random.Range(0, 10);

        if (randomChoice < 3)
            currentPassiveBehaviour = PassiveBehaviour.MoveInOut;
        else
            currentPassiveBehaviour = PassiveBehaviour.Circle;

        SetupPassiveBehaviour();
    }

    void SetupPassiveBehaviour()
    {
        passiveMoveSpeed = Random.Range(passiveMoveSpeedMin, passiveMoveSpeedMax);

        if (currentPassiveBehaviour == PassiveBehaviour.Circle)
        {
            circleDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            desiredCircleDistance = Random.Range(circleDistanceMin, circleDistanceMax);
        }
        else
        {
            moveInOutType = Random.Range(0, 3);
        }
    }

    void ExecutePassiveBehaviour()
    {
        switch (currentPassiveBehaviour)
        {
            case PassiveBehaviour.MoveInOut:
                MoveInAndOut();
                break;
            case PassiveBehaviour.Circle:
                CircleAround();
                break;
        }
    }
    // =========================================================
    // Active Behaviour
    // =========================================================

    void MoveToward()
    {
        // 🔥 รอหลังโจมตี
        if (waitingAfterAttack)
        {
            postAttackTimer -= Time.deltaTime;
            if (postAttackTimer > 0f)
                return;

            waitingAfterAttack = false;
        }

        // 🔥 ลด reengage timer
        if (reengageTimer > 0f)
            reengageTimer -= Time.deltaTime;

        if (!agent.enabled || playerPositon == null) return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, playerPositon.position);

        float desiredAttackDistance =
            attackRange + agent.radius + 0.15f;

        agent.acceleration = 8f;
        agent.angularSpeed = 300f;
        agent.stoppingDistance = 0.05f;

        // =========================================================
        // 1️⃣ ยังไกล → เดินเข้าหา
        // =========================================================
        if (distanceToPlayer > desiredAttackDistance + 0.6f)
        {
            float slowRange = 3f;

            float t = Mathf.InverseLerp(
                desiredAttackDistance,
                slowRange,
                distanceToPlayer);

            float dynamicSpeed =
                Mathf.Lerp(1.2f, activeMoveSpeed, t);

            Vector3 offset =
                (transform.position - playerPositon.position).normalized
                * 0.5f;

            agent.speed = dynamicSpeed;
            agent.isStopped = false;

            if (!agent.hasPath || agent.remainingDistance > 0.2f)
                SmartSetDestination(playerPositon.position + offset);

            LookAtPlayer();
            return;
        }

        // =========================================================
        // 2️⃣ อยู่ในระยะโจมตี → ขอสิทธิ์
        // =========================================================
        if (!isAttacking &&
            attackCooldownTimer <= 0f &&
            reengageTimer <= 0f)
        {
            bool granted =
                EnemyStateManager.Instance.RequestAttack(this);

            if (granted)
            {
                isAttacking = true;

                agent.avoidancePriority = 1;
                agent.obstacleAvoidanceType =
                    ObstacleAvoidanceType.NoObstacleAvoidance;

                agent.ResetPath();
                agent.isStopped = true;

                LookAtPlayer();
                StartAttack();
                return;
            }
        }

        // =========================================================
        // 3️⃣ ไม่ได้สิทธิ์ → รักษา Combat Ring
        // =========================================================

        Vector3 toPlayer =
            transform.position - playerPositon.position;

        toPlayer.y = 0;

        float currentDistance = toPlayer.magnitude;
        Vector3 dir = toPlayer.normalized;

        // ถ้าใกล้เกิน → ถอย
        if (currentDistance <
            combatRingDistance - combatRingTolerance)
        {
            Vector3 retreatPos =
                playerPositon.position +
                dir * combatRingDistance;

            agent.speed = 2f;
            agent.isStopped = false;

            SmartSetDestination(retreatPos);
            LookAtPlayer();
            return;
        }

        // ถ้าไกลเกิน → ขยับเข้า
        if (currentDistance >
            combatRingDistance + combatRingTolerance)
        {
            Vector3 approachPos =
                playerPositon.position +
                dir * combatRingDistance;

            agent.speed = 1.8f;
            agent.isStopped = false;

            SmartSetDestination(approachPos);
            LookAtPlayer();
            return;
        }

        // อยู่ในระยะ → orbit
        Vector3 perpendicular =
            new Vector3(-dir.z, 0, dir.x);

        Vector3 orbitPos =
            playerPositon.position +
            (dir + perpendicular *
             personalOrbitSide * 0.7f)
            .normalized * combatRingDistance;

        agent.speed = 1.5f;
        agent.isStopped = false;

        SmartSetDestination(orbitPos);
        LookAtPlayer();
    }
    // =========================================================
    // Passive Movement
    // =========================================================

    public void MoveInAndOut()
    {
        if (playerPositon == null) return;

        Vector3 toPlayer =
            playerPositon.position - transform.position;

        toPlayer.y = 0;

        float centerDistance = toPlayer.magnitude;
        Vector3 dir = toPlayer.normalized;

        float enemyRadius = agent.radius;
        float playerRadius = 0.5f;

        CapsuleCollider playerCol =
            player.GetComponent<CapsuleCollider>();

        if (playerCol != null)
            playerRadius =
                playerCol.radius * player.transform.localScale.x;

        float edgeDistance =
            centerDistance - enemyRadius - playerRadius;

        if (isThinking)
        {
            thinkTimer -= Time.deltaTime;
            agent.isStopped = true;
            LookAtPlayer();

            if (thinkTimer <= 0f)
                isThinking = false;

            return;
        }

        if (edgeDistance < passiveMinEdgeDistance)
        {
            Vector3 retreatPos =
                playerPositon.position -
                dir * (passiveMinEdgeDistance +
                       enemyRadius + playerRadius);

            agent.isStopped = false;
            agent.speed = passiveMoveSpeed;
            SmartSetDestination(retreatPos);
            LookAtPlayer();
            return;
        }

        if (edgeDistance > passiveMaxEdgeDistance)
        {
            Vector3 approachPos =
                playerPositon.position +
                dir * (passiveMaxEdgeDistance +
                       enemyRadius + playerRadius);

            agent.isStopped = false;
            agent.speed = passiveMoveSpeed;
            SmartSetDestination(approachPos);
            LookAtPlayer();
            return;
        }

        Vector3 moveDir =
            (moveInOutType == 1) ? dir : -dir;

        Vector3 targetPos =
            transform.position + moveDir * 1.5f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(
            targetPos, out hit, 2f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.speed = passiveMoveSpeed;
            SmartSetDestination(hit.position);
        }

        LookAtPlayer();
    }

    public void CircleAround()
    {
        if (playerPositon == null) return;

        Vector3 toPlayer =
            transform.position - playerPositon.position;

        toPlayer.y = 0;

        float currentDistance = toPlayer.magnitude;
        if (currentDistance < 0.1f) return;

        Vector3 dir = toPlayer.normalized;

        Vector3 perpendicular =
            new Vector3(-dir.z, 0, dir.x);

        Vector3 move =
            perpendicular * circleDirection;

        float distanceError =
            currentDistance - desiredCircleDistance;

        if (Mathf.Abs(distanceError) > 0.2f)
        {
            move +=
                -dir * distanceError *
                circleDistanceAdjustSpeed;
        }

        Vector3 targetPos =
            transform.position +
            move.normalized * 2f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(
            targetPos, out hit, 2f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.speed = passiveMoveSpeed;
            SmartSetDestination(hit.position);
        }

        LookAtPlayer();
    }

    // =========================================================
    // Attack & Recovery
    // =========================================================

    public void StartAttack()
    {
        agent.isStopped = true;
        agent.ResetPath();

        agent.velocity = Vector3.zero;

        // 🔥 สำคัญ: ลดแรงดันจาก RVO
        agent.avoidancePriority = 25;
        agent.obstacleAvoidanceType =
            ObstacleAvoidanceType.LowQualityObstacleAvoidance;

        // 🔥 ไม่ต้องปิด updatePosition
        // 🔥 ไม่ต้องแตะ nextPosition

        animator.SetTrigger("Attack");
    }

    public void FinsihAttack()
    {
        // 🔥 คืนค่าการเคลื่อนที่
        agent.speed = activeMoveSpeed;
        agent.acceleration = 5f;
        agent.stoppingDistance = 0.05f;

        agent.isStopped = false;
        agent.ResetPath();

        agent.avoidancePriority = 40;

        EnemyStateManager.Instance.ReleaseAttack(this);

        isAttacking = false;
        hasActiveTarget = false;

        attackCooldownTimer = attackCooldown;

        recoveryTimer =
            Random.Range(recoveryDelayMin, recoveryDelayMax);

        recoveryInitialized = true;

        ChangeState(EnemyState.Recover);
    }
    public void Recovery()
    {
        if (!recoveryInitialized) return;

        recoveryTimer -= Time.deltaTime;

        ApplyMovement(Vector3.zero);
        LookAtPlayer();

        if (recoveryTimer > 0f) return;

        recoveryInitialized = false;

        int decision = Random.Range(0, 3);

        if (decision > 0)
        {
            bool gotActive =
                EnemyStateManager.Instance
                .TryRequestActive(this);

            if (!gotActive)
            {
                EndAttack();
            }
        }
        else
        {
            EndAttack();
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        hasActiveTarget = false;

        EnemyStateManager.Instance.RequestPassive(this);

        ChangeState(EnemyState.Passive);   // 🔥 สำคัญมาก
    }

    // =========================================================
    // Utility
    // =========================================================

    void ApplyMovement(Vector3 dir)
    {
        if (agent == null) return;

        if (dir == Vector3.zero)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        Vector3 targetPos =
            transform.position + dir;

        SmartSetDestination(targetPos);
    }

    void LookAtPlayer(float rotateSpeed = 8f)
    {
        if (playerPositon == null) return;

        Vector3 lookDir =
            playerPositon.position - transform.position;

        lookDir.y = 0;

        if (lookDir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot =
            Quaternion.LookRotation(lookDir);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * rotateSpeed);
    }

    public void ResetEnemyState()
    {
        if (!aiActive) return;

        isAttacking = false;
        hasActiveTarget = false;

        recoveryInitialized = false;
        recoveryTimer = 0f;

        isThinking = false;
        thinkTimer = 0f;

        currentVelocity = Vector3.zero;

        if (animator != null)
            animator.ResetTrigger("Attack");

        EnemyStateManager.Instance.ReleaseAttack(this);
        EnemyStateManager.Instance.RequestPassive(this);
    }
}
    // =========================================================
    // (Active / Attack / Recovery / Utility functions continue exactly as your original logic)
    // =========================================================
