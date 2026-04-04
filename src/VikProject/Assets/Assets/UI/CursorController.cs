using UnityEngine;

public class CursorController : MonoBehaviour
{
    void Start()
    {
        // สั่งให้เมาส์แสดงผล
        Cursor.visible = true;

        // ปลดล็อกเมาส์ให้เคลื่อนที่อิสระ (ไม่โดนล็อกไว้กลางจอ)
        Cursor.lockState = CursorLockMode.None;
    }
}