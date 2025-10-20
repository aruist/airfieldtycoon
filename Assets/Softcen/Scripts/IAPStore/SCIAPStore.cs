#if EI_KAYTOSSA
using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class SCIAPStore : MonoBehaviour {
    public static SCIAPStore Instance = null;

    public enum ID
    {
        IAP_ID_FISTFUL_OF_GEMS,
        IAP_ID_POUCH_OF_GEMS,
        IAP_ID_BUCKET_OF_GEMS,
        IAP_ID_BARREL_OF_GEMS,
        IAP_ID_ADFREE,
        IAP_ID_AUTOTAP,
        //IAP_ID_WAGON_OF_GEMS,
        //IAP_ID_MOUNTAIN_OF_GEMS
    }

    public int ItemsCount { get { return 6; } }

    public SCIAPManager m_SCIAPManager;

	void Awake () {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        m_SCIAPManager = new SCIAPManager(this);
    }

    public string GetProductIDString(ID id)
    {
        string retVal = "";
        switch (id)
        {
            case ID.IAP_ID_ADFREE:
                retVal = "IAP_ID_ADFREE";
                break;
            case ID.IAP_ID_AUTOTAP:
                retVal = "IAP_ID_AUTOTAP";
                break;
            case ID.IAP_ID_BARREL_OF_GEMS:
                retVal = "IAP_ID_BARREL_OF_GEMS";
                break;
            case ID.IAP_ID_BUCKET_OF_GEMS:
                retVal = "IAP_ID_BUCKET_OF_GEMS";
                break;
            case ID.IAP_ID_FISTFUL_OF_GEMS:
                retVal = "IAP_ID_FISTFUL_OF_GEMS";
                break;
            case ID.IAP_ID_POUCH_OF_GEMS:
                retVal = "IAP_ID_POUCH_OF_GEMS";
                break;
        }
        return retVal;
    }


    public void AddProducts(ConfigurationBuilder builder)
    {
#if SOFTCEN_DEBUG
        string key = M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl5);
        if (key == "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAhfFxU+ydEdv8PMpdl4MIdKrJd40UsVqrXXlez2wwGVKdFIxLzCNxHxMBLg9NClDAxonaO/rqJiL+kqqHFjiCRAdZ3nTkd1vKTjCvpSjYkLaJNjl73nv+igQa75e8M5hv8qtOeV2cj6JX+JxOop2ji2Rdu+DbRF+c9XTg4yxvgs4HmVjS8lZ4+FvDIdTrH1cYlsPMt5FX4LllnI6mCRC9WXICAWMo0eBwpfym00g0ZgTvweojGIcEKANSNMp6wB3YBTSWZlVs+7L+0Y5jjUKciF6i5T3t5mNhIyY6PoQTlA/LI+XxeHFgU2dsFgkHgggf5gbgSH3qUrm4WJaIIZDNPQIDAQAB")
        {
            Debug.Log("<color=green>AddProducts key ok</color>");
        }
        else
        {
            Debug.LogError("AddProducts key FAIL");
        }
#endif

        builder.Configure<IGooglePlayConfiguration>().SetPublicKey(M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl5));

        builder.AddProduct(GetProductIDString(ID.IAP_ID_ADFREE), ProductType.NonConsumable, new IDs
        {
            { "com.softcen.airfield.tycoon.clicker.adfree.gp", GooglePlay.Name }
            ,{ "com.softcen.airfield.tycoon.clicker.adfree.ios", AppleAppStore.Name }
        });
        builder.AddProduct(GetProductIDString(ID.IAP_ID_AUTOTAP), ProductType.NonConsumable, new IDs
        {
            { "com.softcen.airfield.tycoon.clicker.auto.tap.gp", GooglePlay.Name }
            ,{ "com.softcen.airfield.tycoon.clicker.auto.tap.ios", AppleAppStore.Name }
        });
        builder.AddProduct(GetProductIDString(ID.IAP_ID_FISTFUL_OF_GEMS), ProductType.Consumable, new IDs
        {
            { "com.softcen.airfield.tycoon.clicker.fistful.of.gems", GooglePlay.Name }
            ,{ "com.softcen.airfield.tycoon.clicker.fistful.of.gems.ios", AppleAppStore.Name }
        });
        builder.AddProduct(GetProductIDString(ID.IAP_ID_POUCH_OF_GEMS), ProductType.Consumable, new IDs
        {
            { "com.softcen.airfield.tycoon.clicker.pouch.of.gems", GooglePlay.Name }
            ,{ "com.softcen.airfield.tycoon.clicker.pouch.of.gems.ios", AppleAppStore.Name }
        });
        builder.AddProduct(GetProductIDString(ID.IAP_ID_BUCKET_OF_GEMS), ProductType.Consumable, new IDs
        {
            { "com.softcen.airfield.tycoon.clicker.bucket.of.gems", GooglePlay.Name }
            ,{ "com.softcen.airfield.tycoon.clicker.bucket.of.gems.ios", AppleAppStore.Name }
        });
        builder.AddProduct(GetProductIDString(ID.IAP_ID_BARREL_OF_GEMS), ProductType.Consumable, new IDs
        {
            { "com.softcen.airfield.tycoon.clicker.barrel.of.gems.gp", GooglePlay.Name }
            ,{ "com.softcen.airfield.tycoon.clicker.barrel.of.gems.ios", AppleAppStore.Name }
        });
    }


    public void BuyProduct(ID id)
    {
        if (m_SCIAPManager != null)
        {
            string productId = GetProductIDString(id);
            if (productId != "")
            {
#if SOFTCEN_DEBUG
                Debug.Log("SCIAPStore BuyProduct: " + productId);
#endif
                m_SCIAPManager.BuyProduct(productId);
            }
        }
    }

    public void RestorePurchases()
    {
        if (m_SCIAPManager != null)
            m_SCIAPManager.RestorePurchases();
    }

    public string GetProductPrice(ID id)
    {
        return m_SCIAPManager.GetProductPrice(id);
    }

    public int GetGemsCount(ID id)
    {
        int count = 0;
        switch (id)
        {
            case ID.IAP_ID_FISTFUL_OF_GEMS:
                count = 100;
                break;
            case ID.IAP_ID_POUCH_OF_GEMS:
                count = 250;
                break;
            case ID.IAP_ID_BUCKET_OF_GEMS:
                count = 500;
                break;
            case ID.IAP_ID_BARREL_OF_GEMS:
                count = 2500;
                break;
        }
        return count;
    }


    public string GetProductTitle(ID id)
    {
        string title = "-";
        switch (id)
        {
            case ID.IAP_ID_BARREL_OF_GEMS:
                title = "Barrel of gems";
                break;
            case ID.IAP_ID_BUCKET_OF_GEMS:
                title = "Bucket of gems";
                break;
            case ID.IAP_ID_FISTFUL_OF_GEMS:
                title = "Fistful of gems";
                break;
            /*case ID.IAP_ID_MOUNTAIN_OF_GEMS:
                title = "Mountain of gems";
                break;*/
            case ID.IAP_ID_POUCH_OF_GEMS:
                title = "Pouch of gems";
                break;
            /*case ID.IAP_ID_WAGON_OF_GEMS:
                title = "Wagon of gems";
                break;*/
            case ID.IAP_ID_ADFREE:
                title = "Disable Banner Ads";
                break;
            case ID.IAP_ID_AUTOTAP:
                title = "Auto Tap";
                break;
        }
        return title;
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        // Validation
        /*
#if SOFTCEN_DEBUG
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        #else
        new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        #endif
        try
        {
            #if SOFTCEN_DEBUG
            var result = validator.Validate(e.purchasedProduct.receipt);
            Debug.Log("SCIAPStore ProcessPurchase Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
            #endif
        }
        catch         //catch (IAPSecurityException)
        {
            #if SOFTCEN_DEBUG
            Debug.Log("SCIAPStore ProcessPurchase Invalid receipt, not unlocking content");
            #endif
            #if !UNITY_EDITOR
            return PurchaseProcessingResult.Complete;
            #endif
        }*/


        #if SOFTCEN_DEBUG
        Debug.Log(string.Format("SCIAPStore ProcessPurchase: Product: '{0}'", e.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
        #endif
        if (String.Equals(e.purchasedProduct.definition.id, GetProductIDString(ID.IAP_ID_ADFREE), StringComparison.Ordinal))
        {
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Shop", "Adfree Purchased", 1);
            GameManager.Instance.SetAdFree();
        }
        else if (String.Equals(e.purchasedProduct.definition.id, GetProductIDString(ID.IAP_ID_AUTOTAP), StringComparison.Ordinal))
        {
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Shop", "AutoTap Purchased", 1);
            GameManager.Instance.playerData.AutoTapOwned = true;
            GameManager.Instance.Save();
        }
        else if (String.Equals(e.purchasedProduct.definition.id, GetProductIDString(ID.IAP_ID_BARREL_OF_GEMS), StringComparison.Ordinal))
        {
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Shop", "Barrel of Gems Purchased", 1);
            GameManager.Instance.playerData.ChangeDiamonds(GetGemsCount(ID.IAP_ID_BARREL_OF_GEMS));
            GameManager.Instance.Save();
        }
        else if (String.Equals(e.purchasedProduct.definition.id, GetProductIDString(ID.IAP_ID_BUCKET_OF_GEMS), StringComparison.Ordinal))
        {
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Shop", "Bucket of Gems Purchased", 1);
            GameManager.Instance.playerData.ChangeDiamonds(GetGemsCount(ID.IAP_ID_BUCKET_OF_GEMS));
            GameManager.Instance.Save();
        }
        else if (String.Equals(e.purchasedProduct.definition.id, GetProductIDString(ID.IAP_ID_FISTFUL_OF_GEMS), StringComparison.Ordinal))
        {
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Shop", "Fistful of Gems Purchased", 1);
            GameManager.Instance.playerData.ChangeDiamonds(GetGemsCount(ID.IAP_ID_FISTFUL_OF_GEMS));
            GameManager.Instance.Save();
        }
        else if (String.Equals(e.purchasedProduct.definition.id, GetProductIDString(ID.IAP_ID_POUCH_OF_GEMS), StringComparison.Ordinal))
        {
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Shop", "Pouch of Gems Purchased", 1);
            GameManager.Instance.playerData.ChangeDiamonds(GetGemsCount(ID.IAP_ID_POUCH_OF_GEMS));
            GameManager.Instance.Save();
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Shop", "Purchase Failed", (long)p);
    }

}
#endif