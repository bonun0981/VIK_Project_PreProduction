using UnityEngine;
using System.Collections; // จำเป็นต้องใช้สำหรับ Coroutine

[RequireComponent(typeof(BoxCollider))]
public class StaggeredGroupActivator : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("ลาก GameObject ตัวแม่ (เช่น 'Enemy_Group1') มาใส่ในนี้")]
    public GameObject groupToActivate;

    [Header("Optimization Settings")]
    [Tooltip("จำนวนศัตรูที่จะให้เปิดต่อ 1 เฟรม (ยิ่งน้อยยิ่งลื่น, แนะนำ 5-10)")]
    public int batchSize = 5;

    [Header("Trigger Settings")]
    [Tooltip("Tag ของวัตถุที่จะมาเหยียบแล้วทำงาน")]
    public string playerTag = "Player";
    [Tooltip("ต้องการให้ทำงานแค่ครั้งเดียวหรือไม่?")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    void Awake()
    {
        // 1. ตั้งค่า Collider ให้เป็น Trigger อัตโนมัติ
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().isTrigger = true;
        }

        // 2. เช็คตัวแม่ และสั่งปิดตัวแม่ไว้ก่อนเพื่อไม่ให้ศัตรูทำงานตอนเริ่มเกม
        if (groupToActivate != null)
        {
            groupToActivate.SetActive(false);
        }
        else
        {
            Debug.LogError($"[StaggeredActivator] ไม่ได้ลากวัตถุตัวแม่ใส่ช่อง groupToActivate บนวัตถุ: {gameObject.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 3. ตรวจสอบ Tag และเช็คว่าเคยทำงานไปหรือยัง
        if (other.CompareTag(playerTag))
        {
            if (triggerOnce && hasTriggered) return;

            if (groupToActivate != null)
            {
                // เริ่มการทยอยเปิด (Coroutine)
                StartCoroutine(ActivateChildrenInBatches());
            }
        }
    }

    // 4. นี่คือหัวใจสำคัญ: ฟังก์ชัน Coroutine ที่ทยอยทำงาน
    IEnumerator ActivateChildrenInBatches()
    {
        hasTriggered = true;
        Debug.Log($"<color=green>[StaggeredActivator] Triggered on {gameObject.name}. Activating group: {groupToActivate.name}</color>");

        // 4a. เปิดตัวแม่ก่อนเพื่อให้เข้าถึงตัวลูกข้างในได้
        groupToActivate.SetActive(true);

        // 4b. วนลูปตรวจสอบ "ตัวลูก" ที่อยู่ในความดูแลของตัวแม่ทั้งหมด
        int activatedCount = 0;
        foreach (Transform child in groupToActivate.transform)
        {
            // ตรวจสอบความสมบูรณ์ของวัตถุลูก
            if (child != null && child.gameObject != null)
            {
                // เปิดใช้งานตัวลูก
                child.gameObject.SetActive(true);
                activatedCount++;

                // 4c. ถ้าเปิดครบจำนวน batchSize แล้ว ให้หยุดรอ 1 เฟรมก่อนทำต่อ
                if (activatedCount % batchSize == 0)
                {
                    yield return null; // หยุดรอตรงนี้ 1 เฟรม (ประมาณ 16.6ms ที่ 60fps)
                }
            }
        }

        Debug.Log($"<color=blue>[StaggeredActivator] All {activatedCount} enemies in {groupToActivate.name} are activated smoothly!</color>");
    }
}