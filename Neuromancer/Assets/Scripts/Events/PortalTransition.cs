using UnityEngine;

public class PortalTransition : MonoBehaviour
{
    [SerializeField] private GameObject plainGate;
    [SerializeField] private GameObject transitionGate;

    public void EnableTransition() {
        transitionGate.SetActive(true);
        plainGate.GetComponent<ObjectPermanent>().SetDestroy();
        Destroy(plainGate);
    }
}
