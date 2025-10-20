using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_IOS && !UNITY_EDITOR
using Unity.Advertisement.IosSupport;
#endif

public class AloitusTarkastukset : MonoBehaviour
{
    /*
     * 1. Check genuine
     * 2. iOS: Check App Tracking Transparency
     * 3. Terms of Service
     * 4. Load Game
     */
    public GameObject goGenuineDialog;
    public GameObject goTOSDialog;
    public GameObject goLoadingDialog;
    public static bool aloitusSuoritettu = false;
#if UNITY_IOS && !UNITY_EDITOR
    private ATTrackingStatusBinding.AuthorizationTrackingStatus m_PreviousStatus;
    private ATTrackingStatusBinding.AuthorizationTrackingStatus m_CurrentStatus;
    private bool m_Once;
#endif

    private void OnEnable()
    {
        TOS.OnTOSHyvaksytty += TOS_OnTOSHyvaksytty;
    }

    private void OnDisable()
    {
        TOS.OnTOSHyvaksytty -= TOS_OnTOSHyvaksytty;
    }

    private void TOS_OnTOSHyvaksytty()
    {
        Agree();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (goTOSDialog != null)
            goTOSDialog.SetActive(false);
        if (goLoadingDialog != null)
            goLoadingDialog.SetActive(false);
        if (goGenuineDialog != null)
            goGenuineDialog.SetActive(false);
        // 1. Check genuine
        if (Application.genuineCheckAvailable)
        {
            bool genuine = Application.genuine;
            #if SOFTCEN_DEBUG
            Debug.Log("genuineChec " + genuine);
            #endif
            if (!genuine)
            {
                if (goGenuineDialog != null)
                {
                    goGenuineDialog.SetActive(true);
                }
                else
                {
                    Application.Quit();
                }
                return;
            }
        }

        // 2. iOS: Check App Tracking Transparency
        #if UNITY_IOS && !UNITY_EDITOR
        m_Once = false;
        m_PreviousStatus = ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED;
        ATTrackingStatusBinding.AuthorizationTrackingStatus status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        #if SOFTCEN_DEBUG
        Debug.Log("AuthorizationTrackingStatus: " + status.ToString());
        #endif
        if(status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            #if SOFTCEN_DEBUG
            Debug.Log("RequestAuthorizationTracking");
            #endif
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        } 
        else
        {
            // 3. Terms of Service
            TermsOfServiceCheck();
        }
        #else
        // 3. Terms of Service
        TermsOfServiceCheck();
        #endif
    }

    public void GeniueQuit()
    {
        Application.Quit();
    }

    private void TermsOfServiceCheck()
    {
#if SOFTCEN_DEBUG
        Debug.Log("TermsOfServiceCheck()");
#endif
        if (PlayerPrefs.HasKey("TOS_Agreed"))
        {
            LoadGame();
        }
        else
        {
            if (goTOSDialog != null)
                goTOSDialog.SetActive(true);
        }
    }

#if UNITY_IOS && !UNITY_EDITOR
    private void Update()
    {
        if (!m_Once)
        {
            m_CurrentStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            if (m_PreviousStatus != m_CurrentStatus)
            {
#if SOFTCEN_DEBUG
                Debug.Log("AuthorizationTrackingStatus: " + m_CurrentStatus.ToString());
#endif
                m_Once = true;
                TermsOfServiceCheck();
            }
        }
    }
#endif

    public void Agree()
    {
#if SOFTCEN_DEBUG
        Debug.Log("Agree()");
#endif
        PlayerPrefs.SetInt("TOS_Agreed", 1);
        PlayerPrefs.Save();
        LoadGame();
        gameObject.SetActive(false);
    }

    private void LoadGame()
    {
        if (goTOSDialog != null)
            goTOSDialog.SetActive(false);
        if (goGenuineDialog != null)
            goGenuineDialog.SetActive(false);
        if (goLoadingDialog != null)
            goLoadingDialog.SetActive(true);
        string skeneName = GameConsts.Skenes.Loader;
#if SOFTCEN_DEBUG
        Debug.Log("LoadGame: " + skeneName);
#endif
        aloitusSuoritettu = true;
        SceneManager.LoadScene(skeneName);
    }
}
