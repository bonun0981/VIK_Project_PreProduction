using System.Collections;
using UnityEngine;

public class EnemyMotherClass : MonoBehaviour
{
   enum EnemyState
   {
        Idel,
        Patrol,
        Passive,
        Active,
        Recover
   }
    EnemyState state;

    //Enemy active status
    public bool aiActive;
    public bool haveSlot;


    //Enemy Timers
    public float attackDelayTime;
    public float thinkTime;

    //Enemy Bools
    public bool isAttacking;

    //Enemy offsets



    //Player
    public Transform playerPositon;
    private GameObject player;
    

    // start and update methods
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }
    private void Update()
    {



        Act();
    }



    //State change method 


    //Enemy Action Handle
    void Act()
    {
        switch (state)
        {
            case EnemyState.Idel:
               IdelMovement();
                break;

            case EnemyState.Passive:
                PassiveMovement();
                break;

            case EnemyState.Active:
                MoveToward();
                break;

            case EnemyState.Recover:
                Recovery();
                break;

           
        }
    }
    //Movement Handle
    public void IdelMovement()
    {

    }

    public void PassiveMovement()//handle enemy passive movement
    {
        //decide movement type
    }
    public void MoveInAndOut()//random move toward and away from player
    {
        //when finish go to passive movement to decide next movement
    }
    public void CircleAround()//enemy circling player left and right
    {
        //when finish go to passive movement to decide next movement
    }
    public void MoveToward()//directly move toward player for perform attack
    {
        //when in attack range start attack
        StartAttack();
    }
    //Attack Handle
    public void StartAttack()
    {



       
        state = EnemyState.Recover;
    }

    public void Recovery()
    {
        //Decide to continue attack and stay active or end attack and go passive
        int continueAttack = Random.Range(0, 3);
        if (continueAttack > 0)
        {
            StartAttack();
        }
        else
        {
            EndAttack();
        }

    }
    public void EndAttack()
    {
        
    }


    //Coroutine
    IEnumerator ChangeStateDelay(float time)
    {
        yield return new WaitForSeconds(time);
        
    }
    IEnumerator AttackDelay(float time)
    {
        yield return new WaitForSeconds(time);

    }
    //Variable methode


}
