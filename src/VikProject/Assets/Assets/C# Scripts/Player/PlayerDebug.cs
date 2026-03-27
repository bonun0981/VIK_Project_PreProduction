using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    [SerializeField] Health hp;
    [SerializeField] PlayerResource resource;
    private void Start()
    {
        hp = GetComponent<Health>();
        resource = GetComponent<PlayerResource>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            hp.HpDebug();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            resource.ResouceDebug();
        }
        
    }
}
