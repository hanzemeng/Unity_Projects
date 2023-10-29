using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager adsManager;

    private const string adUnitID = "Interstitial_iOS"; // copied from https://dashboard.unity3d.com/gaming/organizations/20067024036301/projects/168b4548-c794-469f-823d-b217417c353a/monetization/ad-units?providerType=existingexclusive

    private bool adIsLoaded;

    public UnityEvent onAdsStart;
    public UnityEvent onAdsEnd;

    private void Awake()
    {
        if(null != adsManager)
        {
            Destroy(gameObject);
            return;
        }
        adsManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadAd()
    {
        adIsLoaded = true;
        Advertisement.Load(adUnitID, this);
    }
    public bool AdIsReady()
    {
        return adIsLoaded;
    }
    public void ShowAd()
    {
        adIsLoaded = false;
        Advertisement.Show(adUnitID, this);
    }
 
    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }
 
    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }
 
    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }
 
    public void OnUnityAdsShowStart(string _adUnitId)
    {
        onAdsStart.Invoke();
    }
    public void OnUnityAdsShowClick(string _adUnitId) { }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        onAdsEnd.Invoke();
    }
}
