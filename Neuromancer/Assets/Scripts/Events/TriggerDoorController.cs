using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField] private Animator door = null;

    [Header("Object Permanence on Doors")]
    [SerializeField] private bool isUsingObjectPermanence = false;
    [SerializeField] private ObjectPermanent[] closedDoors = null;
    [Tooltip("Note, if you do enable object permanence, make sure there's an alternate version of the doors that are open. Also make sure the arrays are of equal length")]
    [SerializeField] private ObjectPermanent[] openedDoors = null;

    private void Start()
    {
        // Added contigency since ObjectPermanence is sometimes weirdly buggy
        if(isUsingObjectPermanence)
        {
            for(int i = 0; i < closedDoors.Length; i++)
            {
                ObjectData closedDoorData = SaveLoadManager.saveLoadManager.FindObjectEntry(closedDoors[i].id);
                //ObjectData openDoorData = SaveLoadManager.saveLoadManager.FindObjectEntry(openedDoors[i].id);
                
                // Checks if the opened door's state is equal to enabled AND that they aren't active in hierarchy, along with their closed door counterpart
                if(closedDoorData != null && closedDoorData.state > 0)
                {
                    openedDoors[i].gameObject.SetActive(true);
                }
            }
        }
    }
    public void openDoor()
    {
        door.Play("BigDoorOpen", 0, 0.0f);
        // NavMeshSurfaceGenerator.current.UpdateNavMesh(0.05f);

        if(isUsingObjectPermanence)
        {
            // Apply object permanence on doors
            for(int i = 0; i < closedDoors.Length; i++)
            {
                closedDoors[i].SetDisable();
            }
        }
    }

    public void closeDoor()
    {
        door.Play("BigDoorClose", 0, 0.0f);
    }

    public void PlayDoorSound()
    {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.DOOR_OPEN);
    }

}
