using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [SerializeField]private GameObject pauseMenu;
    [SerializeField]private GameObject settingMenu;
    [SerializeField] private int sceneNum=1;
    private void Start()
    {
        ShowCursor(false); // เริ่มเกม → ซ่อนเมาส์
    }
    public void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ถ้า Settings เปิดอยู่ → ปิดทั้งหมด
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

    private void CloseAllMenus()
    {
        settingMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        Time.timeScale = 1;

        ShowCursor(false); // 🔒 กลับเข้าเกม → ซ่อนเมาส์
    }


    public void PauseMenuHandler()
    {
        if (pauseMenu.activeInHierarchy)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);

            ShowCursor(false); // 🔒 ซ่อนเมาส์
        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);

            ShowCursor(true); // 🔓 แสดงเมาส์
        }
    }

    public void OpenSettingMenu()
    {
        settingMenu.SetActive(true);
    }
    public void CloseSettingMenu()
    {
        settingMenu?.SetActive(false);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(sceneNum);
    }
}
