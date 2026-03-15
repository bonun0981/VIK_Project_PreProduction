using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    OuterRing,
    InnerRing,
    AttackTurn,
    FightingAlly
}
enum CircleMove
{
    Hold,
    StrafeLeft,
    StrafeRight
    
}
public class EnemyNormal : MonoBehaviour
{
    bool isKnockedBack;
    bool movementLocked;
    float repathTimer;
    public float repathInterval = 0.2f;
    EnemyAnimationController anim;
    public float sleepDistance = 30f;
    public Transform allyTarget;
    [Header("Avoidance Priority")]
    public int attackPriority = 5;
    public int innerPriority = 20;
    public int outerPriority = 40;
    public int allyFightPriority = 15;
    public int idlePriority = 70;

    [SerializeField] Renderer rend;
    [Header("Debug Colors")]
    public Color idleColor = Color.gray;
    public Color outerRingColor = Color.cyan;
    public Color innerRingColor = Color.yellow;
    public Color attackColor = Color.red;
    public Color allyFightColor = Color.magenta;


    public EnemyState currentState;
    CircleMove currentMove;

    float moveTimer;
    float moveDuration;

    public float strafeSpeed = 2f;
    public float stepDistance = 1.2f;
    public float pauseChance = 0.15f;
    public Transform player;
    public NavMeshAgent agent;

    public bool aiActive;

    public float attackRange = 2f;
    public float minAttackDistance = 1.6f;
    public float attackCooldown;
    public float minAttackCooldown = 2f;
    public float maxAttackCooldown = 2f;
    public float circleSpeed = 1.5f;

    public float lastAttack;

