using UnityEngine;

public class PlayeAnimationEventStateMachine : StateMachineBehaviour
{
    [Header("Start Event")]
    public string startEventName;
    [Range(0f, 1f)] public float triggerTime;

    [Header("End Event (optional)")]
    public string endEventName;
    [Range(0f, 1f)] public float endTime; 

    private bool startTriggered;
    private bool endTriggered;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        startTriggered = false;
        endTriggered = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = stateInfo.normalizedTime % 1f;

       
        if (!startTriggered && currentTime >= triggerTime)
        {
            NotifyReceiver(animator, startEventName);
            startTriggered = true;
        }

        
        if (endTime > 0f && startTriggered && !endTriggered && currentTime >= endTime)
        {
            NotifyReceiver(animator, endEventName);
            endTriggered = true;
        }
    }
     
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
        if (endTime > 0f && startTriggered && !endTriggered)
        {
            NotifyReceiver(animator, endEventName);
        }
    }

    private void NotifyReceiver(Animator animator, string eventName)
    {
        if (string.IsNullOrEmpty(eventName)) return;

        AnimationReceiverEvent receiver =
            animator.GetComponent<AnimationReceiverEvent>();

        if (receiver != null)
        {
            receiver.ReceiveEvent(eventName);
        }
    }

}
