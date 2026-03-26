using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOutOnStart : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 2f;

    void Start()
    {
        // 🔒 ซ่อนเมาส์ตอนเข้าเกม
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float alpha = 1f;
        SetAlpha(1f);

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
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}