    float circleOffset;
    void RollAttackCooldown()
    {
        attackCooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
    }
    void Start()
    {
        attackCooldown= Random.Range(minAttackCooldown, maxAttackCooldown);
        //rend = GetComponentInChildren<Renderer>();
        anim = GetComponent<EnemyAnimationController>();
        PickCircleMove();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.stoppingDistance = attackRange;
        agent.updateRotation = true;
        agent.acceleration = 40;
        agent.angularSpeed = 720;
        agent.speed = 3.5f;
        agent.radius = 1f;
        agent.updateRotation = false;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70);
        circleOffset = Random.Range(0f, 360f);
        UpdateDebugColor();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void UpdateDebugColor()
    {
        if (rend == null) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                rend.material.color = idleColor;
                break;

            case EnemyState.OuterRing:
                rend.material.color = outerRingColor;
                break;

            case EnemyState.InnerRing:
                rend.material.color = innerRingColor;
                break;

            case EnemyState.AttackTurn:
                rend.material.color = attackColor;
                break;

            case EnemyState.FightingAlly:
                rend.material.color = allyFightColor;
                break;
        }
    }
    void ApplyAvoidance()
    {
        if (agent == null) return;

        switch (currentState)
        {
            case EnemyState.AttackTurn:
                agent.avoidancePriority = attackPriority;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                break;

            case EnemyState.FightingAlly:
                agent.avoidancePriority = allyFightPriority;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                break;

            case EnemyState.InnerRing:
                agent.avoidancePriority = innerPriority;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                break;

            case EnemyState.OuterRing:
                agent.avoidancePriority = outerPriority;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                break;

            case EnemyState.Idle:
                agent.avoidancePriority = idlePriority;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                break;
        }
    }
    public void SetState(EnemyState state)
    {
        currentState = state;

        if (state == EnemyState.AttackTurn)
            agent.stoppingDistance = attackRange;
        else
            agent.stoppingDistance = 0.2f;

        UpdateDebugColor();
        ApplyAvoidance();
    }
    void Update()
    {
        
        if (isKnockedBack)
            return;

        float distToPlayer =
            Vector3.Distance(transform.position, player.position);

        if (aiActive && distToPlayer > sleepDistance)
        {
            SleepEnemy();
            return;
        }

        if (!aiActive && distToPlayer < sleepDistance * 0.8f)
        {
            ActivateAI();
        }

        if (!aiActive) return;

        if (movementLocked) return;

        LookAtPlayer();
        switch (currentState)
        {
            case EnemyState.InnerRing:
                CirclePlayer();
                TryAttack();
                break;

            case EnemyState.OuterRing:
                CirclePlayer();
                break;

            case EnemyState.AttackTurn:

                float dist =
                    Vector3.Distance(transform.position, player.position);

                if (dist > minAttackDistance)
                {
                    agent.SetDestination(player.position);
                }
                else
                {
                    agent.ResetPath();
                    AttackPlayer();
                }

                break;

            case EnemyState.FightingAlly:
                FightAlly();
                break;
        }
    }



    void TryAttack()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // ถ้าเข้า range → ขอ attack turn
        if (dist < minAttackDistance)
        {
            EnemyCombatDirector.Instance.RequestPlayerAttack(this);
            return; 
        }

        // ถ้าไม่มีสิทธิ์โจมตี → อย่าเดินเข้า
        if (!EnemyCombatDirector.Instance.HasPlayerAttackSlot())
            return;

        // ดันเข้า player เฉพาะตอนมี slot ว่าง
        Vector3 dir = (player.position - transform.position).normalized;

        Vector3 target = player.position - dir * minAttackDistance;

        agent.SetDestination(target);
    }

    void AttackPlayer()
    {
        if (Time.time < lastAttack + attackCooldown)
            return;

        anim.PlayAttack();

        Debug.Log("Enemy Attack");
    }
    public void FinishAttack()
    {
        Debug.Log("Enemy Attack Finish");

        lastAttack = Time.time;      // เริ่ม cooldown
        RollAttackCooldown();        // สุ่ม cooldown ใหม่

        EnemyCombatDirector.Instance.FinishPlayerAttack(this);
    }

    public void SetPlayer(Transform p)
    {
        player = p;
    }
    void PickCircleMove()
    {
        moveDuration = Random.Range(0.7f, 1.5f);
        moveTimer = 0;

        float r = Random.value;

        if (r < pauseChance)
            currentMove = CircleMove.Hold;
        else if (Random.value > 0.5f)
            currentMove = CircleMove.StrafeLeft;
        else
            currentMove = CircleMove.StrafeRight;
    }
    void CirclePlayer()
    {
        if (player == null) return;

        moveTimer += Time.deltaTime;

        if (moveTimer > moveDuration)
            PickCircleMove();

        if (currentMove == CircleMove.Hold)
        {
            agent.ResetPath();
            return;
        }

        Vector3 toEnemy = transform.position - player.position;
        toEnemy.y = 0;

        float currentDistance = toEnemy.magnitude;

        if (currentDistance < 0.1f) return;

        Vector3 dir = toEnemy.normalized;

        float desiredRadius = EnemyCombatDirector.Instance.GetRingRadius(this);

        // tangent direction (เดินรอบ player)
        Vector3 perpendicular = new Vector3(-dir.z, 0, dir.x);

        if (currentMove == CircleMove.StrafeLeft)
            perpendicular = -perpendicular;

        // ปรับระยะให้อยู่ในวง
        float distanceError = currentDistance - desiredRadius;

        Vector3 move = perpendicular;

        if (Mathf.Abs(distanceError) > 0.2f)
        {
            move += -dir * distanceError * 1.5f;
        }

        Vector3 target = transform.position + move.normalized * 4f;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(target, out hit, 2f, NavMesh.AllAreas))
        {
            repathTimer += Time.deltaTime;

            if (repathTimer > repathInterval)
            {
                repathTimer = 0f;
                agent.SetDestination(hit.position);
            }
        }
        
    }
    void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * 8f
        );
    }
    public void ActivateAI()
    {
        if (aiActive) return;

        aiActive = true;

        EnemyCombatDirector.Instance.RegisterEnemy(this);
    }
    public void EngageAlly(Transform ally)
    {
        ActivateAI();

        if (!EnemyCombatDirector.Instance.RequestAllyFight(this, ally))
            return;

        EnemyCombatDirector.Instance.RemoveFromPlayerAttackers(this);

        allyTarget = ally;

        SetState(EnemyState.FightingAlly);
    }
    void FightAlly()
    {
        if (allyTarget == null || !allyTarget.gameObject.activeInHierarchy)
        {
            EnemyCombatDirector.Instance.FinishAllyFight(this, allyTarget);

            allyTarget = null;

            SetState(EnemyState.InnerRing);
            return;
        }

        float dist =
            (transform.position - allyTarget.position).sqrMagnitude;

        float attackDist = attackRange * attackRange;

        if (dist > attackDist)
        {
            agent.SetDestination(allyTarget.position);
        }
        else
        {
            agent.ResetPath();
            AttackAlly();
        }

        LookAtAlly();
    }
    void AttackAlly()
    {
        if (Time.time < lastAttack + attackCooldown)
            return;

        lastAttack = Time.time;

        anim.PlayAttack();

        Debug.Log("Enemy Attack Ally");

       
    }
    void LookAtAlly()
    {
        if (allyTarget == null) return;

        Vector3 dir = allyTarget.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            Time.deltaTime * 10f
        );
    }

    void SleepEnemy()
    {
        aiActive = false;

        EnemyCombatDirector.Instance.UnregisterEnemy(this);

        allyTarget = null;

        agent.ResetPath();

        SetState(EnemyState.Idle);
    }

    public void LockMovement()
    {
        if (isKnockedBack) return;

        movementLocked = true;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void UnlockMovement()
    {
        movementLocked = false;

        if (agent != null)
        {
            agent.isStopped = false;
        }
    }
    public void AnimStartAttack()
    {
        LockMovement();
    }

    public void AnimEndAttack()
    {
        UnlockMovement();
        FinishAttack();
        Debug.Log("Enemy End Attacking");
    }
    public void OnHurt()
    {
        UnlockMovement();

        if (agent.enabled)
            agent.ResetPath();

        if (currentState == EnemyState.AttackTurn)
        {
            lastAttack = Time.time;
        }

        if (currentState == EnemyState.FightingAlly)
        {
            agent.ResetPath();
        }
    }

    public void StartKnockback()
    {
        isKnockedBack = true;

        if (agent != null)
            agent.enabled = false;
    }

    public void EndKnockback()
    {
        isKnockedBack = false;

        if (agent != null)
            agent.enabled = true;
    }
}