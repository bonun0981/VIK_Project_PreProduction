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

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (!state.IsName(currentNode.attack.stateName))
            return;

        float normalizedTime = state.normalizedTime;

        // ❌ ยังไม่ถึง allow combo time → ignore เลย
        if (normalizedTime < currentNode.attack.allowComboTime)
            return;

        // ✅ ถึงช่วงอนุญาตแล้ว → ค่อย buffer
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

        if (!state.IsName(attack.stateName))
            return;

        float normalizedTime = state.normalizedTime;

        // 🔥 ช่วงที่ "อนุญาตให้ใช้ buffer"
        bool canProcessBufferedInput =
            normalizedTime >= attack.allowComboTime &&
            normalizedTime <= attack.comboWindow;

        // ========= PROCESS BUFFER =========
        if (inputBuffered && canProcessBufferedInput)
        {
            ComboNodeSO nextNode =
                currentNode.GetNextNode(bufferedInput);

            if (nextNode != null)
            {
                PlayNode(nextNode, false);
                return;
            }

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

    public void ResetCombo()
    {
        currentNode = rootNode;
        isAttacking = false;
        inputBuffered = false;
    }

    #endregion
}
