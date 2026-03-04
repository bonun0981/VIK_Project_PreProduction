using UnityEngine;

public class AllyTargeting : MonoBehaviour
{
    public float detectRange = 7f;
    public LayerMask enemyLayer;

    public Transform CurrentTarget { get; private set; }

    public bool HasTarget()
    {
        if (CurrentTarget == null) return false;

        if (Vector3.Distance(transform.position, CurrentTarget.position) > detectRange)
        {
            CurrentTarget = null;
            return false;
        }

        return true;
    }

    void Update()
    {
        if (CurrentTarget != null) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, enemyLayer);

        float closest = Mathf.Infinity;
        Transform nearest = null;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closest)
            {
                closest = dist;
                nearest = hit.transform;
            }
        }

        CurrentTarget = nearest;
    }

    // =========================
    // 🧪 GIZMO DEBUG SECTION
    // =========================
    private void OnDrawGizmosSelected()
    {
        // วาดวงกลมระยะตรวจจับ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // ถ้ามี Target ให้วาดเส้นไปหา
        if (CurrentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, CurrentTarget.position);

            Gizmos.DrawWireSphere(CurrentTarget.position, 0.4f);
        }
    }



}