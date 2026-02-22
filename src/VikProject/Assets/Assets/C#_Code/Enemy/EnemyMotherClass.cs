using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.PlacematContainer;

public class EnemyMotherClass : MonoBehaviour
{
    [Header("State Distance Config")]
    [SerializeField] float passiveStoppingDistance = 2.5f;
    [SerializeField] float activeStoppingDistance = 2f;
    [SerializeField] float recoverStoppingDistance = 1.5f;
    [Header("State Combat Config")]
    [SerializeField] float passiveAttackRange = 0f;
    [SerializeField] float activeAttackRange = 3f;
    [SerializeField] float recoverAttackRange = 0f;
    NavMeshAgent agent;
    Animator animator;
    //handel enemy states
    public enum EnemyState
   {
        Idel,
        Patrol,
        Passive,
        Active,
        Recover
   }
    public EnemyState state;

    //handel enemy passive behaviour
    public enum PassiveBehaviour
    {
        None,
        MoveInOut,
        Circle
   }
    PassiveBehaviour currentPassiveBehaviour = PassiveBehaviour.None;

    //Enemy active status
    public bool aiActive;
   


    //Enemy Timers
    public float attackDelayTime;
    public float thinkTime;

    //Enemy Bools
    public bool isAttacking;

    //Enemy movement variables

    [SerializeField] float circleDistanceMin = 1.5f;
    [SerializeField] float circleDistanceMax = 3f;
    [SerializeField] float circleDistanceAdjustSpeed = 1.5f;
    float desiredCircleDistance;

    [SerializeField] float passiveMoveSpeedMin = 0.3f;
    [SerializeField] float passiveMoveSpeedMax = 1.4f;
    float passiveTimer;
    float passiveMoveSpeed;
    int circleDirection;
    int moveInOutType;

    bool forceRetreat;

    // Movement smoothing
    Vector3 currentVelocity;
    [SerializeField] float acceleration = 4f;
    [SerializeField] float deceleration = 6f;

    [SerializeField] float thinkDuration = 0.4f;

    bool isThinking;
    float thinkTimer;

    //enemy attack variables
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

    //Player
    public Transform playerPositon;
    public GameObject player;

    //color debug
    [SerializeField] bool debugStateColor = true;
    Color attackColor = Color.magenta;
    Color passiveColor = Color.yellow;
    Color activeColor = Color.red;
    Color idleColor = Color.white;
    Color recoverColor = Color.cyan;
    Renderer[] renderers;
    MaterialPropertyBlock mpb;
    void ApplyDebugColor(Color c)
    {
        if (!debugStateColor) return;

        mpb.SetColor("_BaseColor", c); // URP/Lit
                                       // mpb.SetColor("_Color", c);  // Built-in fallback

        foreach (var r in renderers)
            r.SetPropertyBlock(mpb);
    }


