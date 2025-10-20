using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if USES_BILLING
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
#endif
using System;
#if SOFTCEN_AMAZON
using com.amazon.device.iap.cpt;
#endif

namespace SoftcenSDK
{
    public class SCShop : MonoBehaviour
    {
#if USES_BILLING
        public static SCShop Instance = null;

        public static event Action OnProductInfoUpdate;
        public static event Action<string, eBillingTransactionState> OnReceiveTransactionInfoEvent;
        public Dictionary<string, string> products;

        public bool requestOnStartup = true;
        private int m_productIter;
        private BillingProduct[] m_products;
        private bool debuglog;

        private bool m_productRequestFinished = false;
        private bool m_startRestoreCompletedTransactions = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
#if SOFTCEN_DEBUG
                debuglog = true;
#else
                debuglog = false;
#endif
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void Start()
        {
            // Intialise
            products = new Dictionary<string, string>();

            m_products = NPSettings.Billing.Products;

            if (m_products.Length == 0)
            {
                if (debuglog) Debug.LogWarning("SCShop: We couldn't find any product information. Please configure.");
            }

            if (requestOnStartup)
            {
                RequestBillingProducts(m_products);
            }
        }

#if !SOFTCEN_AMAZON
        #region Native Plugin
        // Use this for initialization

#if USES_BILLING
        void OnEnable()
        {
            // Register for callbacks
            Billing.DidFinishProductsRequestEvent += DidFinishProductsRequestEvent;
            Billing.DidReceiveTransactionInfoEvent += DidReceiveTransactionInfoEvent;
        }
        void OnDisable()
        {
            // Deregister for callbacks
            Billing.DidFinishProductsRequestEvent -= DidFinishProductsRequestEvent;
            Billing.DidReceiveTransactionInfoEvent -= DidReceiveTransactionInfoEvent;
        }
#endif
        void Update()
        {
            if (m_startRestoreCompletedTransactions)
            {
                m_startRestoreCompletedTransactions = false;
                RestoreCompletedTransactions();
            }
        }

        public void RestorePurchases()
        {
            RestoreCompletedTransactions();
        }

        public string GetProductPrice(string ProductName)
        {
            for (int i=0; i < m_products.Length; i++)
            {
                if (m_products[i].Name == ProductName)
                {
                    if (products.ContainsKey(m_products[i].ProductIdentifier))
                    {
                        return products[m_products[i].ProductIdentifier];
                    }
                }
            }
            return "-.-";
        }

        public bool IsProductPurchasedWithName(string ProductName)
        {
            string productId = GetProductIdentifier(ProductName);
            if (productId != "")
            {
                return IsProductPurchased(productId);
            }
            return false;
        }

        public void BuyProductWithName(string ProductName)
        {
            string productId = GetProductIdentifier(ProductName);
            if (productId != "")
            {
                BuyProduct(productId);
            }
        }

        public string GetProductIdentifier(string ProductName)
        {
            for (int i = 0; i < m_products.Length; i++)
            {
                if (m_products[i].Name == ProductName)
                {
                    return m_products[i].ProductIdentifier;
                }
            }
            return "";
        }

#region API Methods
        public void RequestBillingProducts()
        {
            if (m_products.Length != 0 && !m_productRequestFinished)
            {
                RequestBillingProducts(m_products);
            }
        }
        private void RequestBillingProducts(BillingProduct[] _products)
        {
            NPBinding.Billing.RequestForBillingProducts(_products);
        }

        private void RestoreCompletedTransactions()
        {
            NPBinding.Billing.RestoreCompletedTransactions();
        }

        private void BuyProduct(string _productIdentifier)
        {
            NPBinding.Billing.BuyProduct(_productIdentifier);
        }

        private bool IsProductPurchased(string _productIdentifier)
        {
            return NPBinding.Billing.IsProductPurchased(_productIdentifier);
        }

#endregion

#region API Callback Methods

        private void DidFinishProductsRequestEvent(BillingProduct[] _regProductsList, string _error)
        {
#if SOFTCEN_DEBUG
            if (debuglog) Debug.Log(string.Format("SCShop: Billing products request finished. Error = {0}.", _error.GetPrintableString()));
#endif
            if (_regProductsList != null)
            {
                m_productRequestFinished = true;

#if SOFTCEN_DEBUG
                if (debuglog) Debug.Log(string.Format("SCShop: Totally {0} billing products information were received.", _regProductsList.Length));
#endif
                for (int i=0; i < _regProductsList.Length; i++)
                {
                    UpdateProduct(_regProductsList[i]);
#if SOFTCEN_DEBUG
                    if (debuglog) Debug.Log("SCShop: "+_regProductsList[i].ToString());
#endif
                }

                if (OnProductInfoUpdate != null)
                    OnProductInfoUpdate();
#if UNITY_ANDROID
                m_startRestoreCompletedTransactions = true;
#endif
            }
        }

