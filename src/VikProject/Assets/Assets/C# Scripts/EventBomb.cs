using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EventBomb : MonoBehaviour
{
    [Header("Particle Settings")]
    [Tooltip("Particle แบบที่ 1: เล่นครั้งเดียวแล้วลบทิ้ง")]
    public GameObject particleOnce;
    public float destroyDelay = 5f;

    [Tooltip("Particle แบบที่ 2: เล่นค้างไว้ตลอดไป")]
    public GameObject particleLoop;

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
        // บังคับให้ Collider เป็น Trigger เสมอ
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

        // 1. เล่น Audio Source ที่ลากมาใส่
        if (audioToPlay != null)
        {
            audioToPlay.Play();
        }

        // 2. จัดการ Particle Once (เปิด + สั่ง Play + ลบทิ้ง)
        if (particleOnce != null)
        {
            particleOnce.SetActive(true);
            // สั่ง Play ซ้ำเพื่อความชัวร์ (เผื่อไม่ได้ติ๊ก Play On Awake)
            ParticleSystem ps = particleOnce.GetComponentInChildren<ParticleSystem>();
            if (ps != null) ps.Play();

            Destroy(particleOnce, destroyDelay);
        }

        // 3. จัดการ Particle Loop (เปิด + สั่ง Play ค้างไว้)
        if (particleLoop != null)
        {
            particleLoop.SetActive(true);
            ParticleSystem ps = particleLoop.GetComponentInChildren<ParticleSystem>();
            if (ps != null) ps.Play();
        }

        // 4. ลบ Object (เช่น FogWall)
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