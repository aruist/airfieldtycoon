using System;
using System.Linq;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.ObjectModel;

public class Kauppa : MonoBehaviour
{
    public static Kauppa Instance;
    public static event Action OnSetAdFree;
    public static event Action OnDisableBannerAds;
    public static event Action OnAutotapEnabled;
    public static event Action<PROGRESS> OnKauppaProgress;
    public static event Action<List<ID>> OnKauppaSuccess;
    public static event Action<List<ID>> OnKauppaDeferred;
    public static event Action<List<ID>> OnKauppaPending;
    List<ID> startupConfirmedList;
    bool startupListUsed;

    const string k_Environment = "production";
    public struct InnAppPurchaseReference
    {
        public string productId;
        public UnityEngine.Purchasing.ProductType type;
    }

    //public HomeGUI homeGUI;
    private static StoreController m_StoreController;          // The Unity Purchasing system.

    // ProductIDs
    public enum ID
    {
        INN_APP_AUTO_TAP,
        INN_APP_FISTFUL_OF_GEMS,    // 100 Gems
        INN_APP_POUCH_OF_GEMS,      // 250
        INN_APP_BUCKET_OF_GEMS,     // 500
        INN_APP_BARREL_OF_GEMS,     // 2500
        INN_APP_AD_FREE,
        NONE
    }
    public enum PROGRESS
    {
        None,
        Started,
        Cancelled,
        Failed,
        Unavailable,
        PaymentDeclined
    }

    private bool _gameservicesInitialized;

