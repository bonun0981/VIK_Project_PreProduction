using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackModular", menuName = "Scriptable Objects/AttackModular")]
public class AttackModular : ScriptableObject
{
    public AnimatorOverrideController animatorOverride;
    public float damage;
    public float knockBackForce;
    public float startTime;
    public float timimgWindow;
    public AnimationClip attackAnimation;
    public enum AttackType
    {
        Normal,
        Lift,
        Knockdown
    }
    public AttackType type;

}
