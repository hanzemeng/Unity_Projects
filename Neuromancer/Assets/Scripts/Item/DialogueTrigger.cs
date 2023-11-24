using UnityEngine;
using Neuromancer;

public class DialogueTrigger : MonoBehaviour {
    [SerializeField] private string dialogueName;
    private bool triggered;

    private void Awake() {
        triggered = false;
    } 
    
    private void OnTriggerEnter(Collider other) {
        if(!triggered && Unit.IsHero(other.transform)) {
            DialogueManager.dialogueManager.StartDialogue(dialogueName);;
            triggered = true;
            gameObject.GetComponent<ObjectPermanent>().SetDestroy();
            Destroy(gameObject);
        }
    }

}
