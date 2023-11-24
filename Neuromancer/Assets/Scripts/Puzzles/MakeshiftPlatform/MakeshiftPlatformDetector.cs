using UnityEngine;


public class MakeshiftPlatformDetector : MonoBehaviour
{
    [SerializeField] private bool isEdgeDetector;
    private GameObject makeshiftPlatform; 
    public GameObject MakeshiftPlatform { get { return makeshiftPlatform;} set {makeshiftPlatform = value;} }

    private MakeshiftPlatformController controller;
    
    private void Awake()
    {
        controller = GetComponentInParent<MakeshiftPlatformController>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.isTrigger) { return; }
        else if(other.CompareTag("Interactable"))
        {
            PushableObject pushable = other.gameObject.GetComponent<PushableObject>();
            if(pushable != null)
            {
                switch (isEdgeDetector)
                {
                    case true:
                        pushable.InitiateFall();
                        break;
                    case false:
                        pushable.DestroyAndReplacePushable(controller);
                        break;
                }
                        
            }
        }
    }

}
