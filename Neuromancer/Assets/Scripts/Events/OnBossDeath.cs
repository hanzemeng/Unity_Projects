using UnityEngine;

public class OnBossDeath : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] GameEvent onAllTriggered;
    private bool notInvoked = true;

    public void OnDestroy()
    {
        if (notInvoked)
        {
            onAllTriggered?.Invoke(id);
        }
    }


}
