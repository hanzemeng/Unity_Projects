using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IN_APP_PURCHASE_INFORMATION", menuName = "ScriptableObjects/In App Purchase Information", order = 3)]
public class InAppPurchaseInformation : ScriptableObject
{
    public int hasRemoveAds;

    private InAppPurchaseInformation()
    {
        hasRemoveAds = 0;
    }

    public InAppPurchaseInformation DeepCopy()
    {
        InAppPurchaseInformation newInAppPurchaseInformation = CreateInstance<InAppPurchaseInformation>();

        newInAppPurchaseInformation.hasRemoveAds = hasRemoveAds;

        return newInAppPurchaseInformation;
    }

    public string Base16Encode()
    {
        string res = "";
        res += Convert.ToString(hasRemoveAds, 16); // 4 bits for hasRemoveAds
        return res;
    }
    public static InAppPurchaseInformation Base16Decode(string base16String)
    {
        if("" == base16String)
        {
            return null;
        }

        int startIndex = 0;
        InAppPurchaseInformation inAppPurchaseInformation = CreateInstance<InAppPurchaseInformation>();

        inAppPurchaseInformation.hasRemoveAds = Convert.ToInt32(base16String.Substring(startIndex,1), 16);

        return inAppPurchaseInformation;
    }
}
