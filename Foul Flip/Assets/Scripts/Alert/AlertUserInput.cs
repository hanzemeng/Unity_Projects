using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class AlertUserInput : MonoBehaviour
{
    public static AlertUserInput alertUserInput;

    public MouseInput removeAds;
    public MouseInput restorePurchase;
    public MouseInput returnGame;

    private void Awake()
    {
        if(null != alertUserInput)
        {
            Destroy(gameObject);
            return;
        }
        alertUserInput = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ListenRemoveAdsInput()
    {
        removeAds.onClickEvent.AddListener(() => AlertControl.alertControl.OnRemoveAdsClick());
        removeAds.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenRemoveAdsInput()
    {
        removeAds.onClickEvent.RemoveAllListeners();
        removeAds.transform.GetComponent<Collider>().enabled = false;
    }

    public void ListenRestorePurchaseInput()
    {
        restorePurchase.onClickEvent.AddListener(() => AlertControl.alertControl.OnRestorePurchaseClick());
        restorePurchase.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenRestorePurchaseInput()
    {
        restorePurchase.onClickEvent.RemoveAllListeners();
        restorePurchase.transform.GetComponent<Collider>().enabled = false;
    }

    public void ListenReturnGameInput()
    {
        returnGame.onClickEvent.AddListener(() => AlertControl.alertControl.OnReturnGameClick());
        returnGame.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenReturnGameInput()
    {
        returnGame.onClickEvent.RemoveAllListeners();
        returnGame.transform.GetComponent<Collider>().enabled = false;
    }
}
