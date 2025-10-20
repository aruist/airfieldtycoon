using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Beebyte.Obfuscator;

public class MainMenuButton : MonoBehaviour {
    public LoadingPanel loadPanel;
    //public MenuHide[] menuHideObjects;
    //public GameObject goMap;

    public CanvasGroup ownCanvasGroup;
    public Sprite[] sprButton;
    public Image imgButton;
    public CommonDialog cdIdleUpgrades;
    public CommonDialog cdMainMenu;
    public CommonDialog cdMissionDlg;
    public CommonDialog cdTapUpgrades;
    public CommonDialog cdLevelUp;
    public CommonDialog cdSettingsDlg;
    public CommonDialog cdCreditsDlg;
    public CommonDialog cdShopDlg;
    public CommonDialog cdNoDiamondsDlg;
    public CommonDialog cdAutoTapDlg;
    public CommonDialog cdRateGameDlg;
    public CommonDialog cdLikeGameDlg;
    public CommonDialog cdPrestigeDlg;
    public CommonDialog cdGPSignIn;
    public CommonDialog cdRewardEventDlg;
    public CommonDialog cdRewardReceivedDlg;
    public CommonDialog cdFreeChestDlg;
    public DialogManager dialogManager;

    public AudioClip acShortButton;


    public Image idleExpImage;
    public GameObject goIdleParticle;
    public GameObject goIdleAvailable;

    public Image tapExpImage;
    public GameObject goTapParticle;
    public GameObject goTapAvailable;

    public Image lvlExpImage;
    public GameObject goLvlParticle;
    public GameObject goLvlAvailable;

    public double minIdlePrice;
    public double minTapPrice;
    public double minLevelPrice;

    private bool idleParticelPlayed;
    private bool tapParticelPlayed;
    private bool lvlParticelPlayed;

    private GameObject goIdleExp;
    private GameObject goTapExp;
    private GameObject goLevelExp;

    void Start()
    {
        goIdleExp = idleExpImage.transform.parent.gameObject;
        goTapExp = tapExpImage.transform.parent.gameObject;
        goLevelExp = lvlExpImage.transform.parent.gameObject;
        idleParticelPlayed = false;
        tapParticelPlayed = false;
        lvlParticelPlayed = false;

        PlayerData_OnIdleUpgrade();
        PlayerData_OnTapUpgrade();
        PlayerData_OnLevelChanged();
        CheckShortButtons();
    }

    void OnDisable()
    {
#if SOFTCEN_DEBUG
        DevOptions.OnReset -= DevOptions_OnReset;
#endif
        PlayerData.OnIdleUpgrade -= PlayerData_OnIdleUpgrade;
        PlayerData.OnMoneyChanged -= PlayerData_OnMoneyChanged;
        PlayerData.OnLevelChanged -= PlayerData_OnLevelChanged;
        PlayerData.OnTapUpgrade -= PlayerData_OnTapUpgrade;
    }

    void OnEnable()
    {
#if SOFTCEN_DEBUG
        DevOptions.OnReset += DevOptions_OnReset;
#endif
        PlayerData.OnIdleUpgrade += PlayerData_OnIdleUpgrade;
        PlayerData.OnMoneyChanged += PlayerData_OnMoneyChanged;
        PlayerData.OnLevelChanged += PlayerData_OnLevelChanged;
        PlayerData.OnTapUpgrade += PlayerData_OnTapUpgrade;
    }

    private void PlayerData_OnMoneyChanged(double obj)
    {
        CheckShortButtons();
    }

#if SOFTCEN_DEBUG
    private void DevOptions_OnReset()
    {
        CloseAll();
    }
#endif
    private void PlayerData_OnTapUpgrade()
    {
        tapParticelPlayed = false;
        minTapPrice = BonusManager.Instance.GetLowestTapPrice();
        CheckShortButtons();
    }

    private void PlayerData_OnLevelChanged()
    {
        lvlParticelPlayed = false;
        minLevelPrice = BonusManager.Instance.GetLowestLevelPrice();
        PlayerData_OnTapUpgrade();
        PlayerData_OnIdleUpgrade();
        CheckShortButtons();
    }

