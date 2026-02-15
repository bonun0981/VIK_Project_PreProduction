using UnityEngine;

[CreateAssetMenu(fileName = "AttackDataSO", menuName = "Scriptable Objects/AttackDataSO")]
public class AttackDataSO : ScriptableObject
{
    [SerializeField] float damageMultiplier = 1f;
    [SerializeField] float knockbackMultiplier = 1f;

    public float DamageMultiplier => damageMultiplier;
    public float KnockbackMultiplier => knockbackMultiplier;
}
