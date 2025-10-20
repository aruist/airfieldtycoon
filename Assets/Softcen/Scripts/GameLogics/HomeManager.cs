using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
#if SOFTCEN_DEBUG
using UnityEngine.SceneManagement;
#endif
using SoftcenSDK;
using Beebyte.Obfuscator;
using VoxelBusters.EssentialKit;
using System.Collections.Generic;

[Rename("xxx96")]
public class HomeManager : MonoBehaviour
{
    public Canvas myCanvas;
    public EventSystem myEventSystem;
    public KauppaProgress kauppaProgress;
    public KauppaStatusDlg kauppaStatusDlg;

    public GameObject goAutotapEnabled;
    public GameObject goDisableBannerAdsEnabled;

    public GameObject goPrestigeIcon;
    public GameObject goPrestigeButton;

    public MainMenuButton mainMenuButton;
    public GameObject goGameOnButtons;
    public MenuHide[] menuHideObjects;

    //public WorldMapView worldMapView;
    //public MapDlg mapDlg;
    public LevelResources lvlResources;
    //public MainMenuManager m_MainMenuManager;
    //public AudioMixerSnapshot gameOnSnapshot;
    //public float snapshottimeScale = 2f;

    public GameObject goSponsorPanel;
    public ClickerControl m_ClickerControl;
    public DialogManager m_DialogManager;
    public GraphicRaycaster m_MainCanvasGraphicRaycaster;


    public static HomeManager Instance;

    //public InstantBoost m_InstantBoost;
    public RewardEvent m_RewardReceivedDlg;
    public RewardEvent m_RewardEvent = null;
    public RewardArea m_RewardArea;
    public EventIcon m_EventIcon_IdleBoost;
    public EventIcon m_EventIcon_TapBoost;

    //public LuckyDrawEvent m_LuckyDrawEvent;
    //public RewardArea m_LuckyDrawArea;

    //public MenuMusic m_MenuMusic;
    public CommonDialog m_QuitDialog;
    public CommonDialog m_Logo;

    //public Text txtLevelInfo;

    public CommonDialog m_TopPanel;

    public GameStates gameState;

    private bool eventSubscribed = false;

    private float m_RewardTimer;
    private float m_RewardShowTime;

    private EventData m_CurrentRevardEventData;
    private long m_CurrentGigBonus;

    private bool m_LikeGameDelayed = false;
    public bool test;
    public string test_Dialog = "RewardReceived";

    //public LevelLogic m_CurrentLevelLogic = null;

    public RewardArea m_AutoTapRewardBtn;
    public EventIcon m_EventIcon_AutoTap;
    public GameObject goAutoTapDlg;
    public GameObject goAutoTapIcon;

    private float m_AutoTapRevardTimer;
    private float m_AutoTapRevardShowTime;
    private float m_AutoTapTryoutDuration;
    private float m_AutoTapTryoutTimer;

    private bool m_ShowInterstitial = false;

    [SerializeField]
    private bool m_BoostIdleEnabled = false;
    [SerializeField]
    private bool m_BoostTapEnabled = false;
    [SerializeField]
    private float m_BoostIdleTimer;
    [SerializeField]
    private float m_BoostIdleDuration;
    [SerializeField]
    private float m_BoostTapTimer;
    [SerializeField]
    private float m_BoostTapDuration;

    //private hillo hilloInstance = null;
    //private int startLevel;

    public enum GameStates
    {
        GameStart,
        Menu,
        Map
    }


    public AudioClip acError;
    // Use this for initialization

    private bool autoTapOwned;

    void Awake()
    {
        if (GameManager.Instance == null)
            return;
        Instance = this;

        if (m_RewardEvent.gameObject.activeSelf == true)
        {
            m_RewardEvent.gameObject.SetActive(false);
        }
       /* if (m_LuckyDrawEvent.gameObject.activeSelf == true)
        {
            m_LuckyDrawEvent.gameObject.SetActive(false);
        }*/
        if (m_QuitDialog.gameObject.activeSelf == true)
            m_QuitDialog.gameObject.SetActive(false);
        if (m_EventIcon_IdleBoost.gameObject.activeSelf == true)
            m_EventIcon_IdleBoost.gameObject.SetActive(false);

        /*if (myCanvas.scaleFactor >= 1)
            myEventSystem.pixelDragThreshold = (int)(5 * myCanvas.scaleFactor);
        else
            myEventSystem.pixelDragThreshold = 5;*/

        myEventSystem.pixelDragThreshold = (int)(0.5f * Screen.dpi / 2.54f);
#if SOFTCEN_DEBUG
        Debug.Log("scaleFactor: " + myCanvas.scaleFactor
            + ", DragThreshold: " + myEventSystem.pixelDragThreshold
            );
#endif
    }

    void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        if (GameManager.Instance == null)
            return;

        //hilloInstance = hillo.Instance;
        if (GameManager.Instance.playerData.PrestigeCount > 0 && GameManager.Instance.IsPrestige)
        {
            goPrestigeIcon.SetActive(true);
        }
        else
        {
            goPrestigeIcon.SetActive(false);
        }
        CheckPrestigeButton();

        //CrashReporting.Init(GameConsts.UnityProjectId, PygmyMonkey.AdvancedBuilder.AppParameters.Get.bundleVersion, "SC01");

        //SCAnalytics.InitializeGoogleAnalyticsV4();
        //SCAnalytics.LogScreen(GameConsts.AnalyticsName + " Launch");

