using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AiActiveRadian : MonoBehaviour
{
    public float activeRadius = 30f;
    public LayerMask enemyLayer;
    [SerializeField]private float flequency = 0.4f;
    void Start()
    {
        StartCoroutine(ScanLoop());
    }

    //Scan for enemies in range
    IEnumerator ScanLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(flequency);
        while (true)
        {
            ScanEnemy();
            yield return wait;
        }
    }
    public void ScanEnemy()
    {
        Collider[] hit = Physics.OverlapSphere(
            transform.position,
            activeRadius,
            enemyLayer
        );

        foreach (var e in hit)
        {
            EnemyMotherClass enemy = e.GetComponent<EnemyMotherClass>();
            if (enemy == null) continue;

            if (enemy.aiActive) continue;

            // 🔥 เช็คก่อนว่าเรามี slot ไหม
            if (EnemyStateManager.Instance.CanAddPassive())
            {
                enemy.aiActive = true;
                EnemyStateManager.Instance.AddToPassive(enemy);
            }
            else if (EnemyStateManager.Instance.CanAddActive())
            {
                enemy.aiActive = true;
                EnemyStateManager.Instance.AddToActive(enemy);
            }
            else
            {
                // ไม่มี slotเลย → อย่า activate
                continue;
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activeRadius);
    }

}
