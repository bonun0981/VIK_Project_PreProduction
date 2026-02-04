using UnityEngine;

public class PlayeAnimationEventStateMachine : StateMachineBehaviour
{
    [Header("Start Event")]
    public string startEventName;

    [Header("Start Update Event")]
    public string startUpdateEventName;
    [Range(0f, 1f)] public float triggerTime;

    [Header("End Update Event")]
    public string endUpdateEventName;
    [Range(0f, 1f)] public float endTime;

    [Header("Exit Event")]
    public string exitEventName;
   

    private bool startTriggered;
    private bool endTriggered;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        startTriggered = false;
        endTriggered = false;
        if (!string.IsNullOrEmpty(startEventName))
        {
            NotifyReceiver(animator, startEventName);
        }
            
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = stateInfo.normalizedTime % 1f;

       
        if (!startTriggered && currentTime >= triggerTime)
        {
            NotifyReceiver(animator, startUpdateEventName);
            startTriggered = true;
        }

        
        if (endTime > 0f && startTriggered && !endTriggered && currentTime >= endTime)
        {
            NotifyReceiver(animator, endUpdateEventName);
            endTriggered = true;
        }
    }
     
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
        if (endTime > 0f && startTriggered && !endTriggered)
        {
            if(!string.IsNullOrEmpty(exitEventName))
            {
                NotifyReceiver(animator, exitEventName);
            }
            
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
