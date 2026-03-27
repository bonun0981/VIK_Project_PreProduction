using UnityEngine;
using System.Collections;

public class PlayerResource : MonoBehaviour
{
    public static PlayerResource Instance;

    private void Awake()
    {
        Instance = this;
    }
    public float maxResource = 100f;
    public float currentResource;

    private void Start()
    {
        currentResource = 0;
        StartCoroutine(ResourceRegenRoutine());
    }

    IEnumerator ResourceRegenRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (!IsFull())
            {
                AddResource(1f);
            }
        }
    }
    public void ResouceDebug()
    {
        currentResource = maxResource;
    }
    public void AddResource(float amount)
    {
        currentResource += amount;
        currentResource = Mathf.Clamp(currentResource, 0, maxResource);
    }
    
    public bool IsFull()
    {
        return currentResource >= maxResource;
    }

    public void ConsumeAll()
    {
        currentResource = 0;
    }

    public float NormalizedValue()
    {
        return currentResource / maxResource;
    }
}