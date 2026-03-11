using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCombatDirector : MonoBehaviour
{
    [Header("Attack Turn Logic")]
    [Range(0, 1)]
    public float keepAttackChance = 0.35f; // 35% ตีต่อ

    [Header("Limits")]
    public int maxInnerEnemies = 4;
    public int maxOuterEnemies = 8;
    public int maxPlayerAttackers = 2;
    public int maxAllyAttackers = 1;

    public float innerRingRadius = 3f;
    public float outerRingRadius = 6f;

    public int innerRingSlots = 4;
    public int outerRingSlots = 8;

    public static EnemyCombatDirector Instance;

    public Transform player;

    [Header("Formation")]
    public float innerRadius = 2.5f;
    public float outerRadius = 4.5f;

    

    List<EnemyNormal> innerRing = new();
    List<EnemyNormal> outerRing = new();
    List<EnemyNormal> playerAttackers = new();
    
    Dictionary<Transform, EnemyNormal> allyFighters = new();
    void Awake()
    {
        Instance = this;
    }
    public bool HasSlot()
    {
        int total = innerRing.Count + outerRing.Count;
        int maxTotal = maxInnerEnemies + maxOuterEnemies;

        return total < maxTotal;
    }
    public void RegisterEnemy(EnemyNormal enemy)
    {
        if (innerRing.Contains(enemy) || outerRing.Contains(enemy))
            return;

        if (innerRing.Count < maxInnerEnemies)
        {
            innerRing.Add(enemy);
            enemy.SetState(EnemyState.InnerRing);
        }
        else if (outerRing.Count < maxOuterEnemies)
        {
            outerRing.Add(enemy);
            enemy.SetState(EnemyState.OuterRing);
        }
        else
        {
            // เต็ม → ไม่รับเพิ่ม
            return;
        }
    }

    public Vector3 GetSlotPosition(EnemyNormal enemy)
    {
        int index;
        int count;
        float radius;

        if (enemy.currentState == EnemyState.InnerRing)
        {
            index = innerRing.IndexOf(enemy);
            count = innerRingSlots;
            radius = innerRingRadius;
        }
        else
        {
            index = outerRing.IndexOf(enemy);
            count = outerRingSlots;
            radius = outerRingRadius;
        }

        if (index < 0) return enemy.transform.position;

        float angle = (360f / count) * index;

        Vector3 dir = new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0,
            Mathf.Cos(angle * Mathf.Deg2Rad)
        );

        Vector3 target = player.position + dir * radius;

        return target;
    }

    public float GetRingRadius(EnemyNormal enemy)
    {
        if (enemy.currentState == EnemyState.InnerRing)
            return innerRingRadius;

        if (enemy.currentState == EnemyState.OuterRing)
            return outerRingRadius;

        return innerRingRadius;
    }

    public bool RequestPlayerAttack(EnemyNormal enemy)
    {
        if (playerAttackers.Count >= maxPlayerAttackers)
            return false;

        if (playerAttackers.Contains(enemy))
            return false;

        playerAttackers.Add(enemy);
        enemy.SetState(EnemyState.AttackTurn);

        return true;
    }

    public void FinishPlayerAttack(EnemyNormal enemy)
    {
        if (!playerAttackers.Contains(enemy))
            return;

        if (Random.value < keepAttackChance)
            return;

        playerAttackers.Remove(enemy);

        if (innerRing.Contains(enemy))
            enemy.SetState(EnemyState.InnerRing);

        GiveAttackToAnother();
        FillInnerRing();
    }
    public bool RequestAllyFight(EnemyNormal enemy, Transform ally)
    {
        if (ally == null)
            return false;

        // ally นี้มี enemy สู้แล้ว
        if (allyFighters.ContainsKey(ally))
            return false;

        allyFighters[ally] = enemy;

        return true;
    }
    public void FinishAllyFight(EnemyNormal enemy, Transform ally)
    {
        if (ally == null)
            return;

        if (allyFighters.ContainsKey(ally) && allyFighters[ally] == enemy)
            allyFighters.Remove(ally);
    }

    void GiveAttackToAnother()
    {
        if (playerAttackers.Count >= maxPlayerAttackers)
            return;

        // หา enemy ใน inner ring ที่ยังไม่ได้ตี player
        var candidates = innerRing
            .Where(e =>
                !playerAttackers.Contains(e) &&
                e.currentState != EnemyState.FightingAlly)
            .ToList();

        if (candidates.Count == 0)
            return;

        EnemyNormal next =
            candidates[Random.Range(0, candidates.Count)];

        RequestPlayerAttack(next);
    }
    public void RemoveFromPlayerAttackers(EnemyNormal enemy)
    {
        playerAttackers.Remove(enemy);
    }

    public bool HasPlayerAttackSlot()
    {
        return playerAttackers.Count < maxPlayerAttackers;
    }
    public void UnregisterEnemy(EnemyNormal enemy)
    {
        innerRing.Remove(enemy);
        outerRing.Remove(enemy);
        playerAttackers.Remove(enemy);

        Transform removeKey = null;

        foreach (var pair in allyFighters)
        {
            if (pair.Value == enemy)
            {
                removeKey = pair.Key;
                break;
            }
        }

        if (removeKey != null)
            allyFighters.Remove(removeKey);

        FillInnerRing();
    }

    void FillInnerRing()
    {
        while (innerRing.Count < maxInnerEnemies && outerRing.Count > 0)
        {
            EnemyNormal e = outerRing[0];

            outerRing.RemoveAt(0);
            innerRing.Add(e);

            e.SetState(EnemyState.InnerRing);
        }
    }


}