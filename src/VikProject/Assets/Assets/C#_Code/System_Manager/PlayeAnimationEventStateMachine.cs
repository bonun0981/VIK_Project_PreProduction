using UnityEngine;

public class PlayeAnimationEventStateMachine : StateMachineBehaviour
{
    public string eventName;
    [Range(0f, 1f)] public float triggerTime;

    bool hasTriggered;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasTriggered = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = stateInfo.normalizedTime % 1;

        if (!hasTriggered && currentTime >= triggerTime)
        {
            NotifyReceiver(animator);
            hasTriggered = true;
        }
    }

    void NotifyReceiver(Animator animator)
    {
        AnimationReceiverEvent receiver = animator.GetComponent<AnimationReceiverEvent>();
        if (receiver != null)
        {
            receiver.ReceiveEvent(eventName);
        }
    }

}
