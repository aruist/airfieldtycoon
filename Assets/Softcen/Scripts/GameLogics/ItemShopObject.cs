using UnityEngine;
using UnityEngine.UI;
using Beebyte.Obfuscator;
using TMPro;

public class ItemShopObject : MonoBehaviour {

    public TextMeshProUGUI txtTitle;
    public Image itemImage;
    public GameObject goLocked;
    public GameObject goActive;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI txtCoinPrice;
    public TextMeshProUGUI txtDiamondPrice;
    public TextMeshProUGUI txtOwnedCount;
    public TextMeshProUGUI txtSubTitle;
    public Button coinButton;
    public Button diamondButton;
    public Object itemObj;
    public GameObject goPrestigeIcon;
    public bool checkStatusOnEnable = true;

    private bool m_Initialized = false;
    private bool m_EventSubscribed = false;
    private double m_CoinsNeeded;

    private int m_currentCount;
    private int m_maxCount;

    void Start()
    {
        m_Initialized = true;
        CheckActiveStatus();
        //UpdateSubTitle();

    }
    void OnEnable()
    {
        if (checkStatusOnEnable)
            CheckActiveStatus();
        //UpdateSubTitle();
    }
    void OnDisable()
    {
        UnSubscribeEvents();
    }

    private void UnSubscribeEvents()
    {
        if (m_EventSubscribed)
        {
            m_EventSubscribed = false;
            PlayerData.OnMoneyChanged -= PlayerData_OnMoneyChanged;
            PlayerData.OnBonusesChanged -= PlayerData_OnBonusesChanged;
            PlayerData.OnLevelChanged -= PlayerData_OnLevelChanged;
        }
    }

    private void SubscribeEvents()
    {
        if (!m_EventSubscribed)
        {
            m_EventSubscribed = true;
            PlayerData.OnMoneyChanged += PlayerData_OnMoneyChanged;
            PlayerData.OnBonusesChanged += PlayerData_OnBonusesChanged;
            PlayerData.OnLevelChanged += PlayerData_OnLevelChanged;
        }
    }
    private void PlayerData_OnBonusesChanged()
    {
        CheckActiveStatus();
    }

    private void PlayerData_OnLevelChanged()
    {
        CheckActiveStatus();
    }

    private void PlayerData_OnMoneyChanged(double newCoins)
    {
        if (newCoins >= m_CoinsNeeded)
        {
            if (!coinButton.interactable)
                coinButton.interactable = true;
        }
        else
        {
            if (coinButton.interactable)
                coinButton.interactable = false;
        }
#if SOFTCEN_DEBUG
        if (GameManager.Instance.dev_SkipMoney && !coinButton.interactable)
            coinButton.interactable = true;
#endif

    }

    private int m_setCurrentCount = -1;
    private int m_setMaxCount = -1;
    private bool m_setOwned = false;
    private void UpdateOwnedCountText(bool isLevel)
    {
        if (!isLevel)
        {
            if (m_currentCount > 0)
            {
                if (m_setCurrentCount != m_currentCount || m_setMaxCount != m_maxCount)
                {
                    m_setCurrentCount = m_currentCount;
                    m_setMaxCount = m_maxCount;
                    txtOwnedCount.text = "Level: " + m_currentCount.ToString() + "/" + m_maxCount.ToString();
                }
            }
            else
            {
                txtOwnedCount.text = "Level: -";
                m_setCurrentCount = -1;
                m_setMaxCount = -1;
            }
        }
        else
        {
            m_setCurrentCount = -1;
            m_setMaxCount = -1;
            if (m_currentCount >= m_maxCount)
            {
                if (!m_setOwned)
                {
                    m_setOwned = true;
                    txtOwnedCount.text = "Owned";
                }
            }
            else
            {
                m_setOwned = false;
                txtOwnedCount.text = "";
            }
        }
    }

    private void UpdateOwnedCount()
    {
        if (itemObj is UpgradeItemTap)
        {
            UpgradeItemTap obj = (UpgradeItemTap)itemObj;
            m_currentCount = obj.ownedCount;
            m_maxCount = obj.maxCount;
            UpdateOwnedCountText(false);
        }
        else if (itemObj is UpgradeItemIdle)
        {
            UpgradeItemIdle obj = (UpgradeItemIdle)itemObj;
            m_currentCount = obj.ownedCount;
            m_maxCount = obj.maxCount;
            UpdateOwnedCountText(false);
        }
        else if (itemObj is UpgradeItemLevel)
        {
            m_maxCount = 1;
            m_currentCount = 0;
            UpgradeItemLevel uil = (UpgradeItemLevel)itemObj;
            if (GameManager.Instance.playerData.CurrentChapter == (int)uil.chapterId)
            {
                if (uil.ActivePhase <= GameManager.Instance.playerData.CurrentPhase)
                {
                    m_currentCount = 1;
                }
            }
            else if ((int)uil.chapterId < GameManager.Instance.playerData.CurrentChapter)
            {
                m_currentCount = 1;
            }
            UpdateOwnedCountText(true);
        }
    }




