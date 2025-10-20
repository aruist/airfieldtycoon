using UnityEngine;
using UnityEngine.UI;
#if SOFTCEN_DEBUG
using System;
#endif
using Beebyte.Obfuscator;
using TMPro;
using VoxelBusters.EssentialKit;

public class DevOptions : MonoBehaviour {
    #if SOFTCEN_DEBUG
    public static event Action OnReset;
    #endif
    // Dev Only features
    public Text devLevelTxt;
    public TextMeshProUGUI devLevelTMP;

    private int devLevel = 0;
    public Toggle toggleSkipMoney;

    public TextMeshProUGUI txtPhaseValueTMP;
    public TextMeshProUGUI txtChapterValueTMP;
    public LevelResources levelResources;

    private int chapterValue = 1;
    private int phaseValue = 1;

    void Awake() {
        #if !SOFTCEN_DEBUG
        gameObject.SetActive(false);
        #endif
    }

    void OnEnable()
    {
        #if SOFTCEN_DEBUG
        if (toggleSkipMoney != null)
            toggleSkipMoney.isOn = GameManager.Instance.dev_SkipMoney;
        UpdatePhaseValue();
        #endif
    }

    void Start() {
        if (devLevelTxt != null)
            devLevelTxt.text = devLevel.ToString();
    }

    [SkipRename]
    public void ResetButton() {
        #if SOFTCEN_DEBUG
        HomeManager.Instance.ResetButton();
        if (OnReset != null)
            OnReset();

        #endif
    }

    public void RateGame()
    {
        Utilities.RequestStoreReview();
    }

    [SkipRename]
    public void DevIncLevel() {
        devLevel = Mathf.Min(devLevel+1, GameConsts.maxLevel);
        if (devLevelTxt != null)
            devLevelTxt.text = devLevel.ToString();
    }

    [SkipRename]
    public void DevDecLevel() {
        devLevel = Mathf.Max(devLevel-1, 0);
        if (devLevelTxt != null)
            devLevelTxt.text = devLevel.ToString();
    }


    public void DevSetLevel() {
        #if SOFTCEN_DEBUG
        GameManager gm = GameManager.Instance;
        if (devLevel == 0) {
            ResetButton();
            return;
        }
        GameManager.Instance.playerData.SetLevel(devLevel);
        GameManager.Instance.playerData.SetSubLevel(0);
        //GameManager.Instance.playerData.SetMoney(1000);
        HomeManager.Instance.ResetButton();
        gm.Save();
        #endif
    }

    public void SetLevelTo()
    {
#if SOFTCEN_DEBUG
        GameManager.Instance.playerData.SetLevel(devLevel);
        GameManager.Instance.playerData.CallOnLevelChanged();
#endif
    }

    [SkipRename]
    public void ToggleSkipMoney()
    {
#if SOFTCEN_DEBUG
        if (toggleSkipMoney != null)
            GameManager.Instance.dev_SkipMoney = toggleSkipMoney.isOn;
#endif
    }

    [SkipRename]
    public void DevChartboostButton()
    {
#if SOFTCEN_DEBUG
        Tienistit.Instance.ShowRewarded(55555);
        //hillo.Instance.showRewardVideo(hillo.InterstitialOperators.Chartboost, 55555);
#endif
    }
    [SkipRename]
    public void DevUnityAdsButton()
    {
#if SOFTCEN_DEBUG
        Tienistit.Instance.ShowRewarded(55555, Tienistit.AdOperator.AdMob);
        //hillo.Instance.showRewardVideo(hillo.InterstitialOperators.UnityAds, 55555);
#endif
    }
    [SkipRename]
    public void DevApplovinButton()
    {
#if SOFTCEN_DEBUG
        //hillo.Instance.showRewardVideo(hillo.InterstitialOperators.Applovin, 55555);
#endif
    }
    [SkipRename]
    public void DisableBannerAds()
    {
#if SOFTCEN_DEBUG
        Tienistit.Instance.HideBanner();
        //hillo.Instance.HideBanner ();
        #endif
    }
    [SkipRename]
    public void DevGoogleButton()
    {
#if SOFTCEN_DEBUG
        Tienistit.Instance.ShowRewarded(55555, Tienistit.AdOperator.AdMob);
        //hillo.Instance.showRewardVideo(hillo.InterstitialOperators.AdMob, 55555);
#endif
    }

    public void PhaseValueChange(int val)
    {
#if SOFTCEN_DEBUG
        phaseValue += val;
        if (phaseValue < 0)
            phaseValue = 0;
        if (phaseValue > 5)
            phaseValue = 5;
        UpdatePhaseValue();
#endif
    }
    private void UpdatePhaseValue()
    {
        txtPhaseValueTMP.SetText(phaseValue.ToString());
        txtChapterValueTMP.SetText(chapterValue.ToString());
        //txtPhaseValue.text = phaseValue.ToString();
        //txtChapterValue.text = chapterValue.ToString();
    }
    public void ChangePhase()
    {
#if SOFTCEN_DEBUG
        gameObject.SetActive(false);
        levelResources.test_phasePos = phaseValue;
        levelResources.test_changePos = true;
#endif
    }

    public void ChapterValueChange(int val)
    {
#if SOFTCEN_DEBUG
        chapterValue += val;
        if (chapterValue < 1)
            chapterValue = 1;
        UpdatePhaseValue();
#endif
    }
    public void ChangeChapter()
    {
#if SOFTCEN_DEBUG
        gameObject.SetActive(false);
        levelResources.test_Chapter = chapterValue;
        levelResources.test_changeChapter = true;
#endif
    }

    public void CloseDebug()
    {
        gameObject.SetActive(false);
    }


}
