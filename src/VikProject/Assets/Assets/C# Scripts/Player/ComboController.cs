using System.Collections.Generic;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Animator animator;
    [SerializeField] private ComboNodeSO rootNode;
    [SerializeField] private WeaponHitBox weaponHitBox;

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

        // 🔥 รับ input ได้ตลอด
        inputBuffered = true;
        bufferedInput = input;

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

        // 🔥 ส่ง attackData ไป WeaponHitBox ทุกครั้งที่เปลี่ยนท่า
        if (weaponHitBox != null)
            weaponHitBox.SetAttackData(attack);

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

        // 🔥 อนุญาตให้ประมวลผล input หลัง allowComboTime
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

            // ไม่มีทางไป → ไม่รีเซ็ต รอให้ animation จบ
            inputBuffered = false;
        }

        // ========= ANIMATION FINISHED =========
        if (normalizedTime >= 1f)
        {
            // 🔥 เล่นจนจบเสมอ แล้วค่อย Reset
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
