using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public void LoadLevelByIndex(int sceneIndex)
    {
        // 1. คืนค่าเวลาให้กลับมาเดินปกติ (1 เท่า) ก่อนเปลี่ยนฉาก
        Time.timeScale = 1;

        // 2. สั่งโหลด Scene ตาม Index
        SceneManager.LoadScene(sceneIndex);
    }

    // แถม: ฟังก์ชันสำหรับ Reload ด่านเดิม (ใช้บ่อยในหน้า GameOver)
    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}