        private void UpdateProduct(BillingProduct bp)
        {
            for (int i=0; i < m_products.Length; i++)
            {
                if (products.ContainsKey(bp.ProductIdentifier))
                {
                    products[bp.ProductIdentifier] = bp.LocalizedPrice;
                }
                else
                {
                    products.Add(bp.ProductIdentifier, bp.LocalizedPrice);
                }
            }
        }

        private void DidReceiveTransactionInfoEvent(BillingTransaction[] _transactionList, string _error)
        {

#if SOFTCEN_DEBUG
            if (debuglog) Debug.Log(string.Format("SCShop: Billing transaction finished. Error = {0}.", _error.GetPrintableString()));
#endif
            if (_transactionList != null && string.IsNullOrEmpty(_error))
            {
#if SOFTCEN_DEBUG
                if (debuglog) Debug.Log(string.Format("SCShop: Count of transaction information received = {0}.", _transactionList.Length));
#endif
                foreach (BillingTransaction _eachTransaction in _transactionList)
                {
#if SOFTCEN_DEBUG
                    if (debuglog)
                    {
                        string tmpStr = "SCShop: Product Identifier = " + _eachTransaction.ProductIdentifier;
                        tmpStr += ", Transaction State = " + _eachTransaction.TransactionState;
                        tmpStr += ", Verification State = " + _eachTransaction.VerificationState;
                        tmpStr += ", Transaction Date[UTC] = " + _eachTransaction.TransactionDateUTC;
                        tmpStr += ", Transaction Date[Local] = " + _eachTransaction.TransactionDateLocal;
                        tmpStr += ", Transaction Identifier = " + _eachTransaction.TransactionIdentifier;
                        //tmpStr += ", Transaction Receipt = " + _eachTransaction.TransactionReceipt;
                        tmpStr += ", Error = " + _eachTransaction.Error.GetPrintableString();
                        tmpStr += ", RawPurchaseData: " + _eachTransaction.RawPurchaseData;
                        Debug.Log(tmpStr);
                    }
#endif
                    if (OnReceiveTransactionInfoEvent != null)
                    {
                        if (string.IsNullOrEmpty(_eachTransaction.Error))
                        {
                            OnReceiveTransactionInfoEvent(_eachTransaction.ProductIdentifier, _eachTransaction.TransactionState);
                        }
                        else
                        {
                            OnReceiveTransactionInfoEvent(_eachTransaction.ProductIdentifier, eBillingTransactionState.FAILED);
                        }
                    }
                }

            }
            else
            {
                if (OnReceiveTransactionInfoEvent != null)
                {
                    OnReceiveTransactionInfoEvent("", eBillingTransactionState.FAILED);
                }
            }
        }

#endregion

#endregion
#endif

#if SOFTCEN_AMAZON
#region Amazon Plugin

        private IAmazonIapV2 iapService = null;

        // Use this for initialization

        void OnEnable()
        {
            // Register for callbacks
            iapService = AmazonIapV2Impl.Instance;
            iapService.AddGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseEvent);
            iapService.AddGetProductDataResponseListener(GetProductDataResponseEvent);
            iapService.AddPurchaseResponseListener(PurchaseResponseEvent);
            iapService.AddGetUserDataResponseListener(GetUserDataResponseEvent);

        }
        void OnDisable()
        {
            // Deregister for callbacks
            if (iapService != null)
            {
                iapService.RemoveGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseEvent);
                iapService.RemoveGetProductDataResponseListener(GetProductDataResponseEvent);
                iapService.RemovePurchaseResponseListener(PurchaseResponseEvent);
                iapService.RemoveGetUserDataResponseListener(GetUserDataResponseEvent);
            }

        }


        public void RestorePurchases()
        {
        }

        public string GetProductPrice(string ProductName)
        {
            for (int i = 0; i < m_products.Length; i++)
            {
                if (m_products[i].Name == ProductName)
                {
                    if (products.ContainsKey(m_products[i].ProductIdentifier))
                    {
                        return products[m_products[i].ProductIdentifier];
                    }
                }
            }
            return "-.-";
        }

        public bool IsProductPurchasedWithName(string ProductName)
        {
            return false;
        }

        public void BuyProductWithName(string ProductName)
        {
            string productId = GetProductIdentifier(ProductName);
            if (productId != "")
            {
                BuyProduct(productId);
            }
        }

        public string GetProductIdentifier(string ProductName)
        {
            for (int i = 0; i < m_products.Length; i++)
            {
                if (m_products[i].Name == ProductName)
                {
                    return m_products[i].ProductIdentifier;
                }
            }
            return "";
        }

