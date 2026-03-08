using UnityEngine;
using System.Collections.Generic;

public class AllyTargeting : MonoBehaviour
{
    public float detectRange = 8f;
    public LayerMask enemyLayer;

    public Transform CurrentTarget;

    static Dictionary<Transform, int> enemyAttackCount =
        new Dictionary<Transform, int>();

    public int maxAttackersPerEnemy = 2;

    public bool HasTarget()
    {
        return CurrentTarget != null;
    }

    void Update()
    {
        if (CurrentTarget == null)
            FindTarget();
    }

    void FindTarget()
    {
        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectRange, enemyLayer);

        float closest = Mathf.Infinity;
        Transform best = null;

        foreach (var h in hits)
        {
            Transform enemy = h.transform;

            int count = 0;
            enemyAttackCount.TryGetValue(enemy, out count);

            if (count >= maxAttackersPerEnemy)
                continue;

            float d = (transform.position - enemy.position).sqrMagnitude;

            if (d < closest)
            {
                closest = d;
                best = enemy;
            }
        }

        if (best != null)
        {
            CurrentTarget = best;

            if (!enemyAttackCount.ContainsKey(best))
                enemyAttackCount[best] = 0;

            enemyAttackCount[best]++;
        }
    }

    public void ClearTarget()
    {
        if (CurrentTarget == null) return;

        enemyAttackCount[CurrentTarget] =
    Mathf.Max(0, enemyAttackCount[CurrentTarget] - 1);
        CurrentTarget = null;
    }
}