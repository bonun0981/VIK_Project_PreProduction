using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "Scriptable Objects/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    public float radius;
    public float damage;
    public float knockback;
    public float hitStopDuration;
    public float duration;

    public LayerMask targetLayer;
    public GameObject skillPrefab;
}