#region API Methods
        public void RequestBillingProducts()
        {
            if (m_products.Length != 0 && !m_productRequestFinished)
            {
                RequestBillingProducts(m_products);
            }
        }
        private void RequestBillingProducts(BillingProduct[] _products)
        {
            // Amazon Init
            iapService = AmazonIapV2Impl.Instance;
            GetProductData();
            GetPurchaseUpdates();
        }

        private void GetProductData()
        {
            if (iapService != null)
            {
                if (debuglog) Debug.Log("SCShop: **** Amazon GetProductData() *****");
                SkusInput request = new SkusInput();
                List<string> list = new List<string>();
                for (int i=0; i < m_products.Length; i++)
                {
                    list.Add(m_products[i].ProductIdentifier);
                }
                if (debuglog)
                {
                    string tmpStr = "SCShop: SKUs: ";
                    for (int i=0; i < list.Count; i++)
                    {
                        tmpStr += ", " + list[i];
                    }
                    Debug.Log(tmpStr);
                }
                request.Skus = list;
                iapService.GetProductData(request);
            }
        }
        private void GetPurchaseUpdates()
        {
            if (iapService != null)
            {
                if (debuglog) Debug.Log("SCShop: **** Amazon GetPurchaseUpdates() *****");
                ResetInput request = new ResetInput();
                request.Reset = true;
                iapService.GetPurchaseUpdates(request);
            }
        }
        private void NotifyFulfillment(string receiptID)
        {
            if (iapService != null)
            {
                if (debuglog) Debug.Log("SCShop: **** Amazon NotifyFulfillment() *****");
                NotifyFulfillmentInput request = new NotifyFulfillmentInput();
                request.ReceiptId = receiptID;
                request.FulfillmentResult = "FULFILLED";

                iapService.NotifyFulfillment(request);
            }
        }


        private void RestoreCompletedTransactions()
        {
            //NPBinding.Billing.RestoreCompletedTransactions();
        }

        private void BuyProduct(string _productIdentifier)
        {
            if (iapService != null)
            {
                SkuInput request = new SkuInput();
                request.Sku = _productIdentifier;
                iapService.Purchase(request);
            }
        }

        private bool IsProductPurchased(string _productIdentifier)
        {
            return false;
        }

#endregion

