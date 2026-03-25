using UnityEngine;
using System.Collections.Generic; // ต้องมีเพื่อใช้ List

[RequireComponent(typeof(BoxCollider))]
public class EventBomb : MonoBehaviour
{
    [Header("Particle Settings")]
    [Tooltip("รายการ Particle แบบที่ 1: เล่นครั้งเดียวแล้วลบทิ้ง (เช่น เอฟเฟกต์ระเบิดหลายจุด)")]
    public List<GameObject> particlesOnce = new List<GameObject>();
    public float destroyDelay = 5f;

    [Tooltip("รายการ Particle แบบที่ 2: เล่นค้างไว้ตลอดไป (เช่น ไฟ, ควัน หลายจุด)")]
    public List<GameObject> particlesLoop = new List<GameObject>();

    [Header("Audio Settings")]
    [Tooltip("ลาก Audio Source ที่ตั้งค่าเสียงไว้แล้วมาใส่ที่นี่")]
    public AudioSource audioToPlay;

    [Header("Objects to Manage")]
    [Tooltip("วัตถุที่จะถูกลบเมื่อเดินชน (เช่น FogWall)")]
    public GameObject objectToRemove;

    [Header("Trigger Settings")]
    public string targetTag = "Player";

    private bool hasTriggered = false;

    void Awake()
    {
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && !hasTriggered)
        {
            ExecuteEvent();
        }
    }

    void ExecuteEvent()
    {
        hasTriggered = true;
        Debug.Log("Trigger Activated!");

        // 1. เล่นเสียง
        if (audioToPlay != null)
        {
            audioToPlay.Play();
        }

        // 2. จัดการ List ของ Particle Once (เล่นแล้วลบ)
        foreach (GameObject pOnce in particlesOnce)
        {
            if (pOnce != null)
            {
                pOnce.SetActive(true);
                ParticleSystem ps = pOnce.GetComponentInChildren<ParticleSystem>();
                if (ps != null) ps.Play();

                Destroy(pOnce, destroyDelay);
            }
        }

        // 3. จัดการ List ของ Particle Loop (เล่นค้างไว้)
        foreach (GameObject pLoop in particlesLoop)
        {
            if (pLoop != null)
            {
                pLoop.SetActive(true);
                ParticleSystem ps = pLoop.GetComponentInChildren<ParticleSystem>();
                if (ps != null) ps.Play();
            }
        }

        // 4. จัดการ Object ที่ต้องการเอาออก
        if (objectToRemove != null)
        {
            HandleObjectRemoval();
        }
    }

    void HandleObjectRemoval()
    {
        Collider col = objectToRemove.GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        ParticleSystem ps = objectToRemove.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop();
            Destroy(objectToRemove, 5f);
        }
        else
        {
            Destroy(objectToRemove);
        }
    }
}