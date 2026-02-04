using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CombatAnimationEvent
{

    public string eventName;
    public UnityEvent OnAnimationEvent;
}
