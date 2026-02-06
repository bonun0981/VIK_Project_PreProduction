using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class EnemyActiveRadian : MonoBehaviour
{

    public float activeRadius = 30f;
    public LayerMask enemyLayer;

    HashSet<EnemyBase> currentEnemies = new();

    void Start()
    {
        StartCoroutine(ScanLoop());
    }

    IEnumerator ScanLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(0.4f);

        while (true)
        {
            Scan();
            yield return wait;
        }
    }

    void Scan()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            activeRadius,
            enemyLayer
        );

        HashSet<EnemyBase> newSet = new();

        foreach (var h in hits)
        {
            EnemyBase e = h.GetComponent<EnemyBase>();
            if (e == null) continue;

            newSet.Add(e);
            if (!EnemyManager.Instance.CanEngage(this))
                return;
            if (!currentEnemies.Contains(e))
                e.SetAIActive(true);
        }

        // ตัวที่หลุดระยะ
        foreach (var e in currentEnemies)
        {
            if (!newSet.Contains(e))
                e.SetAIActive(false);
        }

        currentEnemies = newSet;
        if (currentEnemies.Count == 0)
        {
            EnemyManager.Instance.ReleaseEngagement(this);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activeRadius);
    }
#endif

}
