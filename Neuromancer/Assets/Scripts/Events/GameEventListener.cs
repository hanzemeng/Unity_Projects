using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameEvent gameEvent;
    [SerializeField] UnityEvent<GameObject> unityEvent;
    public UnityEvent<GameObject> UnityEvent { get {return unityEvent; }}
    [SerializeField] int triggerid;

    private void Awake()
    {
        gameEvent.Register(this);
    }

    private void OnDestroy()
    {
        gameEvent.Deregister(this);
    }


    public void RaiseEvent(int target, GameObject source = null)
    {
        if (target == triggerid)
            unityEvent?.Invoke(source);
    }

}