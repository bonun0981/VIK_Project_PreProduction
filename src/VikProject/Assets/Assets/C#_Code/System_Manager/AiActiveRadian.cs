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
        Collider[] hit = Physics.OverlapSphere(transform.position, activeRadius, enemyLayer);
        foreach (var e in hit)
        {
           EnemyMotherClass enemy = e.GetComponent<EnemyMotherClass>();
           if (enemy==null) continue;
           if(enemy.aiActive) continue;

           enemy.aiActive = true;
            if (EnemyStateManager.Instance.CanAddPassive())
            {
                EnemyStateManager.Instance.AddToPassive(enemy);
            }
            else if (EnemyStateManager.Instance.CanAddActive())
            {
                EnemyStateManager.Instance.AddToActive(enemy);
            }

        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activeRadius);
    }

}
