using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene(2);
    }
    public void ExitButton()
    {
        Application.Quit();
    }         
}
