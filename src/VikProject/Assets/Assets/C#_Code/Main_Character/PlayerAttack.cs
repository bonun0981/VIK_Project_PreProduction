using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]Animator attackAnimation;
    [SerializeField]BoxCollider hitBox;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            attackAnimation.SetTrigger("Attack");
        }
    }


    private void Start()
    {
          attackAnimation= GetComponent<Animator>();
    }
    public void FirstAttack()
    {
       
        attackAnimation.SetInteger("Combo", 1);
    }

    public void SecondAttack()
    {
        
        attackAnimation.SetInteger("Combo", 2);
    }

    public void ThirdAttack()
    {
        
        attackAnimation.SetInteger("Combo", 3);
    }

    public void ResetAttack()
    {
        //attackAnimation.applyRootMotion = false;
        attackAnimation.SetInteger("Combo", 0);
        attackAnimation.SetBool("CanAttack", false);
    }
    public void StartAttack ()
    {
        //attackAnimation.applyRootMotion = true;
        attackAnimation.SetBool("CanAttack", true);
    }

    public void EnableHitBox()
    {
        hitBox.enabled = true;
    }
    public void DisableHitBox()
    {
        hitBox.enabled = false;
    }


    public void DebugTest()
    {
        Debug.Log("Animaiton event has triggers");
    }
    public void DebugTestend()
    {
        Debug.Log("second Animaiton event has triggers ");
    }
}