        if (m_AutoTapRewardBtn.gameObject.activeSelf)
            m_AutoTapRewardBtn.CloseReward();
        if (m_RewardReceivedDlg.gameObject.activeSelf == true)
        {
            m_RewardReceivedDlg.m_CommonDialog.Hide();
        }

        //startLevel = GameManager.Instance.playerData.Level;

        SetPowerSave();

#if SOFTCEN_DEBUG
        //Profiler.maxNumberOfSamplesPerFrame = -1;
        Debug.Log("HomeManager Start Level:" + GameManager.Instance.playerData.Level
            + ", Chapter: " + GameManager.Instance.playerData.CurrentChapter
            + ", Phase: " + GameManager.Instance.playerData.CurrentPhase
            + ", Selected Chapter: " + GameManager.Instance.selectedChapter
            );
#endif


        gameState = GameStates.Menu;

        InitHomeManager();
        ResetRevardTimer();
        ResetAutoTapRewardTimer();

        //PlayerData_OnLevelChanged();

        //if (gameOnSnapshot != null)
        //{
        //    gameOnSnapshot.TransitionTo(snapshottimeScale);
        //}

        PlayerData_OnStoreItemChanged();
        List<Kauppa.ID> list = Kauppa.Instance.GetStartupConfirmedList();
        if (list.Count > 0)
        {
            Kauppa_OnKauppaSuccess(list);
        }

    }

    void OnDisable()
    {
        if (eventSubscribed)
        {
            eventSubscribed = false;
            Kauppa.OnKauppaProgress -= Kauppa_OnKauppaProgress;
            Kauppa.OnAutotapEnabled -= Kauppa_OnAutotapEnabled;
            Kauppa.OnDisableBannerAds -= Kauppa_OnDisableBannerAds;
            GameManager.OnSaveError -= GameManager_OnSaveError;
            Pilvipalvelut.OnLoggedOut -= GameManager_OnLoggedOut;
            PlayerData.OnLevelChanged -= PlayerData_OnLevelChanged;
            PlayerData.OnMoneyChanged -= HandleOnMoneyChanged;
            PlayerData.OnBonusesChanged -= HandleOnBonusesChanged;
            PlayerData.OnStoreItemChanged -= PlayerData_OnStoreItemChanged;
            //hillo.OnRewardVideoCompleted -= HandleOnRewardVideoCompleted;
            //hillo.OnInterstitialStatusChanged -= AdManager_OnInterstitialStatusChanged;
            Tienistit.OnInterstitialStatusChanged -= Tienistit_OnInterstitialStatusChanged;
            Tienistit.OnRewardVideoCompleted -= Tienistit_OnRewardVideoCompleted;
            SceneActions.OnOpenShopDlg -= SceneActions_OnOpenShopDlg;
            Kauppa.OnKauppaSuccess -= Kauppa_OnKauppaSuccess;
            Kauppa.OnKauppaDeferred -= Kauppa_OnKauppaDeferred;
            Kauppa.OnKauppaPending -= Kauppa_OnKauppaPending;
        }
    }

    void OnEnable()
    {
        if (GameManager.Instance == null)
            return;

        if (!eventSubscribed)
        {
            eventSubscribed = true;
            Kauppa.OnKauppaProgress += Kauppa_OnKauppaProgress;
            Kauppa.OnAutotapEnabled += Kauppa_OnAutotapEnabled;
            Kauppa.OnDisableBannerAds += Kauppa_OnDisableBannerAds;
            GameManager.OnSaveError += GameManager_OnSaveError;
            Pilvipalvelut.OnLoggedOut += GameManager_OnLoggedOut;
            PlayerData.OnLevelChanged += PlayerData_OnLevelChanged;
            PlayerData.OnMoneyChanged += HandleOnMoneyChanged;
            PlayerData.OnBonusesChanged += HandleOnBonusesChanged;
            PlayerData.OnStoreItemChanged += PlayerData_OnStoreItemChanged;
            //hillo.OnRewardVideoCompleted += HandleOnRewardVideoCompleted;
            //hillo.OnInterstitialStatusChanged += AdManager_OnInterstitialStatusChanged;
            Tienistit.OnInterstitialStatusChanged += Tienistit_OnInterstitialStatusChanged;
            Tienistit.OnRewardVideoCompleted += Tienistit_OnRewardVideoCompleted;
            SceneActions.OnOpenShopDlg += SceneActions_OnOpenShopDlg;
            Kauppa.OnKauppaSuccess += Kauppa_OnKauppaSuccess;
            Kauppa.OnKauppaDeferred += Kauppa_OnKauppaDeferred;
            Kauppa.OnKauppaPending += Kauppa_OnKauppaPending;

        }

    }

    private void Kauppa_OnKauppaSuccess(List<Kauppa.ID> ids)
    {
        if (kauppaStatusDlg == null) return;
        string items = "";
        for (int i = 0; i < ids.Count; i++)
        {
            items += Kauppa.Instance.GetProductTitle(ids[i]) + "\n";
        }
        string message = $"Purchase Complete!\nYou received:\n\n {items}";
        kauppaStatusDlg.ShowDialog(message);
    }

    private void Kauppa_OnKauppaDeferred(List<Kauppa.ID> ids)
    {
        if (kauppaStatusDlg == null) return;
        string items = "";
        for (int i = 0; i < ids.Count; i++)
        {
            items += Kauppa.Instance.GetProductTitle(ids[i]) + "\n";
        }
        string monikko = ids.Count > 1 ? "items" : "item";
        string message = $"Purchase of\n {items}\n is awaiting approval. You'll receive your {monikko} once approved.";
        kauppaStatusDlg.ShowDialog(message);
    }

    private void Kauppa_OnKauppaPending(List<Kauppa.ID> ids)
    {
        if (kauppaStatusDlg == null) return;
        string items = "";
        for (int i = 0; i < ids.Count; i++)
        {
            items += Kauppa.Instance.GetProductTitle(ids[i]) + "\n";
        }
        string monikko = ids.Count > 1 ? "items" : "item";
        string message = $"Purchase of\n {items}\nis been processed. You'll receive your {monikko} shortly.";
        kauppaStatusDlg.ShowDialog(message);
    }

    private void Kauppa_OnKauppaProgress(Kauppa.PROGRESS progressState)
    {
        switch (progressState)
        {
            case Kauppa.PROGRESS.None:
                kauppaProgress.gameObject.SetActive(false);
                break;
            case Kauppa.PROGRESS.Started:
                kauppaProgress.gameObject.SetActive(true);
                break;
            case Kauppa.PROGRESS.Cancelled:
                kauppaProgress.CloseWithMessage("Cancelled");
                break;
            case Kauppa.PROGRESS.Failed:
                kauppaProgress.gameObject.SetActive(false);
                break;
            case Kauppa.PROGRESS.Unavailable:
                kauppaProgress.CloseWithMessage("Unavailable");
                break;
            case Kauppa.PROGRESS.PaymentDeclined:
                kauppaProgress.CloseWithMessage("Payment Declined!");
                break;
            default:
                kauppaProgress.gameObject.SetActive(false);
                break;
        }
    }


    private void Kauppa_OnDisableBannerAds()
    {
        goDisableBannerAdsEnabled.SetActive(true);
    }

    private void Kauppa_OnAutotapEnabled()
    {
        goAutotapEnabled.SetActive(true);
    }

    private bool m_OnInterstitialStatusChanged = false;
    private bool m_OnRewardVideoCompleted = false;
    private Tienistit.RewardResult m_result;
    private int m_rewardId;
    private void Tienistit_OnRewardVideoCompleted(int id, Tienistit.RewardResult result)
    {
        m_rewardId = id;
        m_result = result;
        m_OnRewardVideoCompleted = true;
    }

    private void Tienistit_OnInterstitialStatusChanged(int obj)
    {
        m_OnInterstitialStatusChanged = true;
    }

    private void GameManager_OnLoggedOut()
    {
        OpenLoggedOut();
    }

    private void SceneActions_OnOpenShopDlg()
    {
        mainMenuButton.OpenShop();
    }

    /*private void AdManager_OnInterstitialStatusChanged(int obj)
    {
        m_AdManager_OnInterstitialStatusChanged = true;
    }*/

    private void PlayerData_OnStoreItemChanged()
    {
        autoTapOwned = GameManager.Instance.playerData.AutoTapOwned;
        if (autoTapOwned)
        {
            goAutoTapIcon.SetActive(true);
        }
        else
        {
            goAutoTapIcon.SetActive(false);
        }
    }

    private void GameManager_OnSaveError()
    {
        if (GameManager.Instance.saveFailedShowed == false)
        {
            GameManager.Instance.saveFailedShowed = true;
            m_DialogManager.OpenDialog("SaveFailedDlg");
        }
    }

    private void PlayerData_OnLevelChanged()
    {
        CheckPrestigeButton();
    }

    void HandleOnBonusesChanged()
    {
        //UpdateBtnNotifications();
    }


    void HandleOnMoneyChanged(double val)
    {
        CheckNotifications(true);
    }


    private void OpenRewardEvent()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (m_OnRewardVideoCompleted)
        {
            m_OnRewardVideoCompleted = false;
            HandleOnRewardVideoCompleted(m_rewardId, m_result);

        }
        /*if (hilloInstance.completedRewardList.Count > 0) {
            HandleOnRewardVideoCompleted (0);
            hilloInstance.completedRewardList.Clear ();
        }*/

        /*if (Time.frameCount % 30 == 0)
        {
            System.GC.Collect();
        }*/
        if (test)
        {
            test = false;
            OpenLoggedOut();
            //lvlResources.PlayerData_OnLevelChanged();
            //m_DialogManager.ToggleDialog(test_Dialog);
        }

        // Interstitial Ad Check
        if (m_OnInterstitialStatusChanged)
        {
            m_OnInterstitialStatusChanged = false;
            if (goSponsorPanel.activeSelf)
            {
                goSponsorPanel.SetActive(false);
            }
        }
        /*if (hilloInstance.m_AdManager_OnInterstitialStatusChanged)
        {
            hilloInstance.m_AdManager_OnInterstitialStatusChanged = false;
            if (goSponsorPanel.activeSelf)
                goSponsorPanel.SetActive(false);
        }*/
        if (m_ShowInterstitial == true)
        {
            if (goSponsorPanel.activeSelf)
            {
                m_ShowInterstitial = false;
                GameManager.Instance.ShowInterstitial(false);
                //SoftcenSDK.AdManager.Instance.ShowInterstitial(false);
            }
            else
            {
                goSponsorPanel.SetActive(true);
            }
        }

        if (m_BoostIdleEnabled)
        {
            m_BoostIdleTimer += Time.deltaTime;
            if (m_BoostIdleTimer > m_BoostIdleDuration)
            {
                m_EventIcon_IdleBoost.imgDuration.fillAmount = 1;
                m_EventIcon_IdleBoost.gameObject.SetActive(false);
                GameManager.Instance.playerData.SetIdleMultipler(1f);
                m_BoostIdleEnabled = false;
            }
            else
            {
                m_EventIcon_IdleBoost.imgDuration.fillAmount = m_BoostIdleTimer / m_BoostIdleDuration;
            }
        }
        else if (m_BoostTapEnabled)
        {
            m_BoostTapTimer += Time.deltaTime;
            if (m_BoostTapTimer >= m_BoostTapDuration)
            {
                m_EventIcon_TapBoost.imgDuration.fillAmount = 1;
                m_EventIcon_TapBoost.gameObject.SetActive(false);
                GameManager.Instance.playerData.SetTapMultipler(1f);
                m_BoostTapEnabled = false;
            }
            else
            {
                m_EventIcon_TapBoost.imgDuration.fillAmount = m_BoostTapTimer / m_BoostTapDuration;
            }
        }
        else
        {
            m_RewardTimer += Time.deltaTime;
            if (m_RewardTimer >= m_RewardShowTime)
            {
                ResetRevardTimer();
                if (GameManager.Instance.hasRewardVideo())
                {
                    // Show reward button
                    //Debug.Log("<color=yellow>Activate m_RewardArea</color>");
                    m_RewardArea.gameObject.SetActive(true);
                }
            }
        }

        // Autotap EventIcon
        if (!autoTapOwned)
        {
            if (GameManager.Instance.autotap_tryout)
            {
                // Autotap tryout running
                m_AutoTapTryoutTimer += Time.deltaTime;
                if (m_AutoTapTryoutTimer >= m_AutoTapTryoutDuration)
                {
                    m_EventIcon_AutoTap.imgDuration.fillAmount = 1;
                    m_EventIcon_AutoTap.gameObject.SetActive(false);
                    GameManager.Instance.autotap_tryout = false;
                }
                else
                {
                    m_EventIcon_AutoTap.imgDuration.fillAmount = m_AutoTapTryoutTimer / m_AutoTapTryoutDuration;
                }
            }
            else
            {
                if (!goAutoTapDlg.activeSelf)
                {
                    m_AutoTapRevardTimer += Time.deltaTime;
                    if (m_AutoTapRevardTimer >= m_AutoTapRevardShowTime)
                    {
                        ResetAutoTapRewardTimer();
                        if (m_EventIcon_AutoTap.gameObject.activeSelf == false)
                        {
                            if (GameManager.Instance.hasRewardVideo())
                            {
                                Debug.Log("<color=yellow>Activate m_AutoTapRewardBtn</color>");
                                m_AutoTapRewardBtn.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
        }

        /*if (!autoTapOwned && !GameManager.Instance.autotap_tryout
            && !m_AutoTapRewardBtn.gameObject.activeSelf
            && !goAutoTapDlg.activeSelf
            )
        {
            m_AutoTapRevardTimer += Time.deltaTime;
            if (m_AutoTapRevardTimer >= m_AutoTapRevardShowTime)
            {
                ResetAutoTapRewardTimer();
                if (m_EventIcon_AutoTap.gameObject.activeSelf == false)
                {
                    if (GameManager.Instance.hasRewardVideo())
                    {
                        m_AutoTapRewardBtn.gameObject.SetActive(true);
                    }
                }
            }
        }*/

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Pilvipalvelut.Instance.loggedOut)
            {
                LoggedOutQuit();
                return;
            }
            if (m_DialogManager.CheckEscape())
            {
                return;
            }
            if (m_RewardReceivedDlg.gameObject.activeSelf == true)
            {
                ClaimReward();
                return;
            }

            if (m_QuitDialog.gameObject.activeSelf)
            {
                m_QuitDialog.Button_Close();
                return;
            }

#if UNITY_ANDROID || UNITY_STANDALONE
            m_QuitDialog.Show();
#else
            UserQuitAppliation();
#endif
        }
#if SOFTCEN_DEBUG
        if (Input.GetKeyDown(KeyCode.W))
        {
            //mainMenuButton.OpenIdleUpgrades();
            OpenLikeGameDlg();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Utilities.RequestStoreReview();
        }
#endif
    }

    private bool AnyDialogOpen()
    {
        return m_DialogManager.AnyDialogOpen();
    }

    public void ResetButton()
    {
#if SOFTCEN_DEBUG
        GameManager.Instance.ResetGame();
        mainMenuButton.OpenMap();
        //Scene m_currentScene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(m_currentScene.name);
        //SceneManager.LoadScene(GameConsts.Skenes.Loader);
        /*gameState = GameStates.GameStart;
        InitHomeManager();
        m_DialogManager.CloseDialog("SettingsDlg");*/
#endif
    }

    public void CreditsButton()
    {
        CloseMoreButtons();
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("CreditsDialog");

    }
    public void SettingsButtons()
    {
        CloseMoreButtons();
        GameManager.Instance.tapDisabled = true;
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("SettingsDlgGE");
        ShowInterstitial();
    }
    public void OpenStaffDlg()
    {
        CloseMoreButtons();
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("IdleDlg");
        ShowInterstitial();
    }

    public void EquipmentsButton()
    {
        CloseMoreButtons();
        GameManager.Instance.tapDisabled = true;
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("EquipmentsDlgGE");
        ShowInterstitial();
    }
    public void PowerUpButton()
    {
        CloseMoreButtons();
        GameManager.Instance.tapDisabled = true;
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("PowerUpsDlgGE");
        ShowInterstitial();
    }

    public void FamousMissingBuyNowButton()
    {
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.CloseDialog("FamousMissingItemDlgGE");
        CapasityButton();
    }

    public void ShopButton()
    {
        CloseMoreButtons();
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("ShopDlg");
    }
    public void CapasityButton()
    {
        CloseMoreButtons();
        GameManager.Instance.tapDisabled = true;
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("CapacityDlgGE");
        ShowInterstitial();

    }

    public void CloseDialog(GameObject go)
    {
    }

    public void DialogClosed()
    {
        if (!AnyDialogOpen())
        {
            //Debug.Log("DialogClosed true");
            ShowLogo();
            GameManager.Instance.tapDisabled = false;
        }
        else
        {
#if SOFTCEN_DEBUG
            Debug.Log("DialogClosed false");
#endif
            GameManager.Instance.tapDisabled = true;
        }
    }

    public void CloseMoreButtons()
    {
        //m_MainMenuManager.CloseMenuButtons();
    }


    private void ShowLogo()
    {
        /*if (!m_Logo.gameObject.activeSelf)
        {
            m_Logo.Show();
        }*/
    }

    public void UserQuitAppliation()
    {
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.Save();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    public void OpenLoggedOut()
    {
        Pilvipalvelut.Instance.loggedOut = true;
        GameManager.Instance.Save();
        m_DialogManager.OpenDialog("LoggedOut");
    }
    [SkipRename]
    public void LoggedOutQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    public void OpenNoDiamondsDlg()
    {
        CloseMoreButtons();
        GameManager.Instance.tapDisabled = true;
        AudioManager.Instance.PlayClip(acError);
        m_DialogManager.OpenDialog("NoDiamondsDlg");

    }

    public void OpenGigMapDlg()
    {
        CloseMoreButtons();
    }

    public void OpenRateDlg()
    {
        GameManager.Instance.rateGameTime = Time.time;

        m_DialogManager.OpenDialog("RateGameDlg");
        m_DialogManager.CheckDialogs();
    }
    [SkipRename]
    public void RateNoButton()
    {
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.playerData.askRateLater = false;
        GameManager.Instance.Save();
        m_DialogManager.CloseDialog("RateGameDlg");
    }
    [SkipRename]
    public void RateSureButton()
    {
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.playerData.askRateLater = false;
        GameManager.Instance.Save();
        m_DialogManager.CloseDialog("RateGameDlg");
        Utilities.RequestStoreReview();
        //OpenStoreRate();
    }

    [ObfuscateLiterals]
    private void OpenStoreRate()
    {
        // TODO: Change Rate URL
#if SOFTCEN_AMAZON
        Application.OpenURL("http://www.amazon.com/gp/mas/dl/android?p=com.softcen.harbor.tycoon");
#endif
#if UNITY_ANDROID && !SOFTCEN_AMAZON
        Application.OpenURL("market://details?id=com.softcen.airfield.tycoon.clicker");
#endif
#if UNITY_IOS || UNITY_IPHONE
        // TODO
        Application.OpenURL("itms-apps://itunes.apple.com/app/id1069619421");
#endif

    }

    [SkipRename]
    public void RateLaterButton()
    {
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.playerData.askRateLater = true;
        GameManager.Instance.Save();
        m_DialogManager.CloseDialog("RateGameDlg");

    }
    [SkipRename]
    public void LikeYesButton()
    {
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.CloseDialog("LikeGameDlg");
        GameManager gm = GameManager.Instance;
        gm.playerData.likeAsked = true;
        gm.playerData.playerLike = true;
        gm.playerData.askRateLater = true;
        gm.Save();
    }
    [SkipRename]
    public void LikeNoButton()
    {
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.CloseDialog("LikeGameDlg");
        GameManager gm = GameManager.Instance;
        gm.playerData.likeAsked = true;
        gm.playerData.playerLike = false;
        gm.playerData.askRateLater = false;
        gm.Save();
    }

    public void OpenLikeGameDlg()
    {
        GameManager gm = GameManager.Instance;
        gm.rateGameTime = Time.time;
        gm.playerData.likeAsked = true;
        gm.Save();
        m_DialogManager.OpenDialog("LikeGameDlg");
        m_DialogManager.CheckDialogs();
    }


    private void InitHomeManager()
    {

        m_DialogManager.CheckDialogs();
        if (gameState == GameStates.GameStart)
        {
            mainMenuButton.SetMainMenuButtonActive(false);
        }
        else if (gameState == GameStates.Menu)
        {
            mainMenuButton.SetMainMenuButtonActive(true);
            ResetRevardTimer();
            m_TopPanel.Show();
        }
    }




    private void CheckNotifications(bool fast)
    {

    }

    public void ShowInterstitial()
    {
        GameManager gm = GameManager.Instance;
#if SOFTCEN_DEBUG
        Debug.Log("ShowInterstitial level: " + gm.playerData.Level);
#endif
        if (gm.playerData.Level <= 1)
            return;

        // Like or Rate Game
        if (gm.playerData.Level >= GameConsts.RateGameMinLevel)
        {
            float elapsedTime = Time.time - gm.rateGameTime;
#if SOFTCEN_DEBUG
            Debug.Log("<color=aqua>ShowInterstitial() "
                + ", elapsedTime: " + elapsedTime
                + ", likeAsked: " + gm.playerData.likeAsked
                + ", askRateLater: " + gm.playerData.askRateLater
                + "</color>"
                );
#endif
            if (elapsedTime >= GameConsts.RateGameTimeout)
            {
                gm.rateGameTime = Time.time;
                if (!gm.playerData.likeAsked)
                {
                    OpenLikeGameDlg();
                    return;
                }
                else if (gm.playerData.askRateLater)
                {
                    OpenRateDlg();
                    return;
                }
            }
        }

        if (GameManager.Instance.CanShowInterstitialAd(true))
        {
            if (GameManager.Instance.HasInterstitial())
            {
                m_ShowInterstitial = true;
            }
        }
    }

    public void SetPowerSave()
    {
        if (GameManager.Instance.playerData.powerSave)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            //Application.targetFrameRate = 30;
            Application.targetFrameRate = -1;
        }
        else
        {
            //Screen.sleepTimeout = SleepTimeout.SystemSetting;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = -1;
        }
    }

    /// <summary>
    /// Called when Reward button is pressed.
    /// </summary>
    ///
    [SkipRename]
    public void Button_RewardEvent()
    {
        AudioManager.Instance.PlayButtonClick();
        m_CurrentRevardEventData = EventManager.Instance.GetRandomRewardEvent();
        m_RewardArea.CloseReward();
        GameManager.Instance.tapDisabled = true;
        //m_RewardEvent.gameObject.SetActive(true);
        //m_DialogManager.OpenDialog("RewardEventDlg");
        m_RewardEvent.Show("RevardEventDlg", m_CurrentRevardEventData, m_DialogManager);

        /*
        AudioManager.Instance.PlayButtonClick();
        m_AutoTapRewardBtn.CloseReward();
        ResetAutoTapRewardTimer();
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("AutoTapDlg");
        m_DialogManager.CheckDialogs();
        */
    }

    public void Button_LuckyDraw()
    {
        //m_LuckyDrawArea.CloseReward();
        GameManager.Instance.tapDisabled = true;
        m_DialogManager.OpenDialog("LuckyDrawEvent");
        //m_LuckyDrawEvent.Show();
    }

    public void ResetRevardTimer()
    {
        m_RewardTimer = 0f;
        m_RewardShowTime = UnityEngine.Random.Range(GameConsts.Reward.RewardEventMinTime, GameConsts.Reward.RewardEventMaxTime);
    }


    public void ResetAutoTapRewardTimer()
    {
        m_AutoTapRevardTimer = 0f;
        m_AutoTapRevardShowTime = UnityEngine.Random.Range(GameConsts.AutoTap.MinTime, GameConsts.AutoTap.MaxTime);
    }

    public void ShowRewardVideo()
    {
        goSponsorPanel.SetActive(true);

        GameManager.Instance.showRewardVideo(m_CurrentRevardEventData.Id);
    }

    void HandleOnRewardVideoCompleted(int id, Tienistit.RewardResult result)
    {
        if (m_CurrentRevardEventData != null)
        {

#if SOFTCEN_DEBUG
            Debug.Log("HandleOnRewardVideoCompleted int: " + id.ToString()
                      + ", currentId: " + m_CurrentRevardEventData.Id.ToString()
                      + ", result: " + result.ToString()
                      );
#endif
            if (goSponsorPanel != null && goSponsorPanel.activeSelf)
                goSponsorPanel.SetActive(false);

            if (m_CurrentRevardEventData.Id == EventManager.Instance.IlmainenArkkuEventId () && result == Tienistit.RewardResult.Finished) {
                GameManager.Instance.playerData.ChangeDiamonds (m_CurrentRevardEventData.Timantit + m_CurrentRevardEventData.Timantit);
                GameManager.Instance.playerData.IncMoney (m_CurrentRevardEventData.Value + m_CurrentRevardEventData.Value);
                GameManager.Instance.Save ();
            }
            else if (m_RewardReceivedDlg != null && result == Tienistit.RewardResult.Finished)
            {
                m_RewardReceivedDlg.gameObject.SetActive(true);
                //string description = m_CurrentRevardEventData.Description + "\nREWARD ACCEPTED!";
                m_CurrentRevardEventData.Description = "Reward Accepted!";
                m_RewardReceivedDlg.RewardAccepted("RewardReceivedDlg", m_CurrentRevardEventData, m_DialogManager);
            }
        }
    }

    [SkipRename]
    public void ClaimReward()
    {
        if (m_CurrentRevardEventData != null)
        {
            GameManager.Instance.playerData.Score += 1;
            m_RewardReceivedDlg.Button_Close();
#if SOFTCEN_DEBUG
            Debug.Log("ClaimReward " + (int)m_CurrentRevardEventData.Type
                      + ", value: " + m_CurrentRevardEventData.Value.ToString()
                      + ", duration:" + m_CurrentRevardEventData.Duration.ToString()
                      );
#endif
            if (m_CurrentRevardEventData.Type == EventManager.boostType.InstantTap)
            {
                double instantBonus = GameManager.Instance.playerData.GetCurrentTapValue() * m_CurrentRevardEventData.Value;
                GameManager.Instance.playerData.IncMoney(instantBonus);
            }
            else if (m_CurrentRevardEventData.Type == EventManager.boostType.IdleBoost)
            {
                m_BoostIdleDuration = m_CurrentRevardEventData.Duration;
                m_BoostIdleTimer = 0;
                m_BoostIdleEnabled = true;
                m_EventIcon_IdleBoost.StartBoost(m_CurrentRevardEventData.Duration, m_CurrentRevardEventData.Value, EventManager.boostType.IdleBoost);
            }
            else if (m_CurrentRevardEventData.Type == EventManager.boostType.TapBoost)
            {
                m_BoostTapDuration = m_CurrentRevardEventData.Duration;
                m_BoostTapTimer = 0;
                m_BoostTapEnabled = true;
                m_EventIcon_TapBoost.StartBoost(m_CurrentRevardEventData.Duration, m_CurrentRevardEventData.Value, EventManager.boostType.TapBoost);
            }
            else if (m_CurrentRevardEventData.Type == EventManager.boostType.DoubleBonus)
            {
                GameManager.Instance.playerData.IncMoney(m_CurrentRevardEventData.Value);
            }
            else if (m_CurrentRevardEventData.Type == EventManager.boostType.AutoTap)
            {
                if (m_AutoTapRewardBtn.gameObject.activeSelf)
                    m_AutoTapRewardBtn.CloseReward();
                m_AutoTapTryoutTimer = 0;
                m_AutoTapTryoutDuration = m_CurrentRevardEventData.Duration;
                m_EventIcon_AutoTap.StartBoost(m_CurrentRevardEventData.Duration, m_CurrentRevardEventData.Value, EventManager.boostType.AutoTap);
            }
            m_CurrentRevardEventData = null;

        }
        else if (m_LikeGameDelayed == true)
        {
            OpenLikeGameDlg();
        }
    }


    public void DoubleBonusRewardVideo()
    {
        //m_DialogManager.CloseDialog("GigRewardDlg");
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;

        m_CurrentRevardEventData = EventManager.Instance.GetDoubleBonusEvent();
        m_CurrentRevardEventData.Duration = 0f;
        m_CurrentRevardEventData.Value = 2 * mi.GetBonus();

        MissionManager.Instance.CreateNewMission();

#if SOFTCEN_DEBUG
        Debug.Log("DoubleBonus " + m_CurrentRevardEventData.Value.ToString());
#endif
        ShowRewardVideo();
    }

    public void OpenUpgradeDlg()
    {
        CloseMoreButtons();
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("UpgradeDlg");
        ShowInterstitial();
    }

    public void CloseUpgradeDlg()
    {
        m_DialogManager.CloseDialog("UpgradeDlg");
    }

    public void OpenTapBonusDlg()
    {
        CloseMoreButtons();
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("TapBonusDlg");
        ShowInterstitial();

    }
    public void OpenIdleBonusDlg()
    {
        CloseMoreButtons();
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("IdleBonusDlg");
        ShowInterstitial();

    }

    public void OpenMissionDlg()
    {
        CloseMoreButtons();
        AudioManager.Instance.PlayButtonClick();
        MissionManager.Instance.SetupMissionDlg();
        m_DialogManager.OpenDialog("MissionDlg");
    }

    public void CloseMissionDlg()
    {
        m_DialogManager.CloseDialog("MissionDlg");

    }

    [SkipRename]
    public void OpenAutoTapRewardDlg()
    {
        AudioManager.Instance.PlayButtonClick();
        m_AutoTapRewardBtn.CloseReward();
        ResetAutoTapRewardTimer();
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.OpenDialog("AutoTapDlg");
        m_DialogManager.CheckDialogs();
    }
    [SkipRename]
    public void TryAutoTap()
    {
        AudioManager.Instance.PlayButtonClick();
        GameObject goAutoTap = m_DialogManager.GetDialog("AutoTapDlg");
        if (goAutoTap != null)
        {
            goAutoTap.SetActive(false);
        }
        //m_DialogManager.CloseDialog("AutoTapDlg");
        m_CurrentRevardEventData = EventManager.Instance.GetAutoTapEvent();
        ShowRewardVideo();

    }
    [SkipRename]
    public void BuyAutoTap()
    {
        AudioManager.Instance.PlayButtonClick();
        m_DialogManager.CloseDialog("AutoTapDlg");
        Kauppa.Instance.BuyProductID(Kauppa.ID.INN_APP_AUTO_TAP);
    }

    public void HideGameView(bool state)
    {
        if (lvlResources == null || lvlResources.currentLevelManager == null)
        {
            return;
        }

        if (state)
        {
            if (lvlResources.currentLevelManager.gameObject.activeSelf)
            {
                lvlResources.currentLevelManager.gameObject.SetActive(false);
            }
            ShowBusinessman(false);
        }
        else
        {
            if (!lvlResources.currentLevelManager.gameObject.activeSelf)
            {
                lvlResources.currentLevelManager.gameObject.SetActive(true);
                //GameManager.Instance.playerData.CallOnLevelChanged();
            }
            ShowBusinessman(true);
        }
    }

    public void CloseAllDialogs()
    {
        mainMenuButton.CloseAll();
    }

    public void StartGameButton()
    {
        if (GameManager.Instance.playerData.CurrentChapter == 0)
        {
            GameManager.Instance.playerData.CurrentChapter = 1;
            GameManager.Instance.playerData.CurrentPhase = 0;
            GameManager.Instance.playerData.IncLevel();
            mainMenuButton.CloseMap();
            gameState = GameStates.Menu;
            InitHomeManager();
        }
    }


    public void SetMapView(bool showMap)
    {
        /* Not used anymore
        if (showMap)
        {
            lvlResources.camFlow.PauseGameView(true);
            HideGameView(true);
            //goMap.SetActive(true);
            worldMapView.gameObject.SetActive(true);
            worldMapView.SetActiveHarbor();
            mapDlg.SetPrices(worldMapView.currentCoinPrice, worldMapView.currentDiamondPrice);
            goGameOnButtons.SetActive(false);
            goAutoTapIcon.SetActive(false);
            for (int i = 0; i < menuHideObjects.Length; i++)
            {
                if (menuHideObjects[i] != null)
                {
                    menuHideObjects[i].stateWhenHide = menuHideObjects[i].gameObject.activeSelf;
                    menuHideObjects[i].HideItem(true);
                }
            }
        }
        else
        {
            PlayerData_OnStoreItemChanged();
            lvlResources.camFlow.PauseGameView(false);
            worldMapView.gameObject.SetActive(false);
            //goMap.SetActive(false);
            goGameOnButtons.SetActive(true);
            HideGameView(false);
            for (int i = 0; i < menuHideObjects.Length; i++)
            {
                if (menuHideObjects[i] != null)
                {
                    menuHideObjects[i].HideItem(false);
                }
            }

        }*/
    }

    public void ShowBusinessman(bool state)
    {
        if (lvlResources != null)
        {
            lvlResources.SetBusinessManActive(state);
        }
    }

    /*public bool IsPhaseCompleted()
    {
        if (lvlResources != null)
        {
            return lvlResources.IsPhaseCompleted();
        }
        return false;
    }*/

    public void BuyNewChapterWithCoins()
    {
        /*
        double price = 0; // worldMapView.currentCoinPrice;
#if SOFTCEN_DEBUG
        if (GameManager.Instance.dev_SkipMoney)
            GameManager.Instance.playerData.IncMoney(price);
#endif

        if (price <= GameManager.Instance.playerData.Money)
        {
            GameManager.Instance.playerData.DecMoney(price);
            GameManager.Instance.playerData.IncCurrentChapter();
            GameManager.Instance.Save();
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Level", "Level" + GameManager.Instance.playerData.Level.ToString(), 1);
        }*/
    }
    public void BuyNewChapterWithDiamonds()
    {
        /*
        int diamondPrice = 0; // worldMapView.currentDiamondPrice;
        if (diamondPrice <= GameManager.Instance.playerData.Diamonds)
        {
            GameManager.Instance.playerData.ChangeDiamonds(-1*diamondPrice);
            GameManager.Instance.playerData.IncCurrentChapter();
            GameManager.Instance.Save();
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Level", "Level" + GameManager.Instance.playerData.Level.ToString(), 1);
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Diamond Use", "Chapter", 1);
        }
        else
        {
            OpenNoDiamondsDlg();
        }*/
    }

    public void SetLevelActive(bool state)
    {
        if (lvlResources != null)
        {
            if (lvlResources.currentLevelManager != null)
            {
                lvlResources.currentLevelManager.gameObject.SetActive(state);
            }
        }
        CheckPrestigeButton();
    }

    private void CheckPrestigeButton()
    {
        int currentChapter = GameManager.Instance.playerData.CurrentChapter;
        int maxPhase = BonusManager.Instance.GetChapterMaxPhase(currentChapter);
#if SOFTCEN_DEBUG
        Debug.Log("<color=yellow>CheckPrestigeButton "
            + ", Chapter: " + currentChapter
            + " / " + Chapters.MaxChapter
            + ", Phase: " + GameManager.Instance.playerData.CurrentPhase
            + " / " + maxPhase
            + "</color>"
            );
#endif
        if (currentChapter == Chapters.MaxChapter && GameManager.Instance.playerData.CurrentPhase == maxPhase)
        {
            goPrestigeButton.SetActive(true);
        }
        else
        {
            goPrestigeButton.SetActive(false);
        }
    }

    public void ShowLeaderboard()
    {
        if (Pelikeskus.Instance.Authenticated())
        {
            Pelikeskus.Instance.NaytaTulostaulukko();
        }
        else
        {
            #if !UNITY_IOS
            m_DialogManager.OpenDialog("GPSignIn");
            m_DialogManager.CheckDialogs();
            #endif
        }
    }

    public void ShowAchievements()
    {
        if (Pelikeskus.Instance.Authenticated())
        {
            Pelikeskus.Instance.NaytaSaavutukset();
            GameManager.Instance.ShowAchievements();
        }
        else
        {
            #if !UNITY_IOS
            m_DialogManager.OpenDialog("GPSignIn");
            m_DialogManager.CheckDialogs();
            #endif
        }

    }
    public void AuthenticateUser()
    {
        Pelikeskus.Instance.Authenticate();
    }

    public void OpenSaveFailedDlg()
    {
        m_DialogManager.OpenDialog("SaveFailedDlg");
        m_DialogManager.CheckDialogs();
    }

    public void TuplaaIlmainenArkku(double k, int t)
    {
        m_CurrentRevardEventData = EventManager.Instance.GetIlmainenArkkuEvent ();
        m_CurrentRevardEventData.Timantit = t;
        m_CurrentRevardEventData.Value = k;

        #if SOFTCEN_DEBUG
        Debug.Log("TuplaaIlmainenArkku k: " + k + ", t: " + t);
        #endif
        ShowRewardVideo();
    }

}
