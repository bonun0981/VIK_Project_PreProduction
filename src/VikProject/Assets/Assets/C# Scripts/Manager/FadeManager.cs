using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public Image fadeImage;
    public float fadeSpeed = 2f;

    bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // เริ่มต้นโปร่งใส
            SetAlpha(0f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔥 Fade ไปดำ
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

    // 🔥 Fade ออกจากดำ
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
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }

    // 🔥 เรียกตอนกด Play
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

        yield return null;

        yield return StartCoroutine(FadeFromBlack());

        isTransitioning = false;
    }
}