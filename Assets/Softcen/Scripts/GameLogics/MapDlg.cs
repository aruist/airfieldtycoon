using UnityEngine;
using UnityEngine.UI;

public class MapDlg : MonoBehaviour {
    public GameObject goNewChapter;
    public Text txtChapterCoinPrice;
    public Text txtChapterDiamondPrice;
    public Button btnBuyWithCoins;

    private float m_coinPrice;
    private int m_diamondPrice;

    void OnEnable()
    {
        PlayerData.OnMoneyChanged += PlayerData_OnMoneyChanged;
        /*
        if (HomeManager.Instance != null && HomeManager.Instance.IsPhaseCompleted())
        {
            goNewChapter.SetActive(true);
        }
        else
        {
            goNewChapter.SetActive(false);
        }*/
    }
    void OnDisable()
    {
        PlayerData.OnMoneyChanged -= PlayerData_OnMoneyChanged;
    }

    public void SetPrices(float coinPrice, int diamondPrice)
    {
        m_coinPrice = coinPrice;
        m_diamondPrice = diamondPrice;
        txtChapterCoinPrice.text = NumToStr.GetNumStr(m_coinPrice);
        txtChapterDiamondPrice.text = m_diamondPrice.ToString();
    }

    private void PlayerData_OnMoneyChanged(double coins)
    {
        if (goNewChapter.activeSelf)
        {
            if (m_coinPrice <= coins)
            {
                btnBuyWithCoins.interactable = true;
            }
            else
            {
                btnBuyWithCoins.interactable = false;
            }
#if SOFTCEN_DEBUG
            if (GameManager.Instance.dev_SkipMoney)
                btnBuyWithCoins.interactable = true;
#endif

        }
    }


}
