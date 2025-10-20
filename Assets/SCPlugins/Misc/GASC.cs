using UnityEngine;
using System.Collections;

public class GASC : MonoBehaviour {
#if USE_GA
	public bool awakeStartSession = false;
	public string awakeLogScreen = "";

	// Use this for initialization
	void Start () {
		if (GoogleAnalyticsV4.instance == null)
			return;

		if (awakeStartSession) {
			GoogleAnalyticsV4.instance.bundleVersion = PygmyMonkey.AdvancedBuilder.AppParameters.Get.bundleVersion;
			GoogleAnalyticsV4.instance.StartSession();
		}	

		if (awakeLogScreen != "") {
			GoogleAnalyticsV4.instance.LogScreen(awakeLogScreen);
		}
	}
#endif
}
