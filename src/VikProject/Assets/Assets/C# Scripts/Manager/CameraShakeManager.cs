using SmoothShakeFree;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    [SerializeField]private SmoothShake shake;
    GameObject shakeManager;
   
    public void TakeDamageShake()
    {
        if (shake != null)
        {
            shake.StartShake();
        }
    }
}
