using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    private bool isShowing = false;

    public void Show()
    {
        isShowing = true;
    }

    void Update()
    {
        if (!isShowing) return;

        if (Input.anyKeyDown)
        {
            Close();
        }
    }

    void Close()
    {
        isShowing = false;

        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}