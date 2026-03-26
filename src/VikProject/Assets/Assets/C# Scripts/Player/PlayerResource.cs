using UnityEngine;

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