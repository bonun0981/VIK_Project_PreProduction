using UnityEngine;

public enum SkillSpawnType
{
    AtCaster,
    AtWeapon,
    CustomPoint,
    Projectile,
    AttachedToTarget
}

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "Scriptable Objects/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    public SkillSpawnType spawnType;

    public float radius;
    public float damage;
    public float knockback;
    public float hitStopDuration;

    public float duration;

    // projectile behaviour
    public float moveDuration = 2f;
    public float stayDuration = 2f;

    public LayerMask targetLayer;
    public GameObject skillPrefab;
}