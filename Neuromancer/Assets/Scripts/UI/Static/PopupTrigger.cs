using UnityEngine;
using Neuromancer;

public class PopupTrigger : MonoBehaviour {
    [TextArea(3,6)] [SerializeField] private string text;
    private bool triggered;

    private void Awake() {
        triggered = false;
    } 
    
    private void OnTriggerEnter(Collider other) {
        if(!triggered && Unit.IsHero(other.transform)) {
            PopupHandler.current.Show(text);
            triggered = true;
            gameObject.GetComponent<ObjectPermanent>().SetDestroy();
            Destroy(gameObject);
        }
    }

}