    void Awake()
    {
        #if KAUPPA_DEBUG
        Debug.Log("Kauppa Awake()" + (Instance == null));
        #endif
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            startupConfirmedList = new();
            startupListUsed = false;
            //InitializeUnityGamingServices(OnSuccessUnityGamingServices, OnErrorUnityGamingServices);
        } else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        #if KAUPPA_DEBUG
        Debug.Log("Kauppa Start() _gameservicesInitialized: " + _gameservicesInitialized);
        #endif
        // Begin to configure our connection to Purchasing, can use button click instead
        InitializeIAP();
        // InitializePurchasing();
    }

    // void InitializeUnityGamingServices(Action onSuccess, Action<string> onError)
    // {
    //     #if KAUPPA_DEBUG
    //     Debug.Log("Kauppa InitializeUnityGamingServices");
    //     #endif
    //     try
    //     {
    //         var options = new InitializationOptions().SetEnvironmentName(k_Environment);
    //         UnityServices.InitializeAsync(options).ContinueWith(task => onSuccess());
    //     }
    //     catch (Exception exception)
    //     {
    //         onError(exception.Message);
    //     }
    // }

    void OnSuccessUnityGamingServices()
    {
        #if KAUPPA_DEBUG
        Debug.Log("Kauppa Unity Gaming Services has been successfully initialized.");
        #endif
        _gameservicesInitialized = true;
    }

    void OnErrorUnityGamingServices(string message)
    {
        #if KAUPPA_DEBUG
        Debug.LogError($"Kauppa Unity Gaming Services failed to initialize with error: {message}.");
        #endif
    }

    public string GetProductIDString(ID id)
    {
        switch (id)
        {
            case ID.INN_APP_AUTO_TAP:
                return "INN_APP_AUTO_TAP";
            case ID.INN_APP_AD_FREE:
                return "INN_APP_AD_FREE";
            case ID.INN_APP_BARREL_OF_GEMS:
                return "INN_APP_BARREL_OF_GEMS";
            case ID.INN_APP_BUCKET_OF_GEMS:
                return "INN_APP_BUCKET_OF_GEMS";
            case ID.INN_APP_FISTFUL_OF_GEMS:
                return "INN_APP_FISTFUL_OF_GEMS";
            case ID.INN_APP_POUCH_OF_GEMS:
                return "INN_APP_POUCH_OF_GEMS";
            default:
                return "none";
        }
    }

    // Unity 6000:
    async void InitializeIAP()
    {
        #if KAUPPA_DEBUG
        Debug.Log("Kauppa InitializeIAP()");
        #endif

        m_StoreController = UnityIAPServices.StoreController();

        m_StoreController.OnPurchasePending += OnPurchasePending;
        m_StoreController.OnPurchaseConfirmed += OnPurchaseConfirmed;
        m_StoreController.OnPurchaseFailed += OnPurchaseFailed;
        m_StoreController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
        m_StoreController.OnStoreDisconnected += OnStoreDisconnected;
        m_StoreController.OnPurchaseDeferred += OnPurchaseDeferred;

        await m_StoreController.Connect();
        m_StoreController.OnProductsFetched += OnProductsFetched;
        m_StoreController.OnPurchasesFetched += OnPurchasesFetched;
        m_StoreController.OnProductsFetchFailed += OnProductsFetchFailed;

        var initialProductsToFetch = new List<ProductDefinition>
        {
            #if UNITY_IOS
            new(GetProductIDString(ID.INN_APP_AD_FREE), "com.softcen.airfield.tycoon.clicker.adfree.ios", ProductType.NonConsumable),
            new(GetProductIDString(ID.INN_APP_AUTO_TAP), "com.softcen.airfield.tycoon.clicker.auto.tap.ios", ProductType.NonConsumable),
            new(GetProductIDString(ID.INN_APP_FISTFUL_OF_GEMS), "com.softcen.airfield.tycoon.clicker.fistful.of.gems.ios", ProductType.Consumable),
            new(GetProductIDString(ID.INN_APP_POUCH_OF_GEMS), "com.softcen.airfield.tycoon.clicker.pouch.of.gems.ios", ProductType.Consumable),
            new(GetProductIDString(ID.INN_APP_BUCKET_OF_GEMS), "com.softcen.airfield.tycoon.clicker.bucket.of.gems.ios", ProductType.Consumable),
            new(GetProductIDString(ID.INN_APP_BARREL_OF_GEMS), "com.softcen.airfield.tycoon.clicker.barrel.of.gems.ios", ProductType.Consumable)
            #else
            new(GetProductIDString(ID.INN_APP_AD_FREE), "com.softcen.airfield.tycoon.clicker.adfree.gp", ProductType.NonConsumable),
            new(GetProductIDString(ID.INN_APP_AUTO_TAP), "com.softcen.airfield.tycoon.clicker.auto.tap.gp", ProductType.NonConsumable),
            new(GetProductIDString(ID.INN_APP_FISTFUL_OF_GEMS), "com.softcen.airfield.tycoon.clicker.fistful.of.gems", ProductType.Consumable),
            new(GetProductIDString(ID.INN_APP_POUCH_OF_GEMS), "com.softcen.airfield.tycoon.clicker.pouch.of.gems", ProductType.Consumable),
            new(GetProductIDString(ID.INN_APP_BUCKET_OF_GEMS), "com.softcen.airfield.tycoon.clicker.bucket.of.gems", ProductType.Consumable),
            new(GetProductIDString(ID.INN_APP_BARREL_OF_GEMS), "com.softcen.airfield.tycoon.clicker.barrel.of.gems.gp", ProductType.Consumable)
            #endif
        };

        m_StoreController.FetchProducts(initialProductsToFetch);
    }

    public List<ID> GetStartupConfirmedList()
    {
        startupListUsed = true;
        return startupConfirmedList;
    }

    public void FetchPurchases()
    {
        m_StoreController.FetchPurchases();
    }

    public void RestorePurchases()
    {
        #if KAUPPA_DEBUG
        Debug.Log("Kauppa RestorePurchases Started");
        #endif
        #if UNITY_IOS
        OnKauppaProgress?.Invoke(PROGRESS.Started);
        #endif
        m_StoreController.RestoreTransactions(OnTransactionsRestored);

    }

        void OnTransactionsRestored(bool success, string error)
    {
        #if KAUPPA_DEBUG
        Debug.Log("Kauppa OnTransactionsRestored: " + success);
        #endif
        #if UNITY_IOS
        if (success && m_StoreController != null)
        {
            m_StoreController?.FetchPurchases();
        }
        else
        {
            OnKauppaProgress?.Invoke(PROGRESS.None);
        }
        #endif
    }

    void OnProductsFetchFailed(ProductFetchFailed productFetchFailed)
    {
        #if KAUPPA_DEBUG
        Debug.Log($"Kauppa OnProductsFetchFailed Reason: {productFetchFailed.FailureReason}");
        #endif
        OnKauppaProgress?.Invoke(PROGRESS.None);
    }

    void OnPurchasesFetched(Orders orders)
    {
        if (orders == null) {
            #if KAUPPA_DEBUG
            Debug.Log("Kauppa OnPurchasesFetched Orders null");
            #endif
            return;
        }

        // Process purchases, e.g. check for entitlements from completed orders
        int ccount = orders.ConfirmedOrders.Count;
        int pcount = orders.PendingOrders.Count;
        int dcount = orders.DeferredOrders.Count;
        IReadOnlyList<ConfirmedOrder> confirmed = orders?.ConfirmedOrders ?? Array.Empty<ConfirmedOrder>();
        IReadOnlyList<PendingOrder> pending   = orders?.PendingOrders   ?? Array.Empty<PendingOrder>();
        IReadOnlyList<DeferredOrder> deferred  = orders?.DeferredOrders  ?? Array.Empty<DeferredOrder>();
        #if KAUPPA_DEBUG
        Debug.Log($"Kauppa OnPurchasesFetched: confirmed={ccount}, pending={pcount}, deferred={dcount}");
        #endif

        if (confirmed.Count > 0) {
            List<ID> successIDs = new List<ID>();
            foreach (ConfirmedOrder confirmedOrder in confirmed)
            {
                ICart cartOrdered = confirmedOrder.CartOrdered;
                if (cartOrdered == null) continue;
                foreach (var item in cartOrdered.Items() ?? Enumerable.Empty<CartItem>())
                {
                    var id = item?.Product?.definition?.id;
                    if (id == null) continue;
                    #if KAUPPA_DEBUG
                    Debug.Log($"Kauppa OnPurchasesFetched ConfirmedOrder: id={id}");
                    #endif
                    ID receivedId = GetID(id);
                    if (receivedId == ID.INN_APP_AD_FREE)
                    {
                        if (GameManager.Instance.IsAdFree() == false) successIDs.Add(ID.INN_APP_AD_FREE);
                        GameManager.Instance.SetAdFree();
                        OnDisableBannerAds?.Invoke();
                    }
                    else if (receivedId == ID.INN_APP_AUTO_TAP)
                    {
                        if (GameManager.Instance.playerData.AutoTapOwned == false) successIDs.Add(ID.INN_APP_AUTO_TAP);
                        OnAutotapEnabled?.Invoke();
                        GameManager.Instance.playerData.AutoTapOwned = true;
                        GameManager.Instance.Save();
                    }
                }
            }
            if (successIDs.Count > 0)
            {
                OnKauppaSuccess?.Invoke(successIDs);
            }
        }
        if (pending.Count > 0)
        {
            List<ID> pendingIDs = new List<ID>();
            foreach (PendingOrder pendingOrder in pending)
            {
                ICart cartOrdered = pendingOrder.CartOrdered;
                if (cartOrdered == null) continue;
                foreach (var item in cartOrdered.Items() ?? Enumerable.Empty<CartItem>())
                {
                    var id = item?.Product?.definition?.id;
                    if (id == null) continue;
                    ID receivedId = GetID(id);
                    if (receivedId != ID.NONE) pendingIDs.Add(receivedId);
                    #if KAUPPA_DEBUG
                    Debug.Log($"Kauppa OnPurchasesFetched PendingOrder: id={id}");
                    #endif
                }
            }
            if (pendingIDs.Count > 0) OnKauppaPending?.Invoke(pendingIDs);

        }
        if (deferred.Count > 0)
        {
            List<ID> deferredIDs = new List<ID>();
            foreach (DeferredOrder deferredOrder in deferred)
            {
                ICart cartOrdered = deferredOrder.CartOrdered;
                if (cartOrdered == null) continue;
                foreach (var item in cartOrdered.Items() ?? Enumerable.Empty<CartItem>())
                {
                    var id = item?.Product?.definition?.id;
                    if (id == null) continue;
                    ID receivedId = GetID(id);
                    if (receivedId != ID.NONE) deferredIDs.Add(receivedId);
                    #if KAUPPA_DEBUG
                    Debug.Log($"Kauppa OnPurchasesFetched DeferredOrder: id={id}");
                    #endif
                }
            }
            if (deferredIDs.Count > 0) OnKauppaDeferred?.Invoke(deferredIDs);
        }
        OnKauppaProgress?.Invoke(PROGRESS.None);
    }

    void OnProductsFetched(List<Product> products)
    {
        #if KAUPPA_DEBUG
        Debug.Log("Kauppa OnProductsFetched List<Product>: ");
        foreach (var product in products)
        {
            Debug.Log($"Kauppa OnProductsFetched: {product.definition.id}");
        }
        #endif
        // Handle fetched products
        m_StoreController.FetchPurchases();

    }

    void OnPurchaseDeferred(DeferredOrder order)
    {
        #if KAUPPA_DEBUG
        Debug.Log($"Kauppa OnPurchaseDeferred. Purchase of {order.CartOrdered.Items().FirstOrDefault()?.Product.definition.id} is deferred");
        #endif
        List<ID> deferredIDs = new();
        foreach (var cartItem in order.CartOrdered.Items())
        {
            Product product = cartItem.Product;
            #if KAUPPA_DEBUG
            //If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Debug.Log($"Kauppa OnPurchaseDeferred: Product: {product.definition.id}");
            #endif
            ID productID = GetID(product.definition.id);
            if (productID != ID.NONE) deferredIDs.Add(productID);
        }
        if (deferredIDs.Count > 0) OnKauppaDeferred?.Invoke(deferredIDs);
        OnKauppaProgress?.Invoke(PROGRESS.None);
    }

    void OnStoreDisconnected(StoreConnectionFailureDescription description)
    {
        #if KAUPPA_DEBUG
        Debug.Log($"Kauppa OnStoreDisconnected: {description.message}");
        #endif
    }

    public void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
    {
        #if KAUPPA_DEBUG
        Debug.Log($"Kauppa OnPurchasesFetchFailed: {failure.Message}");
        #endif
        OnKauppaProgress?.Invoke(PROGRESS.None);
    }

    void OnPurchaseFailed(FailedOrder failedOrder)
    {
        var reason = failedOrder.FailureReason;
        // User cancelled or somethin else
        #if KAUPPA_DEBUG
        Debug.Log($"Kauppa OnPurchaseFailed: {failedOrder.CartOrdered.Items().First().Product.definition.id}, {reason}");
        foreach (var cartItem in failedOrder.CartOrdered.Items())
        {
            Debug.Log($"Kauppa OnPurchaseFailed: {cartItem.Product}. {reason}");
            //m_PaywallManager.m_IAPLogger.LogFailedPurchase(cartItem.Product, reason);
        }

        #endif
        #if UNITY_IOS
        switch (reason)
        {
            case PurchaseFailureReason.UserCancelled:
                OnKauppaProgress?.Invoke(PROGRESS.Cancelled);
                break;
            case PurchaseFailureReason.ProductUnavailable:
                OnKauppaProgress?.Invoke(PROGRESS.Unavailable);
                break;
            case PurchaseFailureReason.PurchasingUnavailable:
                OnKauppaProgress?.Invoke(PROGRESS.PaymentDeclined);
                break;
            default:
                OnKauppaProgress?.Invoke(PROGRESS.None);
                break;

        }
        #endif
    }

    /// <summary>
    /// The OnPurchasePending callback is invoked when a purchase is made and is awaiting fulfillment.
    /// Your application should fulfill the purchase at this point, for example by unlocking local content or
    /// sending the purchase receipt to a server to update a server-side game model.
    /// </summary>
    /// <param name="order"></param>
    private void OnPurchasePending(PendingOrder order)
    {
        #if KAUPPA_DEBUG
        string receipt = order.Info.Receipt;
        Debug.Log($"Kauppa OnPurchasePending receipt: {receipt}");
        #endif
        // Add your validations here before confirming the purchase.
        PurchaseConfirmed(order);

        // Before confirming the purchase, reward the entitlement to the player.

        m_StoreController.ConfirmPurchase(order);
        #if UNITY_IOS
        OnKauppaProgress?.Invoke(PROGRESS.None);
        #endif
    }

    public void PurchaseConfirmed(Order order)
    {
        #if KAUPPA_DEBUG
        Debug.Log($"Kauppa PurchaseConfirmed ConfirmedOrder");
        #endif
        List<ID> confirmedList = new List<ID>();
        foreach (var cartItem in order.CartOrdered.Items())
        {
            Product product = cartItem.Product;
            #if KAUPPA_DEBUG
            //If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            Debug.Log($"Kauppa PurchaseConfirmed: Product: {product.definition.id}");
            #endif
            ID productID = GetID(product.definition.id);
            if (String.Equals(product.definition.id, GetProductIDString(ID.INN_APP_AD_FREE), StringComparison.Ordinal))
            {
                confirmedList.Add(productID);
                OnSetAdFree?.Invoke();
                GameManager.Instance.SetAdFree();
                OnDisableBannerAds?.Invoke();
            }
            if (String.Equals(product.definition.id, GetProductIDString(ID.INN_APP_AUTO_TAP), StringComparison.Ordinal))
            {
                confirmedList.Add(productID);
                OnAutotapEnabled?.Invoke();
                GameManager.Instance.playerData.AutoTapOwned = true;
                GameManager.Instance.Save();
            }
            else if (String.Equals(product.definition.id, GetProductIDString(ID.INN_APP_BARREL_OF_GEMS), StringComparison.Ordinal))
            {
                confirmedList.Add(productID);
                GameManager.Instance.playerData.ChangeDiamonds(GetGemsCount(ID.INN_APP_BARREL_OF_GEMS));
                GameManager.Instance.Save();
            }
            else if (String.Equals(product.definition.id, GetProductIDString(ID.INN_APP_BUCKET_OF_GEMS), StringComparison.Ordinal))
            {
                confirmedList.Add(productID);
                GameManager.Instance.playerData.ChangeDiamonds(GetGemsCount(ID.INN_APP_BUCKET_OF_GEMS));
                GameManager.Instance.Save();
            }
            else if (String.Equals(product.definition.id, GetProductIDString(ID.INN_APP_FISTFUL_OF_GEMS), StringComparison.Ordinal))
            {
                confirmedList.Add(productID);
                GameManager.Instance.playerData.ChangeDiamonds(GetGemsCount(ID.INN_APP_FISTFUL_OF_GEMS));
                GameManager.Instance.Save();
            }
            else if (String.Equals(product.definition.id, GetProductIDString(ID.INN_APP_POUCH_OF_GEMS), StringComparison.Ordinal))
            {
                confirmedList.Add(productID);
                GameManager.Instance.playerData.ChangeDiamonds(GetGemsCount(ID.INN_APP_POUCH_OF_GEMS));
                GameManager.Instance.Save();
            }
        }
        // OnPurchasedItem?.Invoke();
        if (confirmedList.Count > 0) OnKauppaSuccess?.Invoke(confirmedList);
        if (!startupListUsed) {
            startupConfirmedList.Clear();
            startupConfirmedList.AddRange(confirmedList);
        }
    }

    public ID GetID(string id)
    {
        if (String.Equals(id, GetProductIDString(ID.INN_APP_AD_FREE), StringComparison.Ordinal))
        {
            return ID.INN_APP_AD_FREE;
        }
        else if (String.Equals(id, GetProductIDString(ID.INN_APP_AUTO_TAP), StringComparison.Ordinal))
        {
            return ID.INN_APP_AUTO_TAP;
        }
        else if (String.Equals(id, GetProductIDString(ID.INN_APP_BARREL_OF_GEMS), StringComparison.Ordinal))
        {
            return ID.INN_APP_BARREL_OF_GEMS;
        }
        else if (String.Equals(id, GetProductIDString(ID.INN_APP_BUCKET_OF_GEMS), StringComparison.Ordinal))
        {
            return ID.INN_APP_BUCKET_OF_GEMS;
        }
        else if (String.Equals(id, GetProductIDString(ID.INN_APP_FISTFUL_OF_GEMS), StringComparison.Ordinal))
        {
            return ID.INN_APP_FISTFUL_OF_GEMS;
        }
        else if (String.Equals(id, GetProductIDString(ID.INN_APP_POUCH_OF_GEMS), StringComparison.Ordinal))
        {
            return ID.INN_APP_POUCH_OF_GEMS;
        }
        return ID.NONE;
    }

    void OnPurchaseConfirmed(Order order)
    {
        #if KAUPPA_DEBUG
        Debug.Log($"Kauppa OnPurchaseConfirmed Order");
        #endif
        switch (order)
        {
            case FailedOrder failedOrder:
                #if KAUPPA_DEBUG
                Debug.Log($"Purchase confirmation failed: {failedOrder.CartOrdered.Items().First().Product.definition.id}, {failedOrder.FailureReason.ToString()}, {failedOrder.Details}");
                #endif
                break;
            case ConfirmedOrder:
                #if KAUPPA_DEBUG
                Debug.Log($"Purchase completed: {order.CartOrdered.Items().First().Product.definition.id}");
                #endif
                break;
        }
    }


    public void BuyProductID(ID id)
    {
        string productId = GetProductIDString(id);
        #if KAUPPA_DEBUG
        Debug.Log(string.Format("Kauppa Purchasing product:" + id.ToString() + " = " + productId));
        #endif
        var product = m_StoreController?.GetProducts().FirstOrDefault(product => product.definition.id == productId);

        if (product != null)
        {
            #if UNITY_IOS
            OnKauppaProgress?.Invoke(PROGRESS.Started);
            #endif

            m_StoreController?.PurchaseProduct(product);
        }
        else
        {
            #if KAUPPA_DEBUG
            Debug.Log($"The product service has no product with the ID {productId}");
            #endif
        }

        // if (IsInitialized())
        // {

        //     UnityEngine.Purchasing.Product product = m_StoreController.products.WithID(productId);

        //     if (product != null && product.availableToPurchase)
        //     {
        //         #if KAUPPA_DEBUG
        //         Debug.Log(string.Format("Purchasing product:" + product.definition.id.ToString()));
        //         #endif
        //         m_StoreController.InitiatePurchase(product);
        //     }
        //     else
        //     {
        //         #if KAUPPA_DEBUG
        //         Debug.Log("Kauppa BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
        //         #endif
        //     }
        // }
        // else
        // {
        //     #if KAUPPA_DEBUG
        //     Debug.Log("Kauppa BuyProductID FAIL. Not initialized.");
        //     #endif
        // }
    }

    public void GetProductMetadata(Kauppa.ID id, out string title, out string price, out string description)
    {
        price = "-";
        title = "-";
        description = "";
        if (m_StoreController == null) return;

        Product product = FindProduct(GetProductIDString(id));
        if (product != null)
        {
            #if KAUPPA_DEBUG
            Debug.Log("Kauppa GetProductPrice. localizedPriceString: " + product.metadata.localizedPriceString +
                ", isoCurrencyCode: " + product.metadata.isoCurrencyCode +
                ", localizedPrice: " + product.metadata.localizedPrice);
            #endif

            price = product.metadata.localizedPriceString;
            title = product.metadata.localizedTitle;
            description = product.metadata.localizedDescription;
            return;
        }
        #if KAUPPA_DEBUG
        Debug.Log("Kauppa GetProductMetadata " + id + " not found.");
        #endif
    }

    public Product FindProduct(string productId)
    {
        return GetFetchedProducts()?.FirstOrDefault(product => product.definition.id == productId);
    }
    public ReadOnlyObservableCollection<Product> GetFetchedProducts()
    {
        return m_StoreController?.GetProducts();
    }

    public string GetProductTitle(ID id)
    {
        string title = "-";
        switch (id)
        {
            case ID.INN_APP_BARREL_OF_GEMS:
                title = "Barrel of gems";
                break;
            case ID.INN_APP_BUCKET_OF_GEMS:
                title = "Bucket of gems";
                break;
            case ID.INN_APP_FISTFUL_OF_GEMS:
                title = "Fistful of gems";
                break;
            case ID.INN_APP_POUCH_OF_GEMS:
                title = "Pouch of gems";
                break;
            case ID.INN_APP_AD_FREE:
                title = "Disable Banner Ads";
                break;
            case ID.INN_APP_AUTO_TAP:
                title = "Auto Tap";
                break;
        }
        return title;
    }
    public int GetGemsCount(ID id)
    {
        int count = 0;
        switch (id)
        {
            case ID.INN_APP_FISTFUL_OF_GEMS:
                count = 100;
                break;
            case ID.INN_APP_POUCH_OF_GEMS:
                count = 250;
                break;
            case ID.INN_APP_BUCKET_OF_GEMS:
                count = 500;
                break;
            case ID.INN_APP_BARREL_OF_GEMS:
                count = 2500;
                break;
        }
        return count;
    }

    public string Tuotteenhinta(ID id)
    {
        if (m_StoreController != null)
        {
            Product product = FindProduct(GetProductIDString(id));
            if (product != null)
            {
                #if KAUPPA_DEBUG
                Debug.Log("Kauppa GetProductPrice. localizedPriceString: " + product.metadata.localizedPriceString +
                    ", isoCurrencyCode: " + product.metadata.isoCurrencyCode +
                    ", localizedPrice: " + product.metadata.localizedPrice);
                #endif
                return product.metadata.localizedPriceString;
            }
        }
        return "";
    }
}

    /// <summary>
    /// Called when a purchase fails.
    /// IStoreListener.OnPurchaseFailed is deprecated,
    /// use IDetailedStoreListener.OnPurchaseFailed instead.
    /// </summary>
    // public void OnPurchaseFailed (Product i, PurchaseFailureReason p)
    // {
    //     #if KAUPPA_DEBUG
    //     Debug.Log(string.Format("Kauppa OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", i.definition.storeSpecificId, p));
    //     #endif
    // }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    // public void OnPurchaseFailed (Product i, PurchaseFailureDescription p)
    // {
    //     #if KAUPPA_DEBUG
    //     Debug.Log(string.Format("Kauppa OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureDescription: {1}", i.definition.storeSpecificId, p));
    //     #endif
    // }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    ///
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    // private void OnDeferred(Product item)
    // {
    //     #if KAUPPA_DEBUG
    //     Debug.Log("Purchase deferred: " + item.definition.id);
    //     #endif
    // }

    // public void InitializePurchasing()
    // {
    //     #if KAUPPA_DEBUG
    //     Debug.Log("Kauppa InitializePurchasing() IsInitialized: " + IsInitialized());
    //     #endif
    //     if (IsInitialized())
    //     {
    //         return;
    //     }

    //     var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
    //     string gp = M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.gpki1);
    //     string io = M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.iski1);
    //     #if KAUPPA_DEBUG
    //     if (gp != "com.softcen.airfield.tycoon.clicker.auto.tap.gp" || io != "com.softcen.airfield.tycoon.clicker.auto.tap.ios")
    //     {
    //         Debug.LogError("InApp mismatch!");
    //     }
    //     #endif
    //     builder.AddProduct(GetIdString(ID.INN_APP_AUTO_TAP), ProductType.NonConsumable, new IDs
    //     {
    //         { "com.softcen.airfield.tycoon.clicker.auto.tap.gp", GooglePlay.Name }
    //         ,{ "com.softcen.airfield.tycoon.clicker.auto.tap.ios", AppleAppStore.Name }
    //         //{gp, GooglePlay.Name},
    //         //{io, MacAppStore.Name}
    //     });
    //     gp = M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.gpki4);
    //     io = M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.iski4);
    //     #if KAUPPA_DEBUG
    //     if (gp != "com.softcen.airfield.tycoon.clicker.fistful.of.gems" || io != "com.softcen.airfield.tycoon.clicker.fistful.of.gems.ios")
    //     {
    //         Debug.LogError("InApp mismatch!");
    //     }
    //     #endif
    //     builder.AddProduct(GetIdString(ID.INN_APP_FISTFUL_OF_GEMS), ProductType.Consumable, new IDs
    //     {
    //         { "com.softcen.airfield.tycoon.clicker.fistful.of.gems", GooglePlay.Name }
    //         ,{ "com.softcen.airfield.tycoon.clicker.fistful.of.gems.ios", AppleAppStore.Name }
    //     });

    //     /*builder.AddProduct(ID.INN_APP_FISTFUL_OF_GEMS.ToString(), ProductType.Consumable, new IDs
    //     {
    //         {gp, GooglePlay.Name},
    //         {io, MacAppStore.Name}
    //     });*/
    //     builder.AddProduct(GetIdString(ID.INN_APP_POUCH_OF_GEMS), ProductType.Consumable, new IDs
    //     {
    //         { "com.softcen.airfield.tycoon.clicker.pouch.of.gems", GooglePlay.Name }
    //         ,{ "com.softcen.airfield.tycoon.clicker.pouch.of.gems.ios", AppleAppStore.Name }
    //         //{M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.gpki5), GooglePlay.Name},
    //         //{M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.iski5), MacAppStore.Name}
    //     });
    //     builder.AddProduct(GetIdString(ID.INN_APP_BUCKET_OF_GEMS), ProductType.Consumable, new IDs
    //     {
    //         { "com.softcen.airfield.tycoon.clicker.bucket.of.gems", GooglePlay.Name }
    //         ,{ "com.softcen.airfield.tycoon.clicker.bucket.of.gems.ios", AppleAppStore.Name }
    //         //{M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.gpki3), GooglePlay.Name},
    //         //{M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.iski3), MacAppStore.Name}
    //     });
    //     builder.AddProduct(GetIdString(ID.INN_APP_BARREL_OF_GEMS), ProductType.Consumable, new IDs
    //     {
    //         { "com.softcen.airfield.tycoon.clicker.barrel.of.gems.gp", GooglePlay.Name }
    //         ,{ "com.softcen.airfield.tycoon.clicker.barrel.of.gems.ios", AppleAppStore.Name }
    //         //{M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.gpki2), GooglePlay.Name},
    //         //{M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.iski2), MacAppStore.Name}
    //     });
    //     builder.AddProduct(GetIdString(ID.INN_APP_AD_FREE), ProductType.NonConsumable, new IDs
    //     {
    //         { "com.softcen.airfield.tycoon.clicker.adfree.gp", GooglePlay.Name }
    //         ,{ "com.softcen.airfield.tycoon.clicker.adfree.ios", AppleAppStore.Name }
    //         //{M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.gpki6), GooglePlay.Name},
    //         //{M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.iski6), MacAppStore.Name}
    //     });

    //     #if KAUPPA_DEBUG
    //     Debug.Log("Kauppa Starting Initialized...");
    //     #endif
    //     UnityPurchasing.Initialize(this, builder);

    // }

    // private bool IsInitialized()
    // {
    //     bool retVal = m_StoreController != null && m_StoreExtensionProvider != null;
    //     //#if KAUPPA_DEBUG
    //     //Debug.Log("Kauppa IsInitialized: " + retVal);
    //     //#endif
    //     return retVal;
    // }

    // public void RestorePurchases()
    // {
    //     if (!IsInitialized())
    //     {
    //         #if KAUPPA_DEBUG
    //         Debug.Log("Kauppa RestorePurchases Fail. Not initialized");
    //         #endif
    //         return;
    //     }
    //     if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
    //     {
    //         #if KAUPPA_DEBUG
    //         Debug.Log("Kauppa RestorePurchases Started...");
    //         #endif
    //         m_AppleExtensions.RestoreTransactions(OnRestore);
    //         /*m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(result =>
    //         {
    //             if (result)
    //             {
    //                 #if KAUPPA_DEBUG
    //                 Debug.Log("Kauppa Restore purchases succeeded and continuing: " + result + ". If no further messages, no purchases available to restore.");
    //                 #endif
    //             }
    //             else
    //             {
    //                 #if KAUPPA_DEBUG
    //                 Debug.Log("Kauppa Restore purchases failed.");
    //                 #endif
    //             }
    //         });*/

    //     }
    //     else
    //     {
    //         #if KAUPPA_DEBUG
    //         Debug.Log("Kauppa RestorePurchases FAIL. Not supported on this platform");
    //         #endif
    //     }
    // }

    // void OnRestore(bool success, string error)
    // {
    //     // var restoreMessage = "";
    //     if (success)
    //     {
    //         // This does not mean anything was restored,
    //         // merely that the restoration process succeeded.
    //         #if KAUPPA_DEBUG
    //         Debug.Log("Kauppa OnRestore success: " + success + ", error: " + error);
    //         #endif
    //     }
    //     else
    //     {
    //         // Restoration failed.
    //         #if KAUPPA_DEBUG
    //         Debug.Log("Kauppa OnRestore FAILED success: " + success + ", error: " + error);
    //         #endif
    //     }
    // }

    // public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    // {
    //     #if KAUPPA_DEBUG
    //     Debug.Log("Kauppa OnInitialized: In-App Purchasing successfully initialized");
    //     #endif
    //     m_StoreController = controller;
    //     m_StoreExtensionProvider = extensions;
    //     m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
    //     // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
    //     // On non-Apple platforms this will have no effect; OnDeferred will never be called.
    //     m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

    //     m_GoogleExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

    //     //m_GoogleExtensions?.SetDeferredPurchaseListener(OnPurchaseDeferred);

    //     Dictionary<string, string> dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

    //     foreach (UnityEngine.Purchasing.Product item in controller.products.all)
    //     {
    //         #if KAUPPA_DEBUG
    //         Debug.Log("Kauppa item: " + item.definition.id + ", " + item.ToString() + ", " + item.metadata.localizedTitle + ", " + item.metadata.localizedPrice + ", " + item.metadata.localizedDescription);
    //         #endif

    //         //if (item.definition.id.Equals(AD_FREE))
    //         //{
    //             //#if KAUPPA_DEBUG
    //             //Debug.Log("Kauppa AD_FREE localizedPriceString: " + item.metadata.localizedPriceString +
    //             //    ", localizedDescription: " + item.metadata.localizedDescription + ", localizedTitle: " + item.metadata.localizedTitle);
    //             //#endif
    //         //}
    //         if (item.receipt != null)
    //         {
    //             string intro_json = (dict == null || !dict.ContainsKey(item.definition.storeSpecificId)) ? null : dict[item.definition.storeSpecificId];

    //             if (item.definition.type == ProductType.Subscription)
    //             {
    //                 #if KAUPPA_DEBUG
    //                 SubscriptionManager p = new SubscriptionManager(item, intro_json);
    //                 SubscriptionInfo info = p.getSubscriptionInfo();
    //                 Debug.Log("Kauppa SubInfo: " + info.getProductId().ToString());
    //                 Debug.Log("Kauppa isSubscribed: " + info.isSubscribed().ToString());
    //                 Debug.Log("Kauppa isFreeTrial: " + info.isFreeTrial().ToString());
    //                 #endif
    //             }
    //         }
    //     }
    // }
    // public void ListProducts()
    // {

    //     foreach (UnityEngine.Purchasing.Product item in m_StoreController.products.all)
    //     {
    //         if (item.receipt != null)
    //         {
    //             #if KAUPPA_DEBUG
    //             Debug.Log("Kauppa Receipt found for Product = " + item.definition.id.ToString());
    //             #endif
    //         }
    //     }
    // }