    private void PlayerData_OnIdleUpgrade()
    {
        idleParticelPlayed = false;
        minIdlePrice = BonusManager.Instance.GetLowestIdlePrice();
        CheckShortButtons();
    }

    private void CheckShortButtons()
    {
        double coins = GameManager.Instance.playerData.Money;
        if (minIdlePrice < 0)
        {
            if (goIdleExp.activeSelf)
                goIdleExp.SetActive(false);
            idleExpImage.fillAmount = 0f;
            if (goIdleAvailable.activeSelf)
                goIdleAvailable.SetActive(false);
        }
        else if (minIdlePrice <= coins && !idleParticelPlayed)
        {
            if (!goIdleExp.activeSelf)
                goIdleExp.SetActive(true);

            idleExpImage.fillAmount = 1f;
            idleParticelPlayed = true;
            goIdleParticle.SetActive(true);
            goIdleAvailable.SetActive(true);
            AudioManager.Instance.PlayClip(acShortButton);
        }
        else if (minIdlePrice > coins)
        {
            if (!goIdleExp.activeSelf)
                goIdleExp.SetActive(true);
            idleParticelPlayed = false;
            if (goIdleAvailable.activeSelf)
                goIdleAvailable.SetActive(false);
            idleExpImage.fillAmount = (minIdlePrice == 0) ? 0 : (float)(coins / minIdlePrice);
        }

        if (minTapPrice < 0)
        {
            if (goTapExp.activeSelf)
                goTapExp.SetActive(false);
            tapExpImage.fillAmount = 0f;
            if (goTapAvailable.activeSelf)
                goTapAvailable.SetActive(false);
        }
        else if (minTapPrice <= coins && !tapParticelPlayed)
        {
            if (!goTapExp.activeSelf)
                goTapExp.SetActive(true);
            tapExpImage.fillAmount = 1f;
            tapParticelPlayed = true;
            goTapParticle.SetActive(true);
            goTapAvailable.SetActive(true);
            AudioManager.Instance.PlayClip(acShortButton);
        }
        else if (minTapPrice > coins)
        {
            if (!goTapExp.activeSelf)
                goTapExp.SetActive(true);
            tapParticelPlayed = false;
            if (goTapAvailable.activeSelf)
                goTapAvailable.SetActive(false);
            tapExpImage.fillAmount = (minTapPrice == 0) ? 0 : (float)(coins / minTapPrice);
        }

        if (minLevelPrice < 0)
        {
            if (goLevelExp.activeSelf)
                goLevelExp.SetActive(false);
            lvlExpImage.fillAmount = 0f;
            if (goLvlAvailable.activeSelf)
                goLvlAvailable.SetActive(false);
        }
        else if (minLevelPrice <= coins && !lvlParticelPlayed)
        {
            if (!goLevelExp.activeSelf)
                goLevelExp.SetActive(true);
            lvlExpImage.fillAmount = 1f;
            lvlParticelPlayed = true;
            goLvlParticle.SetActive(true);
            goLvlAvailable.SetActive(true);
            AudioManager.Instance.PlayClip(acShortButton);
        }
        else if (minLevelPrice > coins)
        {
            if (!goLevelExp.activeSelf)
                goLevelExp.SetActive(true);
            lvlParticelPlayed = false;
            if (goLvlAvailable.activeSelf)
                goLvlAvailable.SetActive(false);
            lvlExpImage.fillAmount = (minLevelPrice == 0) ? 0 : (float)(coins / minLevelPrice);
        }

    }
    [SkipRename]
    private void ClosePrestige()
    {
        CheckSprite();
        cdPrestigeDlg.Button_Close();
    }
    [SkipRename]
    private void CloseGPSignIn()
    {
        cdGPSignIn.Button_Close();
    }
    [SkipRename]
    private void CloseShop()
    {
        CheckSprite();
        cdShopDlg.Button_Close();
    }
    [SkipRename]
    private void CloseNoDiamonds()
    {
        CheckSprite();
        cdNoDiamondsDlg.Button_Close();
    }
    [SkipRename]
    private void CloseLikeGame()
    {
        Debug.LogWarning("CloseLikeGame");
        cdLikeGameDlg.Button_Close();
        CheckSprite();
    }
    [SkipRename]
    private void CloseRateGame()
    {
        cdRateGameDlg.Button_Close();
        CheckSprite();
    }
    [SkipRename]
    private void CloseAutoTap()
    {
        cdAutoTapDlg.Button_Close();
        CheckSprite();
    }
    [SkipRename]
    private void CloseRewardReceived()
    {
        cdRewardReceivedDlg.Button_Close();
        CheckSprite();
    }
    [SkipRename]
    private void CloseIdleUpgrades()
    {
        CheckSprite();
        cdIdleUpgrades.Button_Close();
        HomeManager.Instance.ShowInterstitial();
    }
    [SkipRename]
    private void CloseLevelUp()
    {
        CheckSprite();
        cdLevelUp.Button_Close();
    }
    [SkipRename]
    private void CloseCredits()
    {
        CheckSprite();
        cdCreditsDlg.Button_Close();
    }
    [SkipRename]
    private void CloseSettings()
    {
        CheckSprite();
        cdSettingsDlg.Button_Close();
    }
    [SkipRename]
    private void CloseTapUpgrades()
    {
        CheckSprite();
        cdTapUpgrades.Button_Close();
        HomeManager.Instance.ShowInterstitial();
    }
    [SkipRename]
    private void CloseCdMissionDlg()
    {
        CheckSprite();
        cdMissionDlg.Button_Close();
    }
    [SkipRename]
    private void CloseRewardEventDlg()
    {
        CheckSprite();
        cdRewardEventDlg.Button_Close();
    }
    [SkipRename]
    private void CloseMainMenuDlg()
    {
        imgButton.sprite = sprButton[0];
        cdMainMenu.Button_Close();
    }


