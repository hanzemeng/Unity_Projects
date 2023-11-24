using System.Collections;
using EmeraldAI;
using EmeraldAI.Utility;
using Neuromancer;
using UnityEngine;

public class ProjectileInteract : MonoBehaviour
{

    [SerializeField] private LayerMask interactableLayer;
    
    private EmeraldAISystem emeraldComponent;
    private EmeraldAIEventsManager eventsManager;
    private GameObject parentObject;
    
    private bool fieldsSet = false;

    private Interactable interactable;
    private NPCUnit npcUnit;
    private AI ai;
    
    private void Update()
    {

        if (!fieldsSet)
        {

            emeraldComponent = GetComponent<EmeraldAIProjectile>().EmeraldComponent;
            GetComponent<EmeraldAIProjectile>().isInteract = true;
            eventsManager = emeraldComponent.EmeraldEventsManagerComponent;
            parentObject = emeraldComponent.gameObject;
            npcUnit = parentObject.GetComponent<NPCUnit>();
            ai = parentObject.GetComponent<AI>();
            fieldsSet = true;

        }

    }
    
    private void OnTriggerEnter(Collider other)
    {

        if (!fieldsSet) return;

        if ((interactableLayer & (1 << other.gameObject.layer)) != 0)
        {
            interactable = other.GetComponent<Interactable>();
            
            if(((IList)interactable.possibleEnemies).Contains(NPCUnitType.DEFAULT) || ((IList)interactable
                   .possibleEnemies).Contains(npcUnit.unitPrefab.npcUnitType))
            {
                
                interactable.Interact?.Invoke(this.gameObject);
                ai.TriggerInteraction();
                
            }
            
            Destroy(gameObject);
            
        }

    }
}
