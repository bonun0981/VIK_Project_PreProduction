using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] private Animator attackAnimation;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            attackAnimation.SetTrigger("Attack");
        }
    }
}
