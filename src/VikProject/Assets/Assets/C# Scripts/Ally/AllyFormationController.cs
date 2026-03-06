using System.Collections.Generic;
using UnityEngine;

public class AllyFormationController : MonoBehaviour
{
    public Transform player;
    public float radius = 3f;

    List<AllyMovement> allies = new List<AllyMovement>();

    public void Register(AllyMovement ally)
    {
        if (!allies.Contains(ally))
            allies.Add(ally);
    }

    public void Unregister(AllyMovement ally)
    {
        allies.Remove(ally);
    }

    public Vector3 GetSlotPosition(AllyMovement ally)
    {
        int index = allies.IndexOf(ally);
        int total = allies.Count;

        if (total == 0) return player.position;

        float angle = index * Mathf.PI * 2f / total;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle),
            0,
            Mathf.Sin(angle)
        ) * radius;

        return player.position + offset;
    }
}