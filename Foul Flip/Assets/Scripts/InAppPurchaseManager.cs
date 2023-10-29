using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class InAppPurchaseManager : MonoBehaviour, IDetailedStoreListener
{
    public static InAppPurchaseManager inAppPurchaseManager;

    private IStoreController iStoreController;
    private IExtensionProvider iExtensionProvider;

    public const string REMOVE_ADS_ID = "remove_ads";
    public const ProductType REMOVE_ADS_TYPE = ProductType.NonConsumable;

    public delegate void PurchaseCallback();
    private PurchaseCallback removeAdsPurchaseSuccessCallback, removeAdsPurchaseFailCallback;
    private PurchaseCallback restorePurchaseSuccessCallback, restorePurchaseFailCallback;

    private InAppPurchaseInformation inAppPurchaseInformation;
    private const string IN_APP_PURCHASE_INFORMATION_SAVE_PATH = "inAppPurchaseInformation.asset";

    private void Awake()
    {
        if(null != inAppPurchaseManager)
        {
            Destroy(gameObject);
            return;
        }
        inAppPurchaseManager = this;
        DontDestroyOnLoad(gameObject);

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(REMOVE_ADS_ID, REMOVE_ADS_TYPE);
        UnityPurchasing.Initialize(this, builder);

        string inAppPurchaseInformationSavePath = Path.Combine(Application.persistentDataPath, IN_APP_PURCHASE_INFORMATION_SAVE_PATH);
        string inAppPurchaseInformationString = "";
        if(File.Exists(inAppPurchaseInformationSavePath))
        {
            using(var stream = File.Open(inAppPurchaseInformationSavePath, FileMode.Open))
            {
                using(var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    inAppPurchaseInformationString = reader.ReadString();
                }
            }
        }
        inAppPurchaseInformation = InAppPurchaseInformation.Base16Decode(inAppPurchaseInformationString);
        if(null == inAppPurchaseInformation)
        {
            inAppPurchaseInformation = ScriptableObject.CreateInstance<InAppPurchaseInformation>();
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        iStoreController = controller;
        iExtensionProvider = extensions;
        Debug.Log("In App Purchase Initialized");
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {

    }
    public void OnInitializeFailed(InitializationFailureReason error, string s)
    {

    }
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        string purchaseItemID = product.definition.id;
        Debug.Log($"Fail to purchase {purchaseItemID}");
        if(REMOVE_ADS_ID == purchaseItemID)
        {
            removeAdsPurchaseFailCallback.Invoke();
        }
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        string purchaseItemID = product.definition.id;
        Debug.Log($"Fail to purchase {purchaseItemID}");
        if(REMOVE_ADS_ID == purchaseItemID)
        {
            removeAdsPurchaseFailCallback.Invoke();
        }
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        string purchaseItemID = purchaseEvent.purchasedProduct.definition.id;
        Debug.Log($"Purchase {purchaseItemID}");
        if(REMOVE_ADS_ID == purchaseItemID)
        {
            inAppPurchaseInformation.hasRemoveAds = 1;
            removeAdsPurchaseSuccessCallback.Invoke();
        }

        string inAppPurchaseInformationSavePath = Path.Combine(Application.persistentDataPath, IN_APP_PURCHASE_INFORMATION_SAVE_PATH);
        using (var stream = File.Open(inAppPurchaseInformationSavePath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(inAppPurchaseInformation.Base16Encode());
            }
        }
        
        return PurchaseProcessingResult.Complete;
    }


    public void AssignRestorePurchaseCallbacks(PurchaseCallback restoreSuccessCallback, PurchaseCallback restoreFailCallback)
    {
        restorePurchaseSuccessCallback = restoreSuccessCallback;
        restorePurchaseFailCallback = restoreFailCallback;
    }
    public void RestorePurchase()
    {
        iExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((isSuccess, errorMessage) => {
        if (isSuccess)
        {
            // This does not mean anything was restored,
            // merely that the restoration process succeeded.
            restorePurchaseSuccessCallback.Invoke();
            Debug.Log("Start restore");
        } else
        {
            // Restoration failed.
            restorePurchaseFailCallback.Invoke();
            Debug.Log("Fail to start restore");
        }
    });
    }

    public void AssignPurchaseRemoveAdsCallbacks(PurchaseCallback purchaseSuccessCallback, PurchaseCallback purchaseFailCallback)
    {
        removeAdsPurchaseSuccessCallback = purchaseSuccessCallback;
        removeAdsPurchaseFailCallback = purchaseFailCallback;
    }
    public void PurchaseRemoveAds()
    {
        iStoreController.InitiatePurchase(REMOVE_ADS_ID);
    }
    public bool HasRemoveAds()
    {
        return inAppPurchaseInformation.hasRemoveAds == 1;
    }
}
