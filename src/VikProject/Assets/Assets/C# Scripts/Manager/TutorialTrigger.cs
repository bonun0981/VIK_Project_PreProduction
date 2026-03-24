using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public GameObject tutorialPanel;

    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;

            // เปิด popup
            tutorialPanel.SetActive(true);

            // pause game
            Time.timeScale = 0f;

            // เรียก popup
            tutorialPanel.GetComponent<TutorialPopup>().Show();

            // 🔥 ทำลาย trigger ทิ้ง (ครั้งเดียวพอ)
            Destroy(gameObject);
        }
    }
}