using UnityEngine;

public class AllyBrain : MonoBehaviour
{
    public enum AllyState
    {
        FollowPlayer,
        MoveToEnemy,
        AttackEnemy
    }

    public bool isActive = true;
    public AllyState currentState;

    public Transform player;
    public AllyMovement movement;
    public AllyTargeting targeting;
    public AllyCombat combat;

    void Start()
    {
        movement = GetComponent<AllyMovement>();
        targeting = GetComponent<AllyTargeting>();
        combat = GetComponent<AllyCombat>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isActive) return;

        switch (currentState)
        {
            case AllyState.FollowPlayer:
                HandleFollow();
                break;

            case AllyState.MoveToEnemy:
                HandleMoveToEnemy();
                break;

            case AllyState.AttackEnemy:
                HandleAttack();
                break;
        }
    }

    void HandleFollow()
    {
        movement.FollowPlayer(player);

        if (targeting.HasTarget())
            currentState = AllyState.MoveToEnemy;
    }

    void HandleMoveToEnemy()
    {
        if (!targeting.HasTarget())
        {
            currentState = AllyState.FollowPlayer;
            return;
        }

        movement.MoveTo(targeting.CurrentTarget.position);

        if (movement.IsInRange(targeting.CurrentTarget.position, combat.attackRange))
            currentState = AllyState.AttackEnemy;
    }

    void HandleAttack()
    {
        if (!targeting.HasTarget())
        {
            currentState = AllyState.FollowPlayer;
            return;
        }

        if (!movement.IsInRange(targeting.CurrentTarget.position, combat.attackRange))
        {
            currentState = AllyState.MoveToEnemy;
            return;
        }

        combat.Attack(targeting.CurrentTarget);
    }
}