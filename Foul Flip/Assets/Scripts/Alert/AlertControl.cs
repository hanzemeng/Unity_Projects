using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertControl : MonoBehaviour
{
    public static AlertControl alertControl;

    private bool isBusy;
    private bool isOpen;

    private void Awake()
    {
        if(null != alertControl)
        {
            Destroy(gameObject);
            return;
        }
        alertControl = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if(InAppPurchaseManager.inAppPurchaseManager.HasRemoveAds())
        {
            AlertUI.alertUI.SoldRemoveAdsText();
        }
        StopListenInputs();
        InAppPurchaseManager.inAppPurchaseManager.AssignPurchaseRemoveAdsCallbacks(OnRemoveAdsPurchaseSuccess, OnRemoveAdsPurchaseFail);
        InAppPurchaseManager.inAppPurchaseManager.AssignRestorePurchaseCallbacks(OnRestorePurchaseSuccess, OnRestorePurchaseFail);
    }

    private void StartListenInputs()
    {
        AlertUserInput.alertUserInput.ListenReturnGameInput();
        if(!InAppPurchaseManager.inAppPurchaseManager.HasRemoveAds())
        {
            AlertUserInput.alertUserInput.ListenRemoveAdsInput();
        }
        AlertUserInput.alertUserInput.ListenRestorePurchaseInput();
    }
    private void StopListenInputs()
    {
        AlertUserInput.alertUserInput.StopListenReturnGameInput();
        AlertUserInput.alertUserInput.StopListenRemoveAdsInput();
        AlertUserInput.alertUserInput.StopListenRestorePurchaseInput();
    }

    public void OpenAlert()
    {
        StartCoroutine(OpenAlertCoroutine());
    }
    private IEnumerator OpenAlertCoroutine()
    {
        isOpen = true;
        isBusy = true;
        yield return new WaitForSeconds(AlertUI.alertUI.FadeInCanvasGroup());
        StartListenInputs();
        isBusy = false;
    }

    public void OnRemoveAdsClick()
    {
        if(isBusy)
        {
            return;
        }

        AudioManager.audioManager.PlayIconClick();
        AlertUI.alertUI.ShowLoadingText();
        isBusy = true;
        InAppPurchaseManager.inAppPurchaseManager.PurchaseRemoveAds();
    }
    public void OnRemoveAdsPurchaseSuccess()
    {
        AlertUI.alertUI.HideLoadingText();
        if(InAppPurchaseManager.inAppPurchaseManager.HasRemoveAds())
        {
            AlertUI.alertUI.SoldRemoveAdsText();
        }
        isBusy = false;
        Debug.Log("REMOVED ADS");
    }
    public void OnRemoveAdsPurchaseFail()
    {
        AlertUI.alertUI.HideLoadingText();
        isBusy = false;
        Debug.Log("FAIL TO REMOVE ADS");
    }

    public void OnRestorePurchaseClick()
    {
        if(isBusy)
        {
            return;
        }
        AudioManager.audioManager.PlayIconClick();
        AlertUI.alertUI.ShowLoadingText();
        isBusy = true;
        InAppPurchaseManager.inAppPurchaseManager.RestorePurchase();
    }
    public void OnRestorePurchaseSuccess()
    {
        AlertUI.alertUI.HideLoadingText();
        isBusy = false;
        Debug.Log("RESTORE PURCHASE");
    }
    public void OnRestorePurchaseFail()
    {
        AlertUI.alertUI.HideLoadingText();
        isBusy = false;
        Debug.Log("FAIL TO RESTORE PURCHASE");
    }

    public void OnReturnGameClick()
    {
        if(isBusy)
        {
            return;
        }

        isOpen = false;
        AudioManager.audioManager.PlayIconClick();
        AlertUserInput.alertUserInput.StopListenReturnGameInput();
        StopListenInputs();
        AlertUI.alertUI.FadeOutCanvasGroup();
        GameControl.gameControl.OnAlertClose();
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
