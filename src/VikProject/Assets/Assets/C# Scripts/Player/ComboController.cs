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
    private bool comboWindowOpen;

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
        if (currentNode == null)
            return;

        AttackDataSO attack = currentNode.attack;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // 🔥 ป้องกันข้าม state ผิดตัว
        if (!state.IsName(attack.stateName))
            return;

        float normalizedTime = state.normalizedTime;

        bool isLeafNode =
            currentNode.transitions == null ||
            currentNode.transitions.Count == 0;

        // 🔥 เปิด combo window ช่วงท้าย animation เท่านั้น
        bool windowOpen =
            normalizedTime >= attack.comboWindow &&
            normalizedTime < 1f;

        // ========= COMBO CONTINUE =========
        if (inputBuffered && windowOpen)
        {
            ComboNodeSO nextNode =
                currentNode.GetNextNode(bufferedInput);

            if (nextNode != null)
            {
                PlayNode(nextNode, false);
                return;
            }

            // ถ้า input ไม่ถูก route → ignore แต่ไม่ reset
            inputBuffered = false;
        }

        // ========= ANIMATION FINISHED =========
        if (normalizedTime >= 1f)
        {
            ResetCombo();
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
