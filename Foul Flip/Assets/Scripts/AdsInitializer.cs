using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    public static AdsInitializer adsInitializer;

    private const string iOSGameID = "5431321";
    public bool isTestMode;

    private void Awake()
    {
        if(null != adsInitializer)
        {
            Destroy(gameObject);
            return;
        }
        adsInitializer = this;
        DontDestroyOnLoad(gameObject);

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(iOSGameID, isTestMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