    void ApplyStateDistance()
    {
        switch (state)
        {
            case EnemyState.Passive:
                agent.stoppingDistance = passiveStoppingDistance;
                attackRange = passiveAttackRange;
                break;

            case EnemyState.Active:
                agent.stoppingDistance = activeStoppingDistance;
                attackRange = activeAttackRange;
                break;

            case EnemyState.Recover:
                agent.stoppingDistance = recoverStoppingDistance;
                attackRange = recoverAttackRange;
                break;
        }
    }
    void ChangeState(EnemyState newState)
    {
        state = newState;

        agent.isStopped = false;
        isThinking = false;

        ApplyStateDistance();
    }
    // start and update methods
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // เราหมุนเอง
        agent.updateUpAxis = false;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerPositon = player.transform;
        renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
    }
    private void Update()
    {
        if (!aiActive) return;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        Act();
    }



    //State change method 


    //Enemy Action Handle
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
   
   

    public void PassiveMovement()//passive movement manager
    {
        //decide movement type
        if (playerPositon == null) return;

        passiveTimer -= Time.deltaTime;

        if (passiveTimer <= 0f)
        {
            ChoosePassiveBehaviour();
        }

        ExecutePassiveBehaviour();
    }
    void ChoosePassiveBehaviour()
    {
        currentVelocity *= 0.5f;
        passiveTimer = Random.Range(2f, 4f);

        int randomChoice = Random.Range(0, 10);
        // 0-9

        if (randomChoice < 3) // 0,1,2 = 30%
            currentPassiveBehaviour = PassiveBehaviour.MoveInOut;
        else                  // 3-9 = 70%
            currentPassiveBehaviour = PassiveBehaviour.Circle;

        SetupPassiveBehaviour();
    }
    void SetupPassiveBehaviour()
    {
        passiveMoveSpeed = Random.Range(passiveMoveSpeedMin, passiveMoveSpeedMax);

        if (currentPassiveBehaviour == PassiveBehaviour.Circle)
        {
            circleDirection = Random.Range(0, 2) == 0 ? -1 : 1;

            //enemy picks its own preferred distance
            desiredCircleDistance = Random.Range(circleDistanceMin, circleDistanceMax);
        }
        else if (currentPassiveBehaviour == PassiveBehaviour.MoveInOut)
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

    //Movement Handle
    public void MoveInAndOut()
    {
        if (playerPositon == null) return;

        Vector3 toPlayer = playerPositon.position - transform.position;
        toPlayer.y = 0;

        float distance = toPlayer.magnitude;
        Vector3 dir = toPlayer.normalized;

        float minDist = 1f;
        float maxDist = 4f;

        // ===== THINKING =====
        if (isThinking)
        {
            thinkTimer -= Time.deltaTime;
            agent.isStopped = true;
            LookAtPlayer();

            if (thinkTimer <= 0f)
                isThinking = false;

            return;
        }

        // ===== OUT OF RANGE → THINK =====
        if (distance <= minDist || distance >= maxDist)
        {
            isThinking = true;
            thinkTimer = thinkDuration;
            return;
        }

        // ===== MOVEMENT =====
        Vector3 moveDir = (moveInOutType == 1) ? dir : -dir;

        Vector3 targetPos = transform.position + moveDir * 2f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 2f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.speed = passiveMoveSpeed;
            agent.SetDestination(hit.position);
        }

        LookAtPlayer();
    }


    public void CircleAround()
    {
        if (playerPositon == null) return;

        Vector3 toPlayer = transform.position - playerPositon.position;
        toPlayer.y = 0;

        float currentDistance = toPlayer.magnitude;
        if (currentDistance < 0.1f) return;

        Vector3 dir = toPlayer.normalized;

        // 1️⃣ ด้านข้าง
        Vector3 perpendicular = new Vector3(-dir.z, 0, dir.x);
        Vector3 move = perpendicular * circleDirection;

        // 2️⃣ ปรับระยะ
        float distanceError = currentDistance - desiredCircleDistance;

        if (Mathf.Abs(distanceError) > 0.2f)
        {
            move += -dir * distanceError * circleDistanceAdjustSpeed;
        }

        Vector3 targetPos = transform.position + move.normalized * 2f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 2f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.speed = passiveMoveSpeed;
            agent.SetDestination(hit.position);
        }

        LookAtPlayer();
    }


    public void MoveToward()
    {
        if (playerPositon == null) return;

        float centerDistance = Vector3.Distance(transform.position, playerPositon.position);

        float enemyRadius = agent.radius;

        // ดึง radius player จริงจาก collider
        float playerRadius = 0.5f;
        CapsuleCollider playerCol = player.GetComponent<CapsuleCollider>();
        if (playerCol != null)
            playerRadius = playerCol.radius * player.transform.localScale.x;

        // 🔥 ระยะจากขอบชนขอบ
        float edgeDistance = centerDistance - playerRadius - enemyRadius;

        float desiredCombatDistance = attackRange;

        // 🔥 คำนวณตำแหน่งรอบ player (ไม่เข้า center)
        Vector3 dir = (transform.position - playerPositon.position).normalized;
        Vector3 targetPos =
            playerPositon.position + dir * (desiredCombatDistance + playerRadius + enemyRadius);

        if (edgeDistance > desiredCombatDistance)
        {
            agent.isStopped = false;
            agent.speed = activeMoveSpeed;
            agent.SetDestination(targetPos);
        }
        else
        {
            agent.isStopped = true;
            LookAtPlayer();

            if (!isAttacking && attackCooldownTimer <= 0f)
            {
                if (EnemyStateManager.Instance.RequestAttack(this))
                {
                    isAttacking = true;
                    StartAttack();
                }
            }
        }
    }

    void ApplyMovement(Vector3 dir)
    {
        if (agent == null) return;

        if (dir == Vector3.zero)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        Vector3 targetPos = transform.position + dir;
        agent.SetDestination(targetPos);
    }
    void LookAtPlayer(float rotateSpeed = 8f)
    {
        if (playerPositon == null) return;

        Vector3 lookDir = playerPositon.position - transform.position;
        lookDir.y = 0;

        if (lookDir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(lookDir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * rotateSpeed
        );
        agent.nextPosition = transform.position;
    }
    void UpdateAnimator()
    {
        if (animator == null) return;

        float speed = currentVelocity.magnitude;
        animator.SetFloat("VelocityX", speed);

        if (speed < 0.05f)
        {
            animator.SetBool("MoveRight", false);
            animator.SetBool("MoveLeft", false);
            animator.SetBool("MoveForward", false);
            animator.SetBool("MoveBack", false);
            return;
        }

        // 🔥 convert world velocity to local direction
        Vector3 localDir = transform.InverseTransformDirection(currentVelocity.normalized);

        animator.SetBool("MoveRight", localDir.x > 0.2f);
        animator.SetBool("MoveLeft", localDir.x < -0.2f);
        animator.SetBool("MoveForward", localDir.z > 0.2f);
        animator.SetBool("MoveBack", localDir.z < -0.2f);
    }
    //Attack Handle
    void ChooseActivePosition()
    {
        Vector3 randomDir = Random.insideUnitSphere;
        randomDir.y = 0;
        randomDir.Normalize();

        activeTargetPosition = playerPositon.position + randomDir * attackOffsetRadius;

        hasActiveTarget = true;
    }
    public void StartAttack()
    {

        animator.SetTrigger("Attack");
    }
    public void FinsihAttack()
    {
        EnemyStateManager.Instance.ReleaseAttack(this);

        isAttacking = false;
        hasActiveTarget = false;

        attackCooldownTimer = attackCooldown;

        recoveryTimer = Random.Range(recoveryDelayMin, recoveryDelayMax);
        recoveryInitialized = true;


        ChangeState(EnemyState.Recover);
       
        Debug.Log(gameObject.name + " finished attack and entered Recovery state.");
    }

    public void Recovery()
    {
        if (!recoveryInitialized)
            return;

        recoveryTimer -= Time.deltaTime;

        ApplyMovement(Vector3.zero);
        LookAtPlayer();

        if (recoveryTimer > 0f)
            return;

        recoveryInitialized = false;

        int decision = Random.Range(0, 3);

        if (decision > 0)
        {
            // 🔥 ลองขอ Active slot
            bool gotActive =
                EnemyStateManager.Instance.TryRequestActive(this);

            if (!gotActive)
            {
                // ถ้าไม่ได้ slot → กลับ Passive แทน
                EndAttack();
            }
        }
        else
        {
            EndAttack();
            return;
        }
    }
    public void EndAttack()
    {
        isAttacking = false;
        hasActiveTarget = false;
        EnemyStateManager.Instance.RequestPassive(this);
        Debug.Log(gameObject.name + " ended attack and returned to Passive state.");
    }


    //Coroutine
    IEnumerator ChangeStateDelay(float time)
    {
        yield return new WaitForSeconds(time);
        
    }
    IEnumerator AttackDelay(float time)
    {
        yield return new WaitForSeconds(time);

    }
    //Variable methode
    public void ResetEnemyState()
    {
        if(!aiActive) return;
        // stop attacking
        isAttacking = false;
        hasActiveTarget = false;

        // stop recovery
        recoveryInitialized = false;
        recoveryTimer = 0f;

        // stop thinking
        isThinking = false;
        thinkTimer = 0f;

        // stop movement
        currentVelocity = Vector3.zero;

        // clear animator attack trigger just in case
        if (animator != null)
            animator.ResetTrigger("Attack");

        // 🔥 release attack slot if holding one
        EnemyStateManager.Instance.ReleaseAttack(this);

        // ensure enemy is synced to passive safely
        EnemyStateManager.Instance.RequestPassive(this);
    }

}