    [SkipRename]
    public void MainMenuPressed()
    {
        AudioManager.Instance.PlayButtonClick();
        if (cdPrestigeDlg.gameObject.activeSelf)
        {
            ClosePrestige();
        }
        else if (cdGPSignIn.gameObject.activeSelf)
        {
            CloseGPSignIn();
        }
        else if (cdShopDlg.gameObject.activeSelf)
        {
            CloseShop();
        }
        else if (cdNoDiamondsDlg.gameObject.activeSelf)
        {
            CloseNoDiamonds();
        }
        else if (cdLikeGameDlg.gameObject.activeSelf)
        {
            CloseLikeGame();
        }
        else if (cdRateGameDlg.gameObject.activeSelf)
        {
            CloseRateGame();
        }
        else if (cdAutoTapDlg.gameObject.activeSelf)
        {
            CloseAutoTap();
        }
        else if (cdRewardReceivedDlg.gameObject.activeSelf)
        {
            CloseRewardReceived();
        }
        else if (cdIdleUpgrades.gameObject.activeSelf)
        {
            CloseIdleUpgrades();
        }
        else if (cdLevelUp.gameObject.activeSelf)
        {
            CloseLevelUp();
        }
        else if (cdCreditsDlg.gameObject.activeSelf)
        {
            CloseCredits();
        }
        else if (cdSettingsDlg.gameObject.activeSelf)
        {
            CloseSettings();
        }
        else if (cdTapUpgrades.gameObject.activeSelf)
        {
            CloseTapUpgrades();
        }
        else if (cdMissionDlg.gameObject.activeSelf)
        {
            CloseCdMissionDlg();
        }
        else if (cdRewardEventDlg.gameObject.activeSelf)
        {
            CloseRewardEventDlg();
        }
        else if (cdMainMenu.gameObject.activeSelf)
        {
            CloseMainMenuDlg();
        }
        else
        {
            imgButton.sprite = sprButton[1];
            dialogManager.OpenDialog("MainMenuDlg");
        }
    }

    private void CheckSprite()
    {
        if (cdMainMenu.gameObject.activeSelf)
            imgButton.sprite = sprButton[1];
        else
            imgButton.sprite = sprButton[0];
    }

    [SkipRename]
    public void OpenIdleUpgrades()
    {
        imgButton.sprite = sprButton[1];
        AudioManager.Instance.PlayButtonClick();
        dialogManager.OpenDialog("IdleUpgradeDlg");
    }

