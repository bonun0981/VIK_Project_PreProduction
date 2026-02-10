using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    [SerializeField] List<AttackModular> attackSequence;
    float lastClickTime;
    float lastComboEnd;
    int comboStep;
    [SerializeField]Animator animator;
    [SerializeField] PlayerDamage playerDamage;
    bool isAttacking;
    bool attackQueued;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                StartAttack();
            }
            else
            {
                attackQueued = true;
            }
        }
    }
    public void Attack()
    {
        if(Time.time - lastComboEnd > 0.5f &&comboStep<=attackSequence.Count)
        {
            CancelInvoke("ResetCombo");
            if(Time.time - lastClickTime >= 0.2f)
            {
                animator.runtimeAnimatorController = attackSequence[comboStep].animatorOverride;
                animator.Play("Attack",0,0.5f);
                playerDamage.DamageAmount= attackSequence[comboStep].damage;
                comboStep++;
                lastClickTime=Time.time;
                if(comboStep>=attackSequence.Count)
                {
                    comboStep=0;
                }
            }
        }
        

    }

    void StartAttack()
    {
        isAttacking = true;

        animator.runtimeAnimatorController =
            attackSequence[comboStep].animatorOverride;

        animator.Play("Attack", 0, 0);

        playerDamage.DamageAmount =
            attackSequence[comboStep].damage;
    }
    public void TryCombo()
    {
        if (attackQueued && comboStep < attackSequence.Count - 1)
        {
            attackQueued = false;
            comboStep++;
            StartAttack();
        }
        else
        {
            isAttacking = false;
            attackQueued = false;
            comboStep = 0;
        }
    }
    public void ExitAttack()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime>=0.9f
            &&animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("ResetCombo",1f);
        }
    }
    public void ResetCombo()
    {
        comboStep=0;
        lastComboEnd=Time.time;
    }
    public void InvokeTest()
    {
        Debug.Log("fire invoke methode");
    }
}