    private bool LevelItemActive()
    {
        bool retVal = false;
        if (itemObj is UpgradeItemLevel)
        {
            UpgradeItemLevel uil = (UpgradeItemLevel)itemObj;
            if (GameManager.Instance.playerData.CurrentChapter == (int)uil.chapterId)
            {
                int activeOnPhase = uil.ActivePhase - 1; 
                if (activeOnPhase <= GameManager.Instance.playerData.CurrentPhase )
                {
                    retVal = true;
                }
            }
            else if ((int)uil.chapterId < GameManager.Instance.playerData.CurrentChapter)
            {
                retVal = true;
            }
        }
        return retVal;
    }

    private bool TapItemActive()
    {
        if (itemObj is UpgradeItemTap)
        {
            UpgradeItemTap uit = (UpgradeItemTap)itemObj;
            if (GameManager.Instance.playerData.CurrentChapter > (int)uit.chapterId)
            {
                return true;
            }
            else if (GameManager.Instance.playerData.CurrentChapter == (int)uit.chapterId)
            {
                if (uit.ActivePhase <= GameManager.Instance.playerData.CurrentPhase)
                {
                    return true;
                }
            }
        }

        return false;
    }
    private bool IdleItemActice()
    {
        if (itemObj is UpgradeItemIdle)
        {
            UpgradeItemIdle uii = (UpgradeItemIdle)itemObj;
            if (GameManager.Instance.playerData.CurrentChapter > (int)uii.chapterId)
            {
                return true;
            }
            else if (GameManager.Instance.playerData.CurrentChapter == (int)uii.chapterId)
            {
                if (uii.ActivePhase <= GameManager.Instance.playerData.CurrentPhase)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void CheckActiveStatus()
    {
        //Debug.Log("CheckActiveStatus " + gameObject.name);
        if (GameManager.Instance != null && m_Initialized == true)
        {
            if (itemObj is UpgradeItemTap)
            {
                UpgradeItemTap uit = (UpgradeItemTap)itemObj;
                if (BonusManager.Instance.IsTapPrestiged((int)uit.Id))
                {
                    goPrestigeIcon.SetActive(true);
                }
                else
                {
                    goPrestigeIcon.SetActive(false);
                }
            }
            else if (itemObj is UpgradeItemIdle)
            {
                UpgradeItemIdle uii = (UpgradeItemIdle)itemObj;
                if (BonusManager.Instance.IsIdlePrestiged((int)uii.Id))
                {
                    goPrestigeIcon.SetActive(true);
                }
                else
                {
                    goPrestigeIcon.SetActive(false);
                }
            }
            else
            {
                goPrestigeIcon.SetActive(false);
                txtOwnedCount.gameObject.SetActive(true);
            }

            UpdateOwnedCount();
            bool isActive = LevelItemActive();
            if (!isActive) isActive = TapItemActive();
            if (!isActive) isActive = IdleItemActice();

            if (isActive)
            {
                if (canvasGroup.alpha != 1)
                    canvasGroup.alpha = 1f;
                if (goLocked.activeSelf)
                {
                    goLocked.SetActive(false);
                }
                txtOwnedCount.gameObject.SetActive(true);
                if (m_currentCount >= m_maxCount)
                {
                    UnSubscribeEvents();
                    if (goActive.activeSelf)
                        goActive.SetActive(false);
                    if (txtSubTitle.gameObject.activeSelf)
                        txtSubTitle.gameObject.SetActive(false);
                    if (canvasGroup.interactable)
                        canvasGroup.interactable = false;
                }
                else
                {

                    SubscribeEvents();
                    UpdatePrices();
                    UpdateSubTitle();
                    if (!goActive.activeSelf)
                        goActive.SetActive(true);
                    if (!txtSubTitle.gameObject.activeSelf)
                        txtSubTitle.gameObject.SetActive(true);
                    if (!canvasGroup.interactable)
                        canvasGroup.interactable = true;
                }
            }
            else
            {
                UnSubscribeEvents();
                if (!goLocked.activeSelf)
                {
                    goLocked.SetActive(true);
                }
                txtOwnedCount.gameObject.SetActive(false);
                if (txtSubTitle.gameObject.activeSelf)
                    txtSubTitle.gameObject.SetActive(false);
                if (goActive.activeSelf)
                    goActive.SetActive(false);
                if (canvasGroup.alpha != 1)
                    canvasGroup.alpha = 1f;
                if (canvasGroup.interactable)
                    canvasGroup.interactable = false;
            }
        }
    }

    private void UpdateSubTitle()
    {
        if (itemObj is UpgradeItemLevel)
        {
            UpgradeItemLevel uil = (UpgradeItemLevel)itemObj;
            int level = uil.ItemId + 2;
            txtSubTitle.text = "Upgrade to level " + level.ToString();
        }
        else if (itemObj is UpgradeItemIdle)
        {
            UpgradeItemIdle uii = (UpgradeItemIdle)itemObj;
            double bonus = BonusManager.Instance.GetBonusMultiply(BonusTypes.Type.Idle, uii.ItemId, 1);
            txtSubTitle.text = "Increase Idle Bonus +" + NumToStr.GetNumStr(bonus) + "/ sec";
        }
        else if (itemObj is UpgradeItemTap)
        {
            UpgradeItemTap uit = (UpgradeItemTap)itemObj;
            double tapBonusCurrent = BonusManager.Instance.GetTapBonus(uit.ItemId, uit.ownedCount);
            double tapBonusNext = BonusManager.Instance.GetTapBonus(uit.ItemId, uit.ownedCount+1);
            //float bonus = BonusManager.Instance.GetBonusMultiply(BonusTypes.Type.Tap, uit.ItemId, uit.ownedCount+1);
            double bonus = tapBonusNext - tapBonusCurrent;
            txtSubTitle.text = "Increase Tap Bonus +" + NumToStr.GetNumStr(bonus) + "/ sec";
        }
    }

    private void UpdatePrices()
    {
        double currentCoinPrice = -1;
        int currentDiamondPrice = -1;
        if (itemObj is UpgradeItemLevel)
        {
            UpgradeItemLevel uil = (UpgradeItemLevel)itemObj;
            currentCoinPrice = uil.GetCoinPrice();
            currentDiamondPrice = uil.GetDiamondPrice();
            /*Debug.Log("LevelItem Id:" + uil.ItemId
                + ", Coins: " + currentCoinPrice.ToString()
                + ", Diamonds: " + currentDiamondPrice.ToString()
                );*/
        }
        else if (itemObj is UpgradeItemIdle)
        {
            UpgradeItemIdle obj = (UpgradeItemIdle)itemObj;
            currentCoinPrice = obj.GetCoinPrice();
            currentDiamondPrice = obj.GetDiamondPrice();
        }
        else if (itemObj is UpgradeItemTap)
        {
            UpgradeItemTap obj = (UpgradeItemTap)itemObj;
            currentCoinPrice = obj.GetCoinPrice();
            currentDiamondPrice = obj.GetDiamondPrice();
        }
        m_CoinsNeeded = currentCoinPrice;
        txtCoinPrice.text = NumToStr.GetNumStr(currentCoinPrice);
        txtDiamondPrice.text = currentDiamondPrice.ToString();
        PlayerData_OnMoneyChanged(GameManager.Instance.playerData.Money);
        //UpdateSubTitle();

    }

    [SkipRename]
    public void BuyCoinButton()
    {
        AudioManager.Instance.PlayButtonClick();
        if (itemObj is UpgradeItemLevel)
        {
            UpgradeItemLevel uil = (UpgradeItemLevel)itemObj;
            uil.BuyItemWithCoins();
        }
        if (itemObj is UpgradeItemIdle)
        {
            UpgradeItemIdle obj = (UpgradeItemIdle)itemObj;
            obj.BuyItemWithCoins();
        }
        if (itemObj is UpgradeItemTap)
        {
            UpgradeItemTap obj = (UpgradeItemTap)itemObj;
            obj.BuyItemWithCoins();
        }

    }
    [SkipRename]
    public void BuyDiamondButton()
    {
        AudioManager.Instance.PlayButtonClick();
        if (itemObj is UpgradeItemLevel)
        {
            UpgradeItemLevel uil = (UpgradeItemLevel)itemObj;
            if (uil.GetDiamondPrice() <= GameManager.Instance.playerData.Diamonds)
            {
                uil.BuyItemWithDiamonds();
                return;
            }
        }
        if (itemObj is UpgradeItemIdle)
        {
            UpgradeItemIdle obj = (UpgradeItemIdle)itemObj;
            if (obj.GetDiamondPrice() <= GameManager.Instance.playerData.Diamonds)
            {
                obj.BuyItemWithDiamonds();
                return;
            }
        }
        if (itemObj is UpgradeItemTap)
        {
            UpgradeItemTap obj = (UpgradeItemTap)itemObj;
            if (obj.GetDiamondPrice() <= GameManager.Instance.playerData.Diamonds)
            {
                obj.BuyItemWithDiamonds();
                return;
            }
        }

        // No diamonds
        HomeManager.Instance.OpenNoDiamondsDlg();

    }

}
