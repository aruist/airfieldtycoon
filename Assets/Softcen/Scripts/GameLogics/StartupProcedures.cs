using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupProcedures : MonoBehaviour {
    private static bool m_Initialized = false;

	// Use this for initialization
	void Start () {
        if (!m_Initialized)
        {
            m_Initialized = true;
            SCAnalytics.InitializeGoogleAnalyticsV4();
            SCAnalytics.LogScreen(GameConsts.AnalyticsName + " Launch");

            if (GameManager.Instance.playerData.CurrentChapter == 0)
            {
                SceneManager.LoadScene(GameConsts.Skenes.Map);
            }
            else
            {
                SceneManager.LoadScene(GameConsts.Skenes.Home);
            }
        }
    }

}
