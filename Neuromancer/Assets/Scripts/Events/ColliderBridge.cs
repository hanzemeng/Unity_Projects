using System.Collections.Generic;
using UnityEngine;

public class ColliderBridge : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CollisionListener listener;
    private bool triggered = false;
    private List<GameObject> objectOnTrigger = new List<GameObject>();

    [SerializeField]private GameEvent onSinglePlateEnter;
    [SerializeField]private GameEvent onSinglePlateLeave;
    [SerializeField]private int id;

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Neuromancer.Unit.HERO_TAG || Neuromancer.Unit.IsAlly(other.transform))
        {
            objectOnTrigger.Add(other.gameObject);
            //add to collider list
            if (!triggered) {
                triggered = true;
                if(listener != null)
                    listener.OnTriggerEnter(other);
                onSinglePlateEnter?.Invoke(id);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Neuromancer.Unit.HERO_TAG || Neuromancer.Unit.IsAlly(other.transform))
        {
            //remove object in collider list
            //check if list is zero
            objectOnTrigger.Remove(other.gameObject);
            if (triggered) {
                if (objectOnTrigger.Count == 0)
                {
                    triggered = false;
                    if (listener != null)
                        listener.OnTriggerExit(other);
                    onSinglePlateLeave?.Invoke(id);
                }
            }
        }
    }


}
