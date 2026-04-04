using UnityEngine;

public class MainMenuSetup : MonoBehaviour
{
    void Awake()
    {
        // บังคับให้เมาส์แสดงผลทันทีที่โหลด Scene นี้
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // แถม: คืนค่าเวลาเผื่อกรณีโหลดมาจากหน้า Pause
        Time.timeScale = 1f;
    }
}