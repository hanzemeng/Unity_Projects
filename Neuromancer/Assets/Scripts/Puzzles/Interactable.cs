using System.Linq;
using Neuromancer;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    public Neuromancer.NPCUnitType[] possibleEnemies;


    public InteractableGroup group;

    public static string INTERACT_TAG = "Interactable";
    public static string INTERACT_DROP_ZONE_LAYER_NAME = "InteractableDropZone";
    public static string INTERACT_LAYER_NAME = "Interactable";

    public UnityEvent<GameObject> Interact;

    // boolean to be used for any interactables that are designed to be interacted repeatedly identically to units attacking enemies
    public bool repeatInteraction = false;

    [Header("Sound Effects")]
    [SerializeField] private AudioManager.SoundResource interactSFX = AudioManager.SoundResource.INTERACTABLE_SUCCESS;
    [SerializeField] private AudioManager.SoundResource denySFX = AudioManager.SoundResource.INTERACTABLE_DENY;

    public void Awake()
    {
        if (group != null)
        {
            possibleEnemies = group.members;
        }
    }

    private void Start()
    {
        Interact.AddListener(OnInteract);
    }
    
    //Interact event plays corresponding sound based on unit's group type
    private void OnInteract(GameObject obj)
    {
        NPCUnit unit = obj.GetComponent<NPCUnit>();
        if(unit != null)
        {
            if(possibleEnemies.Contains(unit.unitPrefab.npcUnitType) || possibleEnemies.Contains(NPCUnitType.DEFAULT))
            {
                AudioManager.instance.PlayBackgroundSFX(interactSFX);
            }
            else
            {
                AudioManager.instance.PlayBackgroundSFX(denySFX);
            }
        }
        
        else
        {
            ProjectileInteract projectile = obj.GetComponent<ProjectileInteract>();
            if(projectile != null)
            {
                AudioManager.instance.PlayBackgroundSFX(interactSFX);
            }
        }

    }

    private void OnDestroy()
    {
        Interact.RemoveListener(OnInteract);
    }
}
