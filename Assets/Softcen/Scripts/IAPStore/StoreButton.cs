using UnityEngine;
using UnityEngine.UI;
using Beebyte.Obfuscator;
using TMPro;

public class StoreButton : MonoBehaviour {

    public Kauppa.ID storeID;
    public TextMeshProUGUI txtPrice;
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtCount;
    public Image imgShop;
    public Button btn;

    private bool m_Initialized = false;

    void OnEnable()
    {
        PlayerData.OnStoreItemChanged += PlayerData_OnStoreItemChanged;
        CheckButton();
    }

    private void PlayerData_OnStoreItemChanged()
    {
        CheckButton();
    }

    void OnDisable()
    {
        PlayerData.OnStoreItemChanged -= PlayerData_OnStoreItemChanged;
    }

    public void SetStoreId(Kauppa.ID id)
    {
        //Debug.Log("StoreButton SetStoreId id: " + id.ToString());
        storeID = id;
        string title, price, desc;
        if (Kauppa.Instance != null)
        {
            Kauppa.Instance.GetProductMetadata(storeID, out title, out price, out desc);
        } else
        {
            title = "-";
            price = "-";
            desc = "";
        }
        txtTitle.SetText (Kauppa.Instance.GetProductTitle(storeID));
        txtPrice.SetText (price);
        int count = Kauppa.Instance.GetGemsCount(storeID);
        if (count > 0)
            txtCount.SetText (count.ToString());
        else
            txtCount.SetText ("");
        m_Initialized = true;
        CheckButton();
    }

    private void CheckButton()
    {
        if (m_Initialized == true)
        {
            if (storeID == Kauppa.ID.INN_APP_AD_FREE)
            {
                if (GameManager.Instance.playerData.AdFree == true)
                {
                    txtPrice.SetText ("OWNED");
                    btn.interactable = false;
                }
                else
                {
                    txtCount.SetText ("");
                    btn.interactable = true;
                }
            }
            else if (storeID == Kauppa.ID.INN_APP_AUTO_TAP)
            {
                if (GameManager.Instance.playerData.AutoTapOwned == true)
                {
                    txtPrice.SetText ("OWNED");
                    btn.interactable = false;
                }
                else
                {
                    txtCount.SetText ("");
                    btn.interactable = true;
                }
            }
            else
            {
                btn.interactable = true;
            }
        }
    }

    [SkipRename]
    public void BuyProduct()
    {
        AudioManager.Instance.PlayButtonClick();
        Kauppa.Instance.BuyProductID(storeID);
    }
	
}
