using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private GameObject gameOverMenu; // <--- เพิ่ม Canvas จบเกม

    [SerializeField] private int sceneNum = 1;

    private bool isGameOver = false; // <--- เช็กว่าจบเกมหรือยัง

    private void Start()
    {
        ShowCursor(false);
        // มั่นใจว่าเปิดเกมมาเมนูทั้งหมดต้องปิด
        pauseMenu?.SetActive(false);
        settingMenu?.SetActive(false);
        gameOverMenu?.SetActive(false);
    }

    public void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void Update()
    {
        // ถ้าจบเกมแล้ว ไม่ให้กด ESC เพื่อเปิด Pause Menu
        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingMenu != null && settingMenu.activeInHierarchy)
            {
                CloseAllMenus();
            }
            else
            {
                PauseMenuHandler();
            }
        }
    }

    // --- ฟังก์ชันใหม่สำหรับจบเกม ---
    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0; // หยุดเวลาเกม
        gameOverMenu?.SetActive(true); // แสดงหน้าจอจบเกม
        ShowCursor(true); // แสดงเมาส์
    }

    private void CloseAllMenus()
    {
        settingMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        gameOverMenu?.SetActive(false); // ปิดหน้าจบเกมด้วย (เผื่อใช้กรณี Restart)

        Time.timeScale = 1;
        isGameOver = false;
        ShowCursor(false);
    }

    public void PauseMenuHandler()
    {
        if (pauseMenu.activeInHierarchy)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            ShowCursor(false);
        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            ShowCursor(true);
        }
    }

    public void OpenSettingMenu()
    {
        settingMenu?.SetActive(true);
    }

    public void CloseSettingMenu()
    {
        settingMenu?.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        ShowCursor(true);
        SceneManager.LoadScene(sceneNum);
    }
}