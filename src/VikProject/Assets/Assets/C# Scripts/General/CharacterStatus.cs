using UnityEngine;

public class CharacterStatus : MonoBehaviour,
    IStunnable,
    IKnockbackable,
    IKnockUpable

{
    bool isKnockback;
    bool isStunned;
    bool isKnockUp;



    public bool IsKnockBack => isKnockback;
    public bool IsStunned => isStunned;

    public bool IsKnockUp => isKnockUp;
    public bool IsMovementInterrupt => isKnockback || isStunned||isKnockUp;

    

    public void Knockback(float force)
    {
        isKnockback = true;
        // apply force
        //StartCoroutine(KnockbackRoutine());
    }

    public void Stun(float duration)
    {
        isStunned = true;
        //StartCoroutine(StunRoutine(duration));
    }

    public void KnockUp(float force)
    {
        isKnockUp = true;
        // apply force
       
    }
    //IEnumerator KnockbackRoutine()
    //{
    //    yield return new WaitForSeconds(0.3f);
    //    isKnockback = false;
    //}

    //IEnumerator StunRoutine(float duration)
    //{
    //    yield return new WaitForSeconds(duration);
    //    isStunned = false;
    //}

}
