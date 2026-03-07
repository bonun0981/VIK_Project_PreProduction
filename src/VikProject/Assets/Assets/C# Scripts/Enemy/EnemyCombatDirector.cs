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
    public int maxAttackers = 2;

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
    List<EnemyNormal> attackers = new();

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

    public bool RequestAttackTurn(EnemyNormal enemy)
    {
        if (attackers.Count >= maxAttackers)
            return false;

        if (attackers.Contains(enemy))
            return false;

        attackers.Add(enemy);
        enemy.SetState(EnemyState.AttackTurn);

        return true;
    }

    public void FinishAttack(EnemyNormal enemy)
    {
        if (!attackers.Contains(enemy))
            return;

        // สุ่มว่าจะตีต่อไหม
        if (Random.value < keepAttackChance)
        {
            // ตีต่อ
            return;
        }

        // ไม่ตีต่อ → คืน state
        attackers.Remove(enemy);

        if (innerRing.Contains(enemy))
            enemy.SetState(EnemyState.InnerRing);

        // ให้ตัวอื่นมีโอกาสได้เทิร์น
        GiveAttackToAnother();
    }

    void GiveAttackToAnother()
    {
        if (attackers.Count >= maxAttackers)
            return;

        // หา enemy ที่อยู่ inner ring และยังไม่โจมตี
        var candidates = innerRing
            .Where(e => !attackers.Contains(e))
            .ToList();

        if (candidates.Count == 0)
            return;

        EnemyNormal next = candidates[Random.Range(0, candidates.Count)];

        RequestAttackTurn(next);
    }

   
}