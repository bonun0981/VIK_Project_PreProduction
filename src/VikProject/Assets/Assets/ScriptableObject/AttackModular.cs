using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackModular", menuName = "Scriptable Objects/AttackModular")]
public class AttackModular : ScriptableObject
{
    public AnimatorOverrideController animatorOverride;
    public float damage;
    public float knockBackForce;
    public enum AttackType
    {
        Normal,
        Lift,
        Knockdown
    }
    public AttackType type;

}
