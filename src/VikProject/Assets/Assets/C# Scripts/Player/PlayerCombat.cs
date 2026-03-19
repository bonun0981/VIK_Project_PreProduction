using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private InputHandler input;
    [SerializeField] private ComboController comboController;

    private void Awake()
    {
        input = GetComponent<InputHandler>();
        comboController = GetComponent<ComboController>();
    }
    private void Update()
    {
        HandleAttackInput();
    }

    private void HandleAttackInput()
    {
        if (input.isAttackA)
        {
            comboController.ReceiveInput(AttackInputType.A);
        }
        if (input.isAttackB)
        {
            comboController.ReceiveInput(AttackInputType.B);
        }
        if(input.isSkill)
        {
            comboController.ReceiveInput(AttackInputType.Skill);
        }
        if(input.isUltimate)
        {
            comboController.ReceiveInput(AttackInputType.Ultimate);
        }

    }
}
