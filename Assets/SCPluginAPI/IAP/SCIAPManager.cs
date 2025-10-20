#if EI_KAYTOSSA
using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class SCIAPManager : IStoreListener {
    private SCIAPStore m_store;
    private IStoreController m_controller;
    private IExtensionProvider m_extensions;

    public SCIAPManager(SCIAPStore store)
    {
        m_store = store;
        InitializePurchasing();
    }

    private void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        m_store.AddProducts(builder);
        UnityPurchasing.Initialize(this, builder);

    }

    private bool IsInitialized()
    {
        if (m_store == null)
        {
#if SOFTCEN_DEBUG
            Debug.LogError("SCIAPManager store not set!");
#endif
        }
        return m_controller != null && m_extensions != null && m_store != null;
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.m_controller = controller;
        this.m_extensions = extensions;

#if SOFTCEN_DEBUG
        /*
        Debug.Log("SCIAPManager OnInitialized: PASS");
        foreach (var product in controller.products.all)
        {
            Debug.Log(product.metadata.localizedTitle);
            Debug.Log(product.metadata.localizedDescription);
            Debug.Log(product.metadata.localizedPriceString);
        }*/
#endif
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
#if SOFTCEN_DEBUG
        Debug.Log("SCIAPManager OnInitializeFailed: " + error);
#endif
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
#if SOFTCEN_DEBUG
        Debug.Log(string.Format("SCIAPManager ProcessPurchase: PASS. Product: '{0}'", e.purchasedProduct.definition.id));
#endif
        return m_store.ProcessPurchase(e);
        //return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
#if SOFTCEN_DEBUG
        Debug.Log(string.Format("SCIAPManager OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", i.definition.storeSpecificId, p));
#endif
    }

    /// <summary>
    /// // Restore purchases previously made by this customer. Some platforms automatically restore purchases.
    /// </summary>
    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
#if SOFTCEN_DEBUG
            Debug.Log("SCIAPManager RestorePurchases FAIL. Not initialized.");
#endif
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
#if SOFTCEN_DEBUG
            Debug.Log("SCIAPManager RestorePurchases started ...");
#endif
            // Fetch the Apple store-specific subsystem.
            var apple = m_extensions.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
#if SOFTCEN_DEBUG
                Debug.Log("SCIAPManager RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
#endif
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
#if SOFTCEN_DEBUG
            Debug.Log("SCIAPManager RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
#endif
        }
    }

    public void GetProductMetadata(SCIAPStore.ID id, out string title, out string price, out string description)
    {
        price = "-";
        title = "-";
        description = "";

        if (IsInitialized())
        {
            string productId = id.ToString();
            Product product = m_controller.products.WithID(productId);
            if (product != null)
            {
                price = product.metadata.localizedPriceString;
                title = product.metadata.localizedTitle;
                description = product.metadata.localizedDescription;
            }
        }
    }

    public string GetProductPrice(SCIAPStore.ID id)
    {
        if (IsInitialized())
        {
            string productId = id.ToString();
            Product product = m_controller.products.WithID(productId);
            if (product != null)
            {
                return product.metadata.localizedPriceString;
            }
        }
        return "-";
    }

    public void BuyProduct(string productId)
    {
        try
        {
            if (IsInitialized())
            {
                //string productId = id.ToString();
                // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                Product product = m_controller.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
#if SOFTCEN_DEBUG
                    Debug.Log(string.Format("SCIAPManager Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
#endif
                    m_controller.InitiatePurchase(product);
                }
                else
                {
#if SOFTCEN_DEBUG
                    Debug.Log("SCIAPManager BuyProductID: FAIL. (" + productId + ") Not purchasing product, either is not found or is not available for purchase");
#endif

                }
            }
            else
            {
#if SOFTCEN_DEBUG
                Debug.Log("SCIAPManager BuyProduct FAIL. Not initialized.");
#endif
            }
        }
#if SOFTCEN_DEBUG
        catch (Exception e)
        {
            Debug.Log("SCIAPManager BuyProduct: FAIL. Exception during purchase. " + e);
        }
#else
        catch
        {

        }
#endif
    }

}
#endif