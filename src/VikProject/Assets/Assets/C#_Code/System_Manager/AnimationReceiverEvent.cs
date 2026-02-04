using System.Collections.Generic;
using UnityEngine;

public class AnimationReceiverEvent : MonoBehaviour
{
    [SerializeField] List<CombatAnimationEvent> animationEvents = new ();


    public void ReceiveEvent(string eventName)
    {
        CombatAnimationEvent matchingEvent = animationEvents.Find(se => se.eventName == eventName);
        matchingEvent?.OnAnimationEvent?.Invoke();
    }
     


}
