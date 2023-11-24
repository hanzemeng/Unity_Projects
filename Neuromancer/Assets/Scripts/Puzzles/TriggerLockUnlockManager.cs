using UnityEngine;

public class TriggerLockUnlockManager : MonoBehaviour
{
    [SerializeField] private Animator lockAnimator =  null;
    [Header("Particle Effects for Unlock")]
    [SerializeField] private GameObject[] unlockFXs;
    [SerializeField] private Transform[] unlockFx_Targets;
    [SerializeField] private GameObject[] allObjects;

    [Header("Door Opening Event")]
    [SerializeField] private GameEvent OnDropOffPointSatisfied;
    [SerializeField] private int eventID = 0;

    private int currentIndex = 0;
    private ObjectPermanent lockPermanence;

    private void Awake()
    {
        lockPermanence = GetComponent<ObjectPermanent>();
    }

    public void StartUnlockSequence()
    {
        lockAnimator.Play("Key_Lock_Unlocked", 0, 0.0f);
    }

    public void TriggerFXExplosion()
    {
        // Instantiates the respective particle FX explosion assigned to the object
        Instantiate(unlockFXs[currentIndex], unlockFx_Targets[currentIndex].position, Quaternion.identity);
        // Will set the objects to disabled
        allObjects[currentIndex].gameObject.SetActive(false);
        currentIndex += 1;
        
        if(currentIndex == 2)
        {
            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.KEYLOCK_CHAIN_BREAK);
        }
        else
        {
            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.KEYLOCK_KEY_BREAK);
        }

        // only initiate door opening event if everything is destroyed.
        if(currentIndex == allObjects.Length)
        {
            
            OnDropOffPointSatisfied?.Invoke(eventID);
            lockPermanence.SetDestroy();
            Destroy(gameObject);
            
        }
    }


}
