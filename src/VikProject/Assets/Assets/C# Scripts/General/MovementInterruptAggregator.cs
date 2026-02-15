using System.Linq;
using UnityEngine;

public class MovementInterruptAggregator: IMovementInterrupt
{
    private readonly IMovementInterrupt[] interrupts;

    public MovementInterruptAggregator(IMovementInterrupt[] interrupts)
    {
        this.interrupts = interrupts;
    }

    public bool IsMovementInterrupt =>
        interrupts.Any(i => i.IsMovementInterrupt);
}
