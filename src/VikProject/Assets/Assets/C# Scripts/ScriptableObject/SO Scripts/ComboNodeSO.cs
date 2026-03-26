using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboNodeSO", menuName = "Scriptable Objects/ComboNodeSO")]
public class ComboNodeSO : ScriptableObject
{
    [Header("Attack Sequence")]
    public AttackDataSO attack;

    [Header("Branching")]
    public List<ComboTransition> transitions;

    public ComboNodeSO GetNextNode(AttackInputType input)
    {
        foreach (var t in transitions)
        {
            if (t.input == input)
                return t.nextNode;
        }

        return null;
    }
}
