using UnityEngine;

public class AllyAggroController : MonoBehaviour
{
    public Transform player;
    public AllyMovement movement;
    public AllyTargeting targeting;

    public float combatRange = 8f;
    public float leashDistance = 12f;

    private void Start()
    {
        movement = GetComponent<AllyMovement>();
        targeting = GetComponent<AllyTargeting>();
       

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float playerDistance =
            Vector3.Distance(transform.position, player.position);

        // 🔴 Player หนีไกลเกิน → ยกเลิก combat
        if (playerDistance > leashDistance)
        {
            targeting.ClearTarget();
            movement.FollowPlayer(player);
            return;
        }

        // 🟡 มี target และ player ยังอยู่ใกล้
        if (targeting.currentTarget != null)
        {
            movement.MoveTo(targeting.currentTarget.position);
            return;
        }

        // 🟢 ไม่มี target → follow player
        movement.FollowPlayer(player);
    }
}
