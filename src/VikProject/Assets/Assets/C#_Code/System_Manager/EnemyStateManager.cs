using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public static EnemyStateManager Instance;
    [SerializeField]private int maxActiveEnemies = 3;
    [SerializeField]private int maxPassiveEnemies = 5;
    public List<EnemyMotherClass> activeEnemiesLists = new();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void MyMethod()
    {

    }

}
