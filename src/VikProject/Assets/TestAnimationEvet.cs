using UnityEngine;

public class TestAnimationEvet : MonoBehaviour
{

    [SerializeField] string text; 
   public void TestEvent()
   {
       Debug.Log(text);
    }
}
