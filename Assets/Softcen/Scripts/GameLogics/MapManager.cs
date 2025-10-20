using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Beebyte.Obfuscator;
using TMPro;

public class MapManager : MonoBehaviour {
    public GameObject[] gameStartObjs;
    public GameObject[] gameMapObjs;
    public LoadingPanel loadPanel;
    public CameraFlowMap camFlowMap;
    public GameObject goBuyPanel;
    public TextMeshProUGUI txtDiamondPrice;
    public TextMeshProUGUI txtCoinPrice;

    public ChapterMap[] chapters;
    public Color colorActive;
    public Color colorInactive;
    public Color colorDone;

    public Button goBuyCoinBtn;
    public CommonDialog NoDiamondsDlg;
    public CommonDialog ShopDlg;
    public CommonDialog QuitGameDlg;
    public CommonDialog LoggedOutDlg;

    private double coinPrice;
    private int diamondPrice;
    private bool eventSubscribed = false;

    public GameObject goBuyDiamondButton;
    public GameObject goBuyCoinButton;
    public GameObject goPrevButton;
    public GameObject goNextButton;
    public GameObject goSelectButton;
    public GameObject goLockedButton;
    public GameObject goStartButton;
    public TextMeshProUGUI txtAirfieldTitle;
    public TextMeshProUGUI txtAirfieldSubTitle;

    public GameObject goImagePhoto;
    public Image imageAirfield;
    public SCUIAnim scuianimPhoto;

    private int selectedAirfield;
    private bool m_iscompleted;

    void Start()
    {
        GameManager gmi = GameManager.Instance;
        if (gmi == null || BonusManager.Instance == null)
            return;

        selectedAirfield = 0;
#if SOFTCEN_DEBUG
        Debug.Log("MapManager Start CurrentChapter: " + gmi.playerData.CurrentChapter);
#endif
        SetStartMenu(gmi.playerData.CurrentChapter == 0);

        for (int i = 0; i < chapters.Length; i++)
        {
            if ((int)chapters[i].Id == gmi.playerData.CurrentChapter)
            {
                selectedAirfield = i;
            }
            chapters[i].chapterInfo = BonusManager.Instance.GetChapterInfo((int)chapters[i].Id);
            chapters[i].tmTitle.text = chapters[i].chapterInfo.ChapterName;
        }

        if (gmi.playerData.CurrentChapter == 0)
        {
            selectedAirfield = 0;
        }
        else
        {
            m_iscompleted = BonusManager.Instance.IsChapterCompleted(gmi.playerData.CurrentChapter);
            if (m_iscompleted)
            {
                selectedAirfield = Mathf.Min(selectedAirfield + 1, chapters.Length - 1);
            }
        }
        SetActiveChapter();

    }
    void OnEnable()
    {
        if (!eventSubscribed)
        {
            eventSubscribed = true;
            PlayerData.OnMoneyChanged += HandleOnMoneyChanged;
            SceneActions.OnOpenShopDlg += SceneActions_OnOpenShopDlg;
            Pilvipalvelut.OnLoggedOut += GameManager_OnLoggedOut;
        }
    }

    private void GameManager_OnLoggedOut()
    {
        m_loggedOut = true;
        LoggedOutDlg.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        if (eventSubscribed)
        {
            eventSubscribed = false;
            PlayerData.OnMoneyChanged -= HandleOnMoneyChanged;
            SceneActions.OnOpenShopDlg -= SceneActions_OnOpenShopDlg;
            Pilvipalvelut.OnLoggedOut -= GameManager_OnLoggedOut;
        }
    }

