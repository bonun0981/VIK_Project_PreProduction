using System.Collections.Generic;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Animator animator;
    [SerializeField] private ComboNodeSO rootNode;

    private ComboNodeSO currentNode;

    private float lastAttackTime;
    private bool isAttacking;
    private bool inputBuffered;
    private AttackInputType bufferedInput;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentNode = rootNode;
    }

    private void Update()
    {
        if (!isAttacking)
            return;

        HandleComboWindow();
    }

    #region INPUT

    public void ReceiveInput(AttackInputType input)
    {
        if (!isAttacking)
        {
            TryStartAttack(input);
            return;
        }

        // ถ้ากำลังโจมตีอยู่ -> buffer ไว้
        inputBuffered = true;
        bufferedInput = input;
    }

    #endregion

    #region START ATTACK

    private void TryStartAttack(AttackInputType input)
    {
        ComboNodeSO nextNode = currentNode.GetNextNode(input);

        if (nextNode == null)
            return;

        PlayNode(nextNode, true);
    }

    #endregion

    #region PLAY NODE

    private void PlayNode(ComboNodeSO node, bool isFirstHit)
    {
        currentNode = node;

        AttackDataSO attack = node.attack;

        float startTime = 0f;

        if (!isFirstHit)
        {
            float timeSinceLast = Time.time - lastAttackTime;

            if (timeSinceLast <= attack.comboDelayTime && attack.useStartPercent)
            {
                startTime = attack.startPercent;
            }
        }

        animator.CrossFade(
            attack.stateName,
            attack.transitionDuration,
            0,
            startTime
        );

        lastAttackTime = Time.time;
        isAttacking = true;
        inputBuffered = false;
    }

    #endregion

    #region COMBO LOGIC

    private void HandleComboWindow()
    {
        AttackDataSO attack = currentNode.attack;

        float elapsed = Time.time - lastAttackTime;

        // ถ้ามี input buffer และอยู่ใน combo window
        if (inputBuffered && elapsed <= attack.comboWindow)
        {
            ComboNodeSO nextNode = currentNode.GetNextNode(bufferedInput);

            if (nextNode != null)
            {
                PlayNode(nextNode, false);
                return;
            }
        }

        // ถ้าเลย combo window
        if (elapsed > attack.comboWindow + attack.earlyBufferTime)
        {
            if (attack.resetIfLate)
            {
                ResetCombo();
            }
        }
    }

    #endregion

    #region END ATTACK (Animation Event)

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void ResetCombo()
    {
        currentNode = rootNode;
        isAttacking = false;
        inputBuffered = false;
    }

    #endregion
}
