using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TienistitTestaus : MonoBehaviour
{
    public TextMeshProUGUI textStatus;
    public Button btnInterstitialAdMob;
    public Button btnInterstitialUnity;
    public Button btnRewardAdMob;
    public Button btnRewardUnity;
    public Button btnBannerAdMob;
    public Button btnBannerUnity;

    private bool updateStatusText = false;
    private string updateString;


    private void OnEnable()
    {
        Tienistit.OnRewardVideoCompleted += Tienistit_OnRewardVideoCompleted;
        Tienistit.OnInterstitialStatusChanged += Tienistit_OnInterstitialStatusChanged;
        RefreshPressed();
    }
    private void OnDisable()
    {
        Tienistit.OnRewardVideoCompleted -= Tienistit_OnRewardVideoCompleted;
        Tienistit.OnInterstitialStatusChanged -= Tienistit_OnInterstitialStatusChanged;
    }

    private void Tienistit_OnInterstitialStatusChanged(int id)
    {
        updateString = "Interstitial Video complete, ID: " + id.ToString();
        updateStatusText = true;
    }

    private void Tienistit_OnRewardVideoCompleted(int id, Tienistit.RewardResult result)
    {
        updateString = "Reward Video complete, ID: " + id.ToString() + ", Result: " + result.ToString();
        updateStatusText = true;
    }

    private void Update()
    {
        if (updateStatusText)
        {
            updateStatusText = false;
            textStatus.SetText(updateString);
        }
    }

    public void InterstitialAdMobButtonPressed()
    {
        textStatus.SetText("Show AdMob Interstitial");
        Tienistit.Instance.ShowInterstitial(false, Tienistit.AdOperator.AdMob);
    }

    public void InterstitialUnityButtonPressed()
    {
        textStatus.SetText("Show Unity Interstitial");
        Tienistit.Instance.ShowInterstitial(false, Tienistit.AdOperator.Unity);
    }

    public void InterstitialAny()
    {
        textStatus.SetText("Show Any Interstitial");
        Tienistit.Instance.ShowInterstitial(false);
    }

    public void RewardAdMobButtonPressed()
    {
        textStatus.SetText("Show AdMob Reward...");
        Tienistit.Instance.ShowRewarded(900, Tienistit.AdOperator.AdMob);
    }

    public void RewardUnityButtonPressed()
    {
        textStatus.SetText("Show Unity Reward...");
        Tienistit.Instance.ShowRewarded(901, Tienistit.AdOperator.Unity);
    }

    public void RewardedAny()
    {
        textStatus.SetText("Show Any Reward...");
        Tienistit.Instance.ShowRewarded(902);
    }

    public void ShowUnityBanner()
    {
        textStatus.SetText("Show Unity Banner");
        Tienistit.Instance.ShowBanner(Tienistit.AdOperator.Unity);
    }
    public void ShowAdMobBanner()
    {
        textStatus.SetText("Show AdMob Banner");
        Tienistit.Instance.ShowBanner(Tienistit.AdOperator.AdMob);
    }
    public void piilotaBanneri()
    {
        textStatus.SetText("Hide Banner");
        Tienistit.Instance.HideBanner();
    }

    public void CloseButtonPressed()
    {
        gameObject.SetActive(false);
    }

    public void RefreshPressed()
    {
        btnInterstitialAdMob.interactable = Tienistit.Instance.HasInterstitial(Tienistit.AdOperator.AdMob);
        btnInterstitialAdMob.enabled = Tienistit.Instance.HasInterstitial(Tienistit.AdOperator.AdMob);
        btnInterstitialUnity.interactable = Tienistit.Instance.HasInterstitial(Tienistit.AdOperator.Unity);
        btnInterstitialUnity.enabled = Tienistit.Instance.HasInterstitial(Tienistit.AdOperator.Unity);
        btnRewardAdMob.interactable = Tienistit.Instance.HasRewarded(Tienistit.AdOperator.AdMob);
        btnRewardAdMob.enabled = Tienistit.Instance.HasRewarded(Tienistit.AdOperator.AdMob);
        btnRewardUnity.interactable = Tienistit.Instance.HasRewarded(Tienistit.AdOperator.Unity);
        btnRewardUnity.enabled = Tienistit.Instance.HasRewarded(Tienistit.AdOperator.Unity);
    }

}
