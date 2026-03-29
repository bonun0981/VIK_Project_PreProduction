using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class ObjectActivator_MultiGroup : MonoBehaviour
{
    [Header("Group Settings")]
    [Tooltip("ลาก Game Object ตัวแม่ทุกกลุ่มที่ต้องการให้ทำงานพร้อมกันมาใส่ใน List นี้")]
    public List<GameObject> parentGroups = new List<GameObject>();

    [Header("Optimization Settings")]
    [Tooltip("จำนวนศัตรูที่จะให้เปิดต่อ 1 เฟรม (รวมทุกกลุ่มแล้ว)")]
    [Range(1, 15)]
    public int batchSize = 5;

    [Header("Trigger Settings")]
    public string playerTag = "Player";
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    void Awake()
    {
        if (GetComponent<BoxCollider>() != null)
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        // ปิดศัตรูตัวลูกในทุกกลุ่มทิ้งไว้ก่อนเพื่อ Save CPU ตั้งแต่เริ่ม
        foreach (GameObject group in parentGroups)
        {
            if (group != null)
            {
                foreach (Transform child in group.transform)
                {
                    child.gameObject.SetActive(false);
                }
                group.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (triggerOnce && hasTriggered) return;

            StartCoroutine(ActivateAllChildrenStaggered());
        }
    }

    IEnumerator ActivateAllChildrenStaggered()
    {
        hasTriggered = true;

        // 1. รวบรวมตัวลูกจาก "ทุกกลุ่ม" มาไว้ใน List เดียวกันก่อน
        List<GameObject> allEnemies = new List<GameObject>();
        foreach (GameObject group in parentGroups)
        {
            if (group != null)
            {
                foreach (Transform child in group.transform)
                {
                    allEnemies.Add(child.gameObject);
                }
            }
        }

        Debug.Log($"<color=cyan>Activating total {allEnemies.Count} enemies across {parentGroups.Count} groups...</color>");

        // 2. ทยอยเปิดตามจำนวน Batch ที่กำหนด
        int counter = 0;
        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i] != null)
            {
                allEnemies[i].SetActive(true);
                counter++;
            }

            // ถ้าเปิดครบจำนวน Batch ในเฟรมนั้นแล้ว ให้รอเฟรมถัดไป
            if (counter >= batchSize)
            {
                counter = 0;
                yield return null;
            }
        }

        Debug.Log("<color=white>All groups activated smoothly!</color>");
    }
}