    [SkipRename]
    public void OpenLevelUp()
    {
        imgButton.sprite = sprButton[1];
        AudioManager.Instance.PlayButtonClick();
        dialogManager.OpenDialog("LevelUpDlg");
    }

    [SkipRename]
    public void OpenTapUpgrades()
    {
        imgButton.sprite = sprButton[1];
        AudioManager.Instance.PlayButtonClick();
        dialogManager.OpenDialog("TapUpgradeDlg");
    }

    [SkipRename]
    public void OpenCredits()
    {
        SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Open Credits", "Open Credits", 0);
        imgButton.sprite = sprButton[1];
        AudioManager.Instance.PlayButtonClick();
        dialogManager.OpenDialog("CreditsDlg");
    }

    [SkipRename]
    public void OpenPrestige()
    {
        AudioManager.Instance.PlayButtonClick();
        dialogManager.OpenDialog("PrestigeDlg");
    }

    [SkipRename]
    public void OpenMap()
    {
        AudioManager.Instance.PlayButtonClick();
        GameManager gmi = GameManager.Instance;

        // TODO:
        gmi.NextChapterCoinPrice = 0;
        gmi.NextChapterDiamondPrice = 0;
        gmi.IsPhaceCompleted = false;

        loadPanel.m_scenename = GameConsts.Skenes.Map;
        loadPanel.gameObject.SetActive(true);

        //CloseAll();
        //imgButton.sprite = sprButton[1];
        //HomeManager.Instance.SetMapView(true);
        //dialogManager.OpenDialog("MapDlg");
    }

    [SkipRename]
    public void CloseMap()
    {
        AudioManager.Instance.PlayButtonClick();
        CloseAll();
    }

    [SkipRename]
    public void OpenSettings()
    {
        imgButton.sprite = sprButton[1];
        AudioManager.Instance.PlayButtonClick();
        dialogManager.OpenDialog("SettingsDlg");
    }

    [SkipRename]
    public void OpenShop()
    {
        CloseAll();
        imgButton.sprite = sprButton[1];
        AudioManager.Instance.PlayButtonClick();
        dialogManager.OpenDialog("ShopDlg");
    }

    [SkipRename]
    public void OpenMissionDlg()
    {
        imgButton.sprite = sprButton[1];
        AudioManager.Instance.PlayButtonClick();
        MissionManager.Instance.SetupMissionDlg();
        dialogManager.OpenDialog("MissionDlg");
    }

    [SkipRename]
    public void CloseMissionDlg()
    {
        AudioManager.Instance.PlayButtonClick();
        cdMissionDlg.Button_Close();
        if (cdMainMenu.gameObject.activeSelf)
            imgButton.sprite = sprButton[1];
        else
            imgButton.sprite = sprButton[0];

    }

    public void CloseAll()
    {
        if (cdPrestigeDlg.gameObject.activeSelf)
        {
            cdPrestigeDlg.Button_Close();
        }
        if (cdGPSignIn.gameObject.activeSelf)
        {
            cdGPSignIn.Button_Close();
        }
        if (cdRateGameDlg.gameObject.activeSelf)
        {
            cdRateGameDlg.Button_Close();
        }
        if (cdLikeGameDlg.gameObject.activeSelf)
        {
            cdLikeGameDlg.Button_Close();
        }
        if (cdAutoTapDlg.gameObject.activeSelf)
        {
            cdAutoTapDlg.Button_Close();
        }
        if (cdRewardReceivedDlg.gameObject.activeSelf)
        {
            cdRewardReceivedDlg.Button_Close();
        }
        if (cdNoDiamondsDlg.gameObject.activeSelf)
        {
            cdNoDiamondsDlg.Button_Close();
        }
        if (cdSettingsDlg.gameObject.activeSelf)
        {
            cdSettingsDlg.Button_Close();
        }
        if (cdIdleUpgrades.gameObject.activeSelf)
        {
            cdIdleUpgrades.Button_Close();
        }
        if (cdMissionDlg.gameObject.activeSelf)
        {
            cdMissionDlg.Button_Close();
        }
        if (cdTapUpgrades.gameObject.activeSelf)
        {
            cdTapUpgrades.Button_Close();
        }
        if (cdCreditsDlg.gameObject.activeSelf)
        {
            cdCreditsDlg.Button_Close();
        }
        if (cdShopDlg.gameObject.activeSelf)
        {
            cdShopDlg.Button_Close();
        }
        if (cdLevelUp.gameObject.activeSelf)
        {
            cdLevelUp.Button_Close();
        }
        if (cdMainMenu.gameObject.activeSelf)
        {
            cdMainMenu.Button_Close();
        }
        if (cdRewardEventDlg.gameObject.activeSelf)
        {
            cdRewardEventDlg.Button_Close();
        }
        imgButton.sprite = sprButton[0];

    }

