using UnityEngine;

public enum AttackInputType
{
    A,
    B
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
    public float comboWindow = 0.4f;      // เวลากดต่อได้
    public float comboDelayTime = 0.15f;  // ถ้ากดเร็วมากจะใช้ startPercent
    public float earlyBufferTime = 0.2f;  // กดก่อน animation จบได้กี่วิ

    [Header("Behavior")]
    public bool useStartPercent = true;   // ใช้ startPercent ไหม
    public bool resetIfLate = true;       // ถ้ากดช้าให้รีเซ็ตคอมโบ

    [Header("Damage")]
    public float damageMultiplier = 1f;
    public float knockbackMultiplier = 1f;
}
