using UnityEngine;

public class TwoWayLeverController : MonoBehaviour
{
    [SerializeField] private Animator myLever;
    public void LeverStateOnetoTwo()
    {
        myLever.Play("TwoWayLeverStateOne", 0, 0.0f);
    }

    public void LeverStateTwotoOne()
    {
        myLever.Play("TwoWayLeverStateTwo", 0, 0.0f);
    }
}
