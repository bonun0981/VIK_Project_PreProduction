using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] float DamageAmount = 40f;
    [SerializeField]BoxCollider hitBox;
    Vector3 hitBoxDirection;
    GameObject player;
    private void Start()
    {
        hitBox=GetComponent<BoxCollider>();
        player = GameObject.FindWithTag("Player");  
    }
   
    private void OnTriggerEnter(Collider other)
    {
        hitBoxDirection = player.transform.forward; 
        if (other.CompareTag("Enemy"))
          {
              Enemy enemy = other.GetComponent<Enemy>();
              if(enemy != null)
              {
                  enemy.TakeDamage(DamageAmount,hitBoxDirection);
              }
            else
            {
                Debug.Log(other.name);
            }
        }

    }


  

    public void HitBoxOn()
    {
        hitBox.enabled = true;
    }

    public void HitBoxOff()
    {
        hitBox.enabled = false;
    }
}
