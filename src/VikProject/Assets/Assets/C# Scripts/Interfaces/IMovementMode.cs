using Unity.Hierarchy;
using UnityEngine;

public interface IMovementMode
{
    float Acceleration { get; }
    float Deceleration { get; }
    float MaxSpeed { get; }
}


class Walk : IMovementMode
{
    public float Acceleration => 35f;
    public float Deceleration => 40f;
    public float MaxSpeed => 7f;
}
class Run : IMovementMode
{
    public float Acceleration => 55f;
    public float Deceleration => 60f;
    public float MaxSpeed => 12f;
}
