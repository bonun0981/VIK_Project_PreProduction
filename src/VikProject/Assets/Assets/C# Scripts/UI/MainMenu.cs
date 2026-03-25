using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField]private int sceneNum = 0;
    public void StartButton()
    {
        SceneManager.LoadScene(sceneNum);
    }
    public void ExitButton()
    {
        Application.Quit();
    }         
}
