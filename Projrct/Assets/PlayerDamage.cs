using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] float DamageAmount = 40f;
    [SerializeField]BoxCollider hitBox;
    
    private void Start()
    {
        hitBox=GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
          if(other.CompareTag("Enemy"))
          {
              Enemy enemy = other.GetComponent<Enemy>();
              if(enemy != null)
              {
                  enemy.TakeDamage(DamageAmount);
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
