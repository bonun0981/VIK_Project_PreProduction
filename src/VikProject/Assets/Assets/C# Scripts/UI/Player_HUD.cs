using UnityEngine;
using UnityEngine.UI;

public class Player_HUD : MonoBehaviour
{
    public Slider hpSlider;
    public Slider easeHpSlider;

    [SerializeField] private Health playerHealth;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private float snapThreshold = 0.005f;

    private void Update()
    {
        if (playerHealth == null) return;

        float targetHp = playerHealth.currentHealth / playerHealth.maxHealth;

        // Instant front bar
        hpSlider.value = targetHp;

        // Smooth delayed bar
        if (easeHpSlider.value > targetHp)
        {
            easeHpSlider.value = Mathf.Lerp(
                easeHpSlider.value,
                targetHp,
                lerpSpeed * Time.deltaTime
            );

            // Snap when very close to avoid slow finish
            if (Mathf.Abs(easeHpSlider.value - targetHp) < snapThreshold)
            {
                easeHpSlider.value = targetHp;
            }
        }
        else
        {
            easeHpSlider.value = targetHp;
        }
    }
}