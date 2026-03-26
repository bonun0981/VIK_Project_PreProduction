using UnityEngine;
using System.Collections.Generic;

public class AreaGateController : MonoBehaviour
{
    [Header("Assign enemies or objects here")]
    public List<GameObject> targets = new List<GameObject>();

    private BoxCollider gateCollider;

    void Awake()
    {
        gateCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        // ลบ object ที่ถูกปิด (inactive) หรือ null
        targets.RemoveAll(item => item == null || !item.activeInHierarchy);

        // ถ้าไม่มีเหลือ → เปิดทาง
        if (targets.Count == 0)
        {
            OpenGate();
        }
    }

    void OpenGate()
    {


        // กันไม่ให้เรียกซ้ำ
        enabled = false;

        Debug.Log("Gate Opened!");
        Destroy(gameObject);
    }
}