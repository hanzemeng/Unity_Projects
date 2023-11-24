using UnityEngine;

public class LeverTrigger : BasicTrigger
{
    [SerializeField] private Animator myLever;

   protected override void ExtraThing()
    {
        myLever.Play("LeverActive", 0, 0.0f);
    }
}
