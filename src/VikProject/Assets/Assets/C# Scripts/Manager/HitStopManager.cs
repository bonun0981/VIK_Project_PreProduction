using System.Collections;
using UnityEngine;
public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    private bool isInHitStop;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void DoHitStop(float duration)
    {
        if (!isInHitStop)
            StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        isInHitStop = true;

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isInHitStop = false;
    }
}