#region Amazon Events
        private void GetUserDataResponseEvent(GetUserDataResponse args)
        {
            if (debuglog)
            {
                Debug.Log("SCShop: **** Amazon GetUserDataResponseEvent ***** userid: " + args.AmazonUserData.UserId
                              + ", marketplace: " + args.AmazonUserData.Marketplace
                              + ", status: " + args.Status
                              );
            }
        }

        private void GetPurchaseUpdatesResponseEvent(GetPurchaseUpdatesResponse args)
        {
            if (args.Status != "SUCCESSFUL")
            {
                if (debuglog) Debug.Log("SCShop: **** GetPurchaseUpdatesResponseEvent Status: " + args.Status);
                return;
            }
            List<PurchaseReceipt> receipts = args.Receipts;
            for (int i = 0; i < receipts.Count; i++)
            {
                if (debuglog) Debug.Log("SCShop: **** GetPurchaseUpdatesResponseEvent for " + receipts[i].Sku);
                if (OnReceiveTransactionInfoEvent != null)
                {
                    OnReceiveTransactionInfoEvent(receipts[i].Sku, eBillingTransactionState.RESTORED);
                }
                for (int j=0; j < m_products.Length; j++)
                {
                    if (m_products[j].ProductIdentifier == receipts[i].Sku)
                    {
                        if (m_products[j].IsConsumable)
                        {
                            if (debuglog) Debug.Log("SCShop: **** Sending NotifyFulfillment for " + receipts[i].Sku);
                            NotifyFulfillment(receipts[i].ReceiptId);
                            break;
                        }
                    }
                }

            }


            if (debuglog)
            {
                string receiptsStr = "**** GetPurchaseUpdatesResponseEvent Summary:\n";
                for (int i = 0; i < receipts.Count; i++)
                {
                    receiptsStr += "i: " + i.ToString();
                    receiptsStr += ", Sku: " + receipts[i].Sku;
                    receiptsStr += ", ProductType: " + receipts[i].ProductType;
                    receiptsStr += ", PurchaseDate: " + receipts[i].PurchaseDate;
                    receiptsStr += ", CancelDate: " + receipts[i].CancelDate;
                    receiptsStr += ", ReceiptId: " + receipts[i].ReceiptId;
                    receiptsStr += "\n";
                }
                Debug.Log("SCShop: **** Amazon GetPurchaseUpdatesResponseEvent ***** "
                          + ", userid: " + args.AmazonUserData.UserId
                          + ", status: " + args.Status
                          + ", receipts count: " + receipts.Count
                          + ", " + receiptsStr
                          + ", "
                          );
            }
        }

        private void PurchaseResponseEvent(PurchaseResponse args)
        {
            if (args.Status == "ALREADY_PURCHASED")
            {
                if (debuglog) Debug.Log("SCShop: **** ALREADY_PURCHASED " + args.PurchaseReceipt.Sku);
                if (OnReceiveTransactionInfoEvent != null)
                {
                    if (debuglog) Debug.Log("SCShop: Sending OnReceiveTransactionInfoEvent " + args.PurchaseReceipt.Sku + " RESTORED");
                    OnReceiveTransactionInfoEvent(args.PurchaseReceipt.Sku, eBillingTransactionState.RESTORED);
                }
                return;
            }
            if (args.Status != "SUCCESSFUL")
            {
                if (debuglog) Debug.Log("SCShop: **** PurchaseResponseEvent Status: " + args.Status);
                if (OnReceiveTransactionInfoEvent != null)
                {
                    if (debuglog) Debug.Log("SCShop: Sending OnReceiveTransactionInfoEvent FAILED");
                    OnReceiveTransactionInfoEvent("", eBillingTransactionState.FAILED);
                }
                return;
            }
            // Send purchase info
            if (OnReceiveTransactionInfoEvent != null)
            {
                if (debuglog) Debug.Log("SCShop: Sending OnReceiveTransactionInfoEvent " + args.PurchaseReceipt.Sku + " FAILED");
                OnReceiveTransactionInfoEvent(args.PurchaseReceipt.Sku, eBillingTransactionState.PURCHASED);
            }
            // Check if comsume
            for (int i = 0; i < m_products.Length; i++)
            {
                if (m_products[i].ProductIdentifier == args.PurchaseReceipt.Sku)
                {
                    if (m_products[i].IsConsumable)
                    {
                        if (debuglog) Debug.Log("SCShop: **** Sending NotifyFulfillment for " + args.PurchaseReceipt.Sku);
                        NotifyFulfillment(args.PurchaseReceipt.ReceiptId);
                        break;
                    }
                }
            }


            if (debuglog)
            {
                Debug.Log("SCShop: **** Amazon PurchaseResponseEvent Summary ***** userid: " + args.AmazonUserData.UserId
                              + ", marketplace: " + args.AmazonUserData.Marketplace
                              + ", receiptId: " + args.PurchaseReceipt.ReceiptId
                              + ", sku: " + args.PurchaseReceipt.Sku
                              + ", status: " + args.Status
                              );
            }
        }

        private void GetProductDataResponseEvent(GetProductDataResponse args)
        {
            if (args.Status != "SUCCESSFUL")
            {
                if (debuglog) Debug.Log("SCShop: **** GetProductDataResponseEvent Status: " + args.Status);
                return;
            }

            Dictionary<string, ProductData> productDataMap = args.ProductDataMap;
            List<string> unavailableSkus = args.UnavailableSkus;
            // for each item in the productDataMap you can get the following values for a given SKU
            if (debuglog)
            {
                Debug.Log("SCShop: **** Amazon GetProductDataResponseEvent ***** "
                              + ", status: " + args.Status
                              + ", products count: " + productDataMap.Count
                              + ", unavailableSkus: " + unavailableSkus.Count
                              );
            }

            bool updatePrices = false;
            for (int i=0; i < m_products.Length; i++)
            {
                if (productDataMap.ContainsKey(m_products[i].ProductIdentifier))
                {
                    updatePrices = true;
                    if (products.ContainsKey(m_products[i].ProductIdentifier))
                    {
                        products[m_products[i].ProductIdentifier] = productDataMap[m_products[i].ProductIdentifier].Price;
                    }
                    else
                    {
                        products.Add(m_products[i].ProductIdentifier, productDataMap[m_products[i].ProductIdentifier].Price);
                    }
                }
            }
            if (updatePrices && OnProductInfoUpdate != null)
            {
                m_productRequestFinished = true;
                OnProductInfoUpdate();
            }
        }

#endregion

        // End of Amazon
#endregion
// End of SOFTCEN_AMAZON
#endif

        // End of USES_BILLING
#endif
    }
}
