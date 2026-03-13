using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AiActiveRadian : MonoBehaviour
{
    public float activeRadius = 30f;
    public LayerMask enemyLayer;

    [SerializeField] private float frequency = 0.4f;

    void Start()
    {
        StartCoroutine(ScanLoop());
    }

    IEnumerator ScanLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(frequency);

        while (true)
        {
            ScanEnemy();
            yield return wait;
        }
    }

    void ScanEnemy()
    {
        Collider[] hit = Physics.OverlapSphere(
            transform.position,
            activeRadius,
            enemyLayer
        );

        foreach (var col in hit)
        {
            EnemyNormal enemy = col.GetComponent<EnemyNormal>();
            if (enemy == null) continue;

            // ถ้า AI เปิดแล้ว ข้าม
            if (enemy.aiActive) continue;

            ActivateEnemy(enemy);
        }
    }

    void ActivateEnemy(EnemyNormal enemy)
    {
        if (!EnemyCombatDirector.Instance.HasSlot())
            return;

        enemy.aiActive = true;

        enemy.SetPlayer(transform);

        EnemyCombatDirector.Instance.RegisterEnemy(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activeRadius);
    }

}
