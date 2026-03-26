using UnityEngine;

public class AnimationLOD : MonoBehaviour
{
    public Animator animator;
    public Transform player;
    public float lodDistance = 10f; // ระยะที่จะเริ่มลดเฟรมเรต

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > lodDistance)
        {
            // ถ้าอยู่ไกล ให้รันแอนิเมชันแค่ 1 ใน 4 เฟรม (ประมาณ 15 FPS ถ้าเกมรัน 60)
            animator.enabled = (Time.frameCount % 4 == 0);
        }
        else
        {
            animator.enabled = true; // อยู่ใกล้ให้รันเต็ม 60 FPS
        }
    }
}