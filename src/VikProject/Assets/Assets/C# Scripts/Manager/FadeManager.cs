using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class FadeManager : MonoBehaviour
{
    

    public Image fadeImage;
    public float fadeSpeed = 2f;

    bool isTransitioning = false;

    private void Start()
    {
        ShowCursor(); // เรียกใช้ตอนเริ่ม
    }

    void Awake()
    {
        
            ShowCursor(); // เรียกใช้เพื่อให้มั่นใจ
            SetAlpha(0f);
        
        
    }
     
    // ฟังก์ชันสำหรับเปิดเมาส์ (สร้างแยกไว้จะได้เรียกใช้ง่ายๆ)
    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // ปลดล็อกเมาส์ให้ขยับได้อิสระ
    }

    IEnumerator FadeToBlack()
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.unscaledDeltaTime * fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1f);
    }

    IEnumerator FadeFromBlack()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.unscaledDeltaTime * fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);
    }

    void SetAlpha(float a)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = a;
            fadeImage.color = c;
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(Transition(sceneName));
    }

    IEnumerator Transition(string sceneName)
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(sceneName);

        // หลังจากโหลด Scene ใหม่เสร็จ ถ้าเป็นหน้าเมนู ให้เปิดเมาส์อีกรอบ
        ShowCursor();

        yield return new WaitForSeconds(0.1f); // รอเล็กน้อยเพื่อให้ Scene โหลด Object ครบ
        yield return StartCoroutine(FadeFromBlack());

        isTransitioning = false;
    }
}