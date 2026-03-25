using UnityEngine;
using System.Collections.Generic; // จำเป็นต้องมีเพื่อใช้ List

public class MoveToWaypoints : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Object ที่ต้องการให้เคลื่อนที่ (ถ้าไม่ใส่จะใช้ตัวมันเอง)")]
    public GameObject objectToMove;

    [Tooltip("รายการจุดหมาย (Empty GameObjects)")]
    public List<GameObject> waypoints = new List<GameObject>();

    public float moveSpeed = 5f;
    public bool loop = false;

    private int _currentIndex = 0;
    private bool _isMoving = false;

    void Start()
    {
        // ถ้าไม่ได้ลาก Object มาใส่ ให้ใช้ตัวที่ถือสคริปต์นี้อยู่
        if (objectToMove == null)
        {
            objectToMove = this.gameObject;
        }
    }

    void Update()
    {
        // เริ่มเคลื่อนที่เมื่อมีจุดใน List อย่างน้อย 1 จุด
        if (waypoints.Count > 0 && _currentIndex < waypoints.Count)
        {
            Move();
        }
    }

    void Move()
    {
        // ตำแหน่งปัจจุบันของ Object และตำแหน่งของเป้าหมายใน List
        Vector3 currentPos = objectToMove.transform.position;
        Vector3 targetPos = waypoints[_currentIndex].transform.position;

        // คำนวณการเคลื่อนที่
        objectToMove.transform.position = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);

        // ตรวจสอบว่าถึงจุดหมายหรือยัง (ใช้ระยะห่างน้อยๆ แทนการเช็คค่าเป๊ะๆ)
        if (Vector3.Distance(objectToMove.transform.position, targetPos) < 0.01f)
        {
            _currentIndex++;

            // ถ้าเป็นระบบ Loop ให้กลับไปจุดแรกเมื่อถึงจุดสุดท้าย
            if (loop && _currentIndex >= waypoints.Count)
            {
                _currentIndex = 0;
            }
        }
    }

    // ฟังก์ชันเสริม: สั่งให้เริ่มเดินใหม่จากจุดแรก
    public void ResetMovement()
    {
        _currentIndex = 0;
    }
}