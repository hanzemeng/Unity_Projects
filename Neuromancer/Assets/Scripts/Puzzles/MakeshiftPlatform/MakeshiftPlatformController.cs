using UnityEngine;
using UnityEngine.Events;

public class MakeshiftPlatformController : MonoBehaviour
{
    [SerializeField] private GameObject platformSuccessFX;
    [SerializeField] private GameObject mainPlatform;
    [SerializeField] private AudioManager.SoundResource plaftormSuccessSound = AudioManager.SoundResource.MINI_PUZZLE_SOLVED;
    public GameObject MainPlatform { get {return mainPlatform;} }
    private MakeshiftPlatformDetector[] allDetectors;
    [HideInInspector] public UnityEvent<GameObject> activatePlatform;

    private void Awake()
    {
        allDetectors = GetComponentsInChildren<MakeshiftPlatformDetector>();

        if(allDetectors.Length > 0)
        {
            foreach(MakeshiftPlatformDetector detector in allDetectors)
            {
                detector.MakeshiftPlatform = mainPlatform;
            }
        }       
    }

    public void ActivatePlatform()
    {
        mainPlatform.SetActive(true);
        AudioManager.instance.PlayBackgroundSFX(plaftormSuccessSound);
        activatePlatform?.Invoke(this.gameObject);

        if(NavMeshSurfaceGenerator.current != null)
        {
            NavMeshSurfaceGenerator.current.UpdateNavMesh(0.05f);
        }
        
        if(allDetectors.Length > 0)
        {
            foreach(MakeshiftPlatformDetector detector in allDetectors)
            {
                _ = platformSuccessFX != null ? Instantiate(platformSuccessFX, detector.transform.position, Quaternion.identity) : null;
            }
        }
        
    }

   
}
