using UnityEngine;
using System.Collections.Generic; // ต้องมีบรรทัดนี้เพื่อใช้งาน List

public class ObjectRemover : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("ลาก Object ทั้งหมดที่ต้องการให้หายมาใส่ในรายการนี้")]
    public List<GameObject> targetsToRemove = new List<GameObject>();

    [Header("Removal Options")]
    [Tooltip("True = ลบทิ้งถาวร (Destroy), False = แค่ซ่อนไว้ (Disable)")]
    public bool destroyPermanently = true;

    [Tooltip("ตั้งเวลาถอยหลังก่อนจะหายไป (วินาที)")]
    public float delay = 0f;

    [Header("Trigger Settings")]
    public string targetTag = "Player";
    private bool hasTriggered = false;

    public void RemoveAll()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        // วนลูปจัดการกับทุก Object ใน List
        foreach (GameObject obj in targetsToRemove)
        {
            if (obj != null)
            {
                ExecuteRemoval(obj);
            }
        }

        // ถ้าใน List ว่างเปล่า ให้ลบตัวมันเอง (Option เสริม)
        if (targetsToRemove.Count == 0)
        {
            ExecuteRemoval(gameObject);
        }
    }

    private void ExecuteRemoval(GameObject target)
    {
        if (destroyPermanently)
        {
            Destroy(target, delay);
            Debug.Log(target.name + " will be destroyed in " + delay + "s");
        }
        else
        {
            if (delay <= 0)
            {
                target.SetActive(false);
            }
            else
            {
                // ใช้ Coroutine แทน Invoke เพื่อให้รองรับการส่งค่า target เข้าไปได้แม่นยำขึ้น
                StartCoroutine(DisableAfterDelay(target, delay));
            }
            Debug.Log(target.name + " will be disabled in " + delay + "s");
        }
    }

    // ฟังก์ชันช่วยหน่วงเวลาสำหรับ SetActive(false)
    private System.Collections.IEnumerator DisableAfterDelay(GameObject target, float time)
    {
        yield return new WaitForSeconds(time);
        if (target != null)
        {
            target.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            RemoveAll();
        }
    }
}