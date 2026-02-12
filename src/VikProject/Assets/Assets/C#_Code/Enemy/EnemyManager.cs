using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public int maxActive = 3;
    public int maxPassive = 5;

    HashSet<EnemyBase> activeSet = new();
    HashSet<EnemyBase> passiveSet = new();

    float passiveReopenTime = 0f;
    [SerializeField] float passiveCooldown = 0.6f;

    EnemyActiveRadian currentEngagement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this; 
    }

    // ---------- ENGAGEMENT ----------
    public bool CanEngage(EnemyActiveRadian requester)
    {
        if (currentEngagement == null)
        {
            currentEngagement = requester;
            return true;
        }
        return currentEngagement == requester;
    }

    public void ReleaseEngagement(EnemyActiveRadian owner)
    {
        if (currentEngagement == owner)
            currentEngagement = null;
    }

    // ---------- PASSIVE ----------
    public bool RequestPassive(EnemyBase e)
    {
        if (!e.aiActive) return false;
        if (currentEngagement == null) return false;
        if (passiveSet.Count >= maxPassive) return false;

        passiveSet.Add(e);
        return true;
    }

    // ---------- ACTIVE ----------
    public bool RequestActive(EnemyBase e)
    {
        if (activeSet.Count >= maxActive) return false;

        passiveSet.Remove(e);
        activeSet.Add(e);
        return true;
    }

    // ---------- ATTACK ROTATION ----------
    public void OnEnemyFinishedAttack(EnemyBase finishedEnemy)
    {
        if (activeSet.Count >= maxActive)
            return;

        EnemyBase next = GetRandomPassiveExcept(finishedEnemy);
        if (next != null)
            PromoteToActive(next);
    }

    EnemyBase GetRandomPassiveExcept(EnemyBase except)
    {
        List<EnemyBase> candidates = new();

        foreach (var e in passiveSet)
        {
            if (e != except && e.IsReadyToAttack())
                candidates.Add(e);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    void PromoteToActive(EnemyBase e)
    {
        passiveSet.Remove(e);
        activeSet.Add(e);

        e.EnterActiveState(); // 👈 cleaner than SetState
    }

    // ---------- RELEASE ----------
    public void ReleaseAll(EnemyBase e)
    {
        if (passiveSet.Remove(e))
            passiveReopenTime = Time.time + passiveCooldown;

        activeSet.Remove(e);
    }

    public bool IsActive(EnemyBase e) => activeSet.Contains(e);
    public bool IsPassive(EnemyBase e) => passiveSet.Contains(e);
}