    private bool m_loggedOut = false;
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (m_loggedOut)
                Application.Quit();
            else
                BackButtonPress();
        }
    }
    private void SceneActions_OnOpenShopDlg()
    {
        ShopDlg.Show();
    }

    void HandleOnMoneyChanged(double val)
    {
        if (goBuyPanel.activeSelf)
        {
            if (coinPrice <= GameManager.Instance.playerData.Money)
            {
                goBuyCoinBtn.interactable = true;
            }
            else
            {
                goBuyCoinBtn.interactable = true;
            }
        }
    }

    [SkipRename]
    public void BackButtonPress()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (QuitGameDlg.gameObject.activeSelf)
        {
            QuitGameDlg.Button_Close();
        }
        else if (gameStartObjs[0].activeSelf)
        {
            OpenQuitGameDlg();
        }
        else if (ShopDlg.gameObject.activeSelf)
        {
            ShopDlg.Button_Close();
        }
        else if (NoDiamondsDlg.gameObject.activeSelf)
        {
            NoDiamondsDlg.Button_Close();
        }
        else
        {
            OpenHome();
        }
    }

    [SkipRename]
    public void OpenHome()
    {
        loadPanel.m_scenename = GameConsts.Skenes.Home;
        loadPanel.gameObject.SetActive(true);
    }

    private void SetStartMenu(bool state)
    {
        for (int i = 0; i < gameStartObjs.Length; i++)
        {
            gameStartObjs[i].SetActive(state);
        }
        for (int i = 0; i < gameMapObjs.Length; i++)
        {
            gameMapObjs[i].SetActive(!state);
        }
    }

    [SkipRename]
    public void StartGame()
    {
        GameManager gmi = GameManager.Instance;
        if (gmi != null)
        {
            if (gmi.playerData.CurrentChapter == 0)
            {
                gmi.playerData.CurrentChapter = 1;
                gmi.playerData.CurrentPhase = 0;
                gmi.playerData.IncLevel();
                gmi.Save();
            }
            gmi.selectedChapter = gmi.playerData.CurrentChapter;
        }
        OpenHome();
    }

    [SkipRename]
    public void BuyNewChapterCoins()
    {
        AudioManager.Instance.PlayButtonClick();

#if SOFTCEN_DEBUG
        if (GameManager.Instance.dev_SkipMoney)
            GameManager.Instance.playerData.IncMoney(coinPrice);
#endif
        if (coinPrice <= GameManager.Instance.playerData.Money)
        {
            GameManager.Instance.playerData.DecMoney(coinPrice);
            GameManager.Instance.playerData.IncCurrentChapter();
            GameManager.Instance.Save();
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Level", "Level" + GameManager.Instance.playerData.Level.ToString(), 1);
            GameManager.Instance.selectedChapter = GameManager.Instance.playerData.CurrentChapter;
            OpenHome();
        }

    }
    [SkipRename]
    public void BuyNewChapterDiamonds()
    {
        AudioManager.Instance.PlayButtonClick();
        if (diamondPrice <= GameManager.Instance.playerData.Diamonds)
        {
            GameManager.Instance.playerData.ChangeDiamonds(-1 * diamondPrice);
            GameManager.Instance.playerData.IncCurrentChapter();
            GameManager.Instance.Save();
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Level", "Level" + GameManager.Instance.playerData.Level.ToString(), 1);
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Diamond Use", "Chapter", 1);
            GameManager.Instance.selectedChapter = GameManager.Instance.playerData.CurrentChapter;
            OpenHome();
        }
        else
        {
            OpenNoDiamondsDlg();
        }

    }

    private void OpenNoDiamondsDlg()
    {
        NoDiamondsDlg.Show();
    }

    private void SetActiveChapter()
    {
        goStartButton.SetActive(false);
        goBuyCoinButton.SetActive(false);
        goBuyDiamondButton.SetActive(false);
        goLockedButton.SetActive(false);
        goSelectButton.SetActive(false);

        if (BonusManager.Instance == null)
            return;

        GameManager gmi = GameManager.Instance;

        int currentId = (int)chapters[selectedAirfield].Id;
#if SOFTCEN_DEBUG
        Debug.Log("SetActiveChapter currentId: " + currentId.ToString()
            + ", seletedAirfield: " + selectedAirfield
            + ", currentChapter: " + gmi.playerData.CurrentChapter
            );
#endif
        if (currentId == 1 && gmi.playerData.CurrentChapter == 0)
        {
            goStartButton.SetActive(true);
        }
        else if ((int)chapters[selectedAirfield].Id < gmi.playerData.CurrentChapter)
        {
            goSelectButton.SetActive(true);
        }
        else if ((int)chapters[selectedAirfield].Id == gmi.playerData.CurrentChapter+1)
        {
            if (m_iscompleted)
            {
                goBuyCoinButton.SetActive(true);
                goBuyDiamondButton.SetActive(true);
                coinPrice = BonusManager.Instance.GetLevelCoinPrice((int)chapters[selectedAirfield].itemId);
                diamondPrice = BonusManager.Instance.GetLevelDiamondPrice((int)chapters[selectedAirfield].itemId);
                txtCoinPrice.SetText(NumToStr.GetNumStr(coinPrice));
                txtDiamondPrice.SetText(NumToStr.GetNumStr(diamondPrice));
                if (coinPrice <= GameManager.Instance.playerData.Money)
                {
                    goBuyCoinBtn.interactable = true;
                }
                else
                {
                    goBuyCoinBtn.interactable = true;
                }

            }
            else
            {
                goLockedButton.SetActive(true);
            }
        }
        else if ((int)chapters[selectedAirfield].Id == gmi.playerData.CurrentChapter)
        {
            goSelectButton.SetActive(true);
        }
        else 
        {
            goLockedButton.SetActive(true);
        }

        camFlowMap.trTarget = chapters[selectedAirfield].transform;
        for (int i = 0; i < chapters.Length; i++)
        {
            if ((int)chapters[i].Id == currentId)
            {
                chapters[i].SetSelected(ChapterMap.Mode.Selected, colorActive);
            }
            else
            {
                if ((int)chapters[i].Id < gmi.playerData.CurrentChapter)
                {
                    chapters[i].SetSelected(ChapterMap.Mode.Done, colorDone);
                }
                else
                {
                    chapters[i].SetSelected(ChapterMap.Mode.Inactive, colorInactive);
                }
            }
        }

#if SOFTCEN_DEBUG
            Debug.Log("SetActive chapter: " + currentId);
#endif


        txtAirfieldTitle.SetText(chapters[selectedAirfield].chapterInfo.ChapterName);
        txtAirfieldSubTitle.SetText(chapters[selectedAirfield].chapterInfo.ChapterDescription);

        if (selectedAirfield >= chapters.Length-1)
        {
            goNextButton.SetActive(false);
        }
        else
        {
            goNextButton.SetActive(true);
        }
        if (selectedAirfield == 0)
        {
            goPrevButton.SetActive(false);
        }
        else
        {
            goPrevButton.SetActive(true);
        }

        if (chapters[selectedAirfield].chapterInfo.chapterSprite != null)
        {
            imageAirfield.sprite = chapters[selectedAirfield].chapterInfo.chapterSprite;
            if (!goImagePhoto.activeSelf)
            {
                goImagePhoto.SetActive(true);
            }
            else
            {
                scuianimPhoto.ResetAnim();
            }

        }
        else
        {
            goImagePhoto.SetActive(false);
        }

        /*for (int i = 0; i < chapters.Length; i++)
        {
            if (chapters[i] != null)
            {
                chapters[i].tmTitle.text = BonusManager.Instance.GetChapterName((int)chapters[i].Id);
                if ((int)chapters[i].Id == currentId)
                {
                    camFlowMap.trTarget = chapters[i].transform;
                    chapters[i].SetSelected(ChapterMap.Mode.Selected, colorActive);
                    if (BonusManager.Instance != null)
                    {
                        coinPrice = BonusManager.Instance.GetLevelCoinPrice((int)chapters[i].itemId);
                        diamondPrice = BonusManager.Instance.GetLevelDiamondPrice((int)chapters[i].itemId);
                        txtCoinPrice.text = NumToStr.GetNumStr(coinPrice);
                        txtDiamondPrice.text = NumToStr.GetNumStr(diamondPrice);
                        if (coinPrice <= GameManager.Instance.playerData.Money)
                        {
                            goBuyCoinBtn.interactable = true;
                        }
                        else
                        {
                            goBuyCoinBtn.interactable = true;
                        }
                    }

                    //txtDiamondPrice.text = GameManager.Instance.NextChapterDiamondPrice.ToString();
                    //txtCoinPrice.text = GameManager.Instance.NextChapterCoinPrice.ToString();
                }
                else
                {
                    if (currentId > (int)chapters[i].Id)
                    {
                        chapters[i].SetSelected(ChapterMap.Mode.Done, colorDone);
                    }
                    else
                    {
                        chapters[i].SetSelected(ChapterMap.Mode.Inactive, colorInactive);
                    }
                }
            }*/

    }

    public void OpenQuitGameDlg()
    {
        QuitGameDlg.Show();
    }

    [SkipRename]
    public void PrevButtonPress()
    {
        AudioManager.Instance.PlayButtonClick();
        selectedAirfield = Mathf.Max(selectedAirfield - 1, 0);
        SetActiveChapter();

    }
    [SkipRename]
    public void NextButtonPress()
    {
        AudioManager.Instance.PlayButtonClick();
        selectedAirfield = Mathf.Min(selectedAirfield + 1, chapters.Length-1);
        SetActiveChapter();
    }
    [SkipRename]
    public void SelectButtonPress()
    {
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.selectedChapter = (int)chapters[selectedAirfield].Id;
        OpenHome();
    }
}
