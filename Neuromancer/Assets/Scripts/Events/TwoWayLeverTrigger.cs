using System.Collections;
using UnityEngine;

public class TwoWayLeverTrigger : MonoBehaviour
{

    [SerializeField] private int id;
    [SerializeField] private int id2;
    [SerializeField] private GameEvent onTriggeredState1;
    protected bool notInvokedState1 = true;
    protected bool notInvokedState2 = true;
    [SerializeField] private float cooldown = 5f;

    private bool isTriggered = false;

    public void Trigger(GameObject source = null)
    {
         if (!isTriggered && (source.CompareTag(Neuromancer.Unit.HERO_TAG) || Neuromancer.Unit.IsAlly(source.transform)))
         {
             if (notInvokedState1)
             {
                onTriggeredState1?.Invoke(id);
                notInvokedState1 = false;
                notInvokedState2 = true;
                isTriggered = true;
                StartCoroutine(ResetTriggerAfterDelay());
              
             }
             else if (notInvokedState2)
             {
                onTriggeredState1?.Invoke(id2);
                notInvokedState2 = false;
                notInvokedState1 = true;
                isTriggered = true;
                StartCoroutine(ResetTriggerAfterDelay());
             }
         }
    }

    private IEnumerator ResetTriggerAfterDelay()
    {
        yield return new WaitForSeconds(cooldown);

        isTriggered = false;
    }


}
