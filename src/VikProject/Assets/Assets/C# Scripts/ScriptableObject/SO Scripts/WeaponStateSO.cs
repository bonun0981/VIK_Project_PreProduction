using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStateSO", menuName = "Scriptable Objects/WeaponStateSO")]
public class WeaponStateSO : ScriptableObject
{
    [SerializeField] float damage;
    [SerializeField] float knockback;

    public float baseDamage => damage;
    public float baseKnockback => knockback;
}
