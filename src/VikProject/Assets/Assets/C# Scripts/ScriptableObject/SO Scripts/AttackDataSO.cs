using UnityEngine;

public enum AttackStatusEffect
{
    None,
    Stun,
    Knockback,
    KnockUp
}
public enum AttackInputType
{
    A,
    B,
    Skill,
    Ultimate
}
[CreateAssetMenu(fileName = "AttackDataSO", menuName = "Scriptable Objects/AttackDataSO")]
public class AttackDataSO : ScriptableObject
{
    [Header("Animator")]
    public string stateName;               // ชื่อ state ใน Animator
    [Range(0f, 1f)]
    public float startPercent = 0f;       // เล่น animation เริ่มที่ %
    public float transitionDuration = 0.1f;

   
    [Header("Timing")]
    [Range(0f, 1f)]
    public float allowComboTime = 0.3f;
    public float comboWindow = 0.6f;      // chain ได้ถึง %

    public float comboDelayTime = 0.15f;
    public float earlyBufferTime = 0.2f;

    public GameObject hitEffect;
    public float hitstopTime;// เอฟเฟกต์ตอน chain สำเร็จ
                                                    // เริ่มรับ input ต่อได้เมื่อถึง %



    [Header("Behavior")]
    public bool useStartPercent = true;   // ใช้ startPercent ไหม
    public bool resetIfLate = true;       // ถ้ากดช้าให้รีเซ็ตคอมโบ

    [Header("Damage")]
    public float damageMultiplier = 1f;
    public float statusPower = 1f;
    public AttackStatusEffect statusEffect;
}