    public void CheckMainMenuButtonImage()
    {
        int index = 0;
        if (( cdSettingsDlg != null && cdSettingsDlg.gameObject.activeSelf )
            || ( cdIdleUpgrades != null && cdIdleUpgrades.gameObject.activeSelf )
            || ( cdMissionDlg != null && cdMissionDlg.gameObject.activeSelf )
            || (cdTapUpgrades != null && cdTapUpgrades.gameObject.activeSelf )
            || (cdCreditsDlg != null && cdCreditsDlg.gameObject.activeSelf )
            || (cdShopDlg != null && cdShopDlg.gameObject.activeSelf)
            || (cdMainMenu != null && cdMainMenu.gameObject.activeSelf)
            || (cdLevelUp != null && cdLevelUp.gameObject.activeSelf)
            || (cdAutoTapDlg != null && cdAutoTapDlg.gameObject.activeSelf)
            || (cdNoDiamondsDlg != null && cdNoDiamondsDlg.gameObject.activeSelf)
            || (cdLikeGameDlg != null && cdLikeGameDlg.gameObject.activeSelf)
            || (cdRateGameDlg != null && cdRateGameDlg.gameObject.activeSelf)
            || (cdPrestigeDlg != null && cdPrestigeDlg.gameObject.activeSelf)
            || (cdGPSignIn != null && cdGPSignIn.gameObject.activeSelf)
            || (cdRewardEventDlg != null && cdRewardEventDlg.gameObject.activeSelf)
            || (cdRewardReceivedDlg != null && cdRewardReceivedDlg.gameObject.activeSelf)
            )
        {
            index = 1;
        }
        imgButton.sprite = sprButton[index];
    }

    public void SetMainMenuButtonActive(bool state)
    {
        ownCanvasGroup.alpha = state ? 1f : 0f;
        ownCanvasGroup.interactable = state;
        ownCanvasGroup.blocksRaycasts = state;
    }
    /*
    public void SetMapView(bool hide)
    {
        if (hide)
        {
            m_mapView = true;
            HomeManager.Instance.HideGameView(true);
            goMap.SetActive(true);
            for (int i=0; i < menuHideObjects.Length; i++)
            {
                menuHideObjects[i].stateWhenHide = menuHideObjects[i].gameObject.activeSelf;
                if (menuHideObjects[i].gameObject.activeSelf)
                    menuHideObjects[i].gameObject.SetActive(false);
            }
        }
        else
        {
            m_mapView = false;
            goMap.SetActive(false);
            HomeManager.Instance.HideGameView(false);
            for (int i = 0; i < menuHideObjects.Length; i++)
            {
                if (menuHideObjects[i].stateWhenHide)
                {
                    menuHideObjects[i].gameObject.SetActive(true);
                }
            }

        }
    }*/

    public void TestMapButton()
    {
        int count = SceneManager.sceneCount;
#if SOFTCEN_DEBUG
        Debug.Log("<color=yellow>SceneCount: " + count + "</color>");
#endif
        /*
        Scene map = SceneManager.GetSceneByName(GameConsts.Skenes.Map);
        if (map != null)
        {
            if (map.isLoaded)
            {
                Debug.Log("<color=yellow>SetActiveScene</color>");
                SceneManager.SetActiveScene(map);
                return;
            }
        }*/
#if SOFTCEN_DEBUG
        Debug.Log("<color=yellow>LoadScene</color>");
#endif
        loadPanel.m_scenename = GameConsts.Skenes.Map;
        loadPanel.gameObject.SetActive(true);
        //SceneManager.LoadScene(GameConsts.Skenes.Map);
    }

    public void FinishPrestige()
    {
        OpenMap();
    }
}
