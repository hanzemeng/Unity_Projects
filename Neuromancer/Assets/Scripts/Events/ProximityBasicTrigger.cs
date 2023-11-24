using UnityEngine;

public class ProximityBasicTrigger : BasicTrigger
{
    private void OnTriggerEnter(Collider other)
    {
        Trigger(other.gameObject);
    }
}
