using System.Collections.Generic;
using UnityEngine;

public class CollisionListener : MonoBehaviour
{
    private int consent;
    private int counter = 0;
    [SerializeField] int id;
    [SerializeField] GameEvent onAllTriggered;
    private bool notInvoked = true;
    Collider[] allColliders;
    private Collider[] colliders;
    private void Awake()
    {
        allColliders = GetComponentsInChildren<Collider>();
        List<Collider> allTriggerColliders = new List<Collider>();

        consent = 0;
        // Acquire all TRIGGER colliders only:
        foreach(Collider col in allColliders)
        {
            if(col.isTrigger) 
            {
                allTriggerColliders.Add(col);
                consent += 1;
            }
        }

        colliders = allTriggerColliders.ToArray();
        // consent = colliders.Length;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Neuromancer.Unit.HERO_TAG || Neuromancer.Unit.IsAlly(other.transform))
        {
            counter++;
        }
        if (counter == consent && notInvoked)
        {
            onAllTriggered?.Invoke(id);
            notInvoked = false;
            //Turn off children colliders
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (notInvoked && other.gameObject.tag == Neuromancer.Unit.HERO_TAG || Neuromancer.Unit.IsAlly(other.transform))
        {
            counter--;
        }
        
    }


}
