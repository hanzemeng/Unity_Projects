using UnityEngine;

public class BasicTrigger : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] GameEvent onAllTriggered;
    protected bool notInvoked = true;

    protected virtual void ExtraThing() { }


    public void Trigger(GameObject source = null)
    {
        
        if (source.CompareTag(Neuromancer.Unit.HERO_TAG) || Neuromancer.Unit.IsAlly(source.transform))
        {
            //add to collider list
            if (notInvoked)
            {
                onAllTriggered?.Invoke(id);
                ExtraThing();
                notInvoked = false;
            }
        }
    }

}