using UnityEngine;

public class ChestTrigger : BasicTrigger
{
    public void OnTriggerEnter(Collider other)
    {
        Trigger(other.gameObject);
    }
}
