#if EI_KAYTOSSA
using UnityEngine;
using System.Collections;
using SoftcenSDK;

public class SceneAds : MonoBehaviour {
	public bool showInterestial;
	public bool showBanner;
	public bool hideBanner;

	// Use this for initialization
	void Start () {
		if (showInterestial) {
			AdManager.Instance.ShowInterstitial();
		}
		if (hideBanner) {
			AdManager.Instance.HideBanner();
		}
		else if (showBanner) {
			AdManager.Instance.ShowBanner();
		}
	}
	
}
#endif