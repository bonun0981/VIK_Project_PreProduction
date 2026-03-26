using UnityEngine;

public class AllyAggroController : MonoBehaviour
{
    public Transform player;
    public AllyMovement movement;
    public AllyTargeting targeting;
    public AllyBrain brain;

    public float combatRange = 8f;
    public float leashDistance = 12f;

    private void Start()
    {
        movement = GetComponent<AllyMovement>();
        targeting = GetComponent<AllyTargeting>();
        brain = GetComponent<AllyBrain>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float playerDistance =
            Vector3.Distance(transform.position, player.position);

        if (playerDistance > leashDistance)
        {
            targeting.ClearTarget();

            brain.currentState = AllyBrain.AllyState.FollowPlayer;

            movement.FollowPlayer(player);

            return;
        }

        if (targeting.CurrentTarget != null)
        {
            return;
        }

        movement.FollowPlayer(player);
    }
}