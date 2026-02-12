using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public static EnemyStateManager Instance;
    //Number of enemy slot for each state
    [SerializeField]private int maxActiveEnemies = 3;
    [SerializeField]private int maxPassiveEnemies = 5;
    [SerializeField] private int maxAttackingEnemies = 1;
    //Lists to hold enemies in each state
    [SerializeField] private List<EnemyMotherClass> activeEnemies = new();
    [SerializeField] private List<EnemyMotherClass> passiveEnemies = new();
    [SerializeField] private List<EnemyMotherClass> currentGroup = new();
    private List<EnemyMotherClass> attackingEnemies = new();

    [SerializeField] private float globalAttackDelay = 0.8f;
    private float globalAttackTimer = 0f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    private void Update()
    {
        if (globalAttackTimer > 0f)
            globalAttackTimer -= Time.deltaTime;
        MaintainActiveSlots();
    }
    private void MaintainActiveSlots()
    {
        if (activeEnemies.Count >= maxActiveEnemies)
            return;

        if (passiveEnemies.Count == 0)
            return;

        int needed = maxActiveEnemies - activeEnemies.Count;

        for (int i = 0; i < needed; i++)
        {
            if (passiveEnemies.Count == 0)
                return;

            int index = Random.Range(0, passiveEnemies.Count);
            EnemyMotherClass next = passiveEnemies[index];

            RequestActive(next);
        }
    }
    //assgin enemy to each state
    public void RequestPassive(EnemyMotherClass enemy)
    {
        if (!currentGroup.Contains(enemy))
            currentGroup.Add(enemy);

        activeEnemies.Remove(enemy);

        if (!passiveEnemies.Contains(enemy))
            passiveEnemies.Add(enemy);

        enemy.state = EnemyMotherClass.EnemyState.Passive;

        TryPromotePassive();
    }
    public void RequestActive(EnemyMotherClass enemy)
    {
        if (!currentGroup.Contains(enemy)) return;
        if (activeEnemies.Contains(enemy)) return;

        if (activeEnemies.Count >= maxActiveEnemies)
            return;

        passiveEnemies.Remove(enemy);

        activeEnemies.Add(enemy);
        enemy.state = EnemyMotherClass.EnemyState.Active;
    }
    private void TryPromotePassive()
    {
        if (activeEnemies.Count >= maxActiveEnemies)
            return;

        if (passiveEnemies.Count == 0)
            return;

        int index = Random.Range(0, passiveEnemies.Count);
        EnemyMotherClass next = passiveEnemies[index];

        RequestActive(next);
    }
    public bool TryRequestActive(EnemyMotherClass enemy)
    {
        if (!currentGroup.Contains(enemy))
            currentGroup.Add(enemy);

        // 🔥 ensure not duplicated anywhere
        passiveEnemies.Remove(enemy);
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count >= maxActiveEnemies)
        {
            // put back to passive safely
            if (!passiveEnemies.Contains(enemy))
                passiveEnemies.Add(enemy);

            enemy.state = EnemyMotherClass.EnemyState.Passive;
            return false;
        }

        activeEnemies.Add(enemy);
        enemy.state = EnemyMotherClass.EnemyState.Active;
        return true;
    }

    //set enemy state after the scan from player
    public void AddToPassive(EnemyMotherClass enemy)
    {
        if (passiveEnemies.Contains(enemy)) return;
        if (passiveEnemies.Count >= maxPassiveEnemies) return;

        if (!currentGroup.Contains(enemy))
            currentGroup.Add(enemy);

        activeEnemies.Remove(enemy);

        passiveEnemies.Add(enemy);
        enemy.state = EnemyMotherClass.EnemyState.Passive;
    }
    public void AddToActive(EnemyMotherClass enemy)//set enemy to active state
    {
        
        if (activeEnemies.Contains(enemy)) return;
        if (activeEnemies.Count >= maxActiveEnemies) return;

        if (!currentGroup.Contains(enemy))
            currentGroup.Add(enemy);

        passiveEnemies.Remove(enemy);

        activeEnemies.Add(enemy);
        enemy.state = EnemyMotherClass.EnemyState.Active;
    }
    //attack slot handler
    public bool RequestAttack(EnemyMotherClass enemy)
    {
        if (!activeEnemies.Contains(enemy))
            return false;

        if (attackingEnemies.Contains(enemy))
            return true;

        
        if (globalAttackTimer > 0f)
            return false;

        if (attackingEnemies.Count >= maxAttackingEnemies)
            return false;

        attackingEnemies.Add(enemy);
        return true;
    }
    public void ReleaseAttack(EnemyMotherClass enemy)
    {
        if (attackingEnemies.Contains(enemy))
        {
            attackingEnemies.Remove(enemy);

            
            globalAttackTimer = globalAttackDelay;
        }
    }
    //check is there is available slot for each state
    public bool CanAddActive()
    {
        return activeEnemies.Count < maxActiveEnemies;
    }

    public bool CanAddPassive()
    {
        return passiveEnemies.Count < maxPassiveEnemies;
    }

}
