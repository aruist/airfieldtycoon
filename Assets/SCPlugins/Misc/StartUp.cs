using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartUp : MonoBehaviour {
    private bool scubscribeOnSynchronizeReady = false;

    private void OnEnable()
    {
        scubscribeOnSynchronizeReady = true;
        Pilvipalvelut.OnSynchronizeReady += Pilvipalvelut_OnSynchronizeReady;
    }

    private void OnDisable()
    {
        if (scubscribeOnSynchronizeReady)
        {
            scubscribeOnSynchronizeReady = false;
            Pilvipalvelut.OnSynchronizeReady -= Pilvipalvelut_OnSynchronizeReady;
        }
    }

    private void Pilvipalvelut_OnSynchronizeReady()
    {
        #if SOFTCEN_DEBUG
        Debug.Log("StartUp Pilvipalvelut_OnSynchronizeReady");
        #endif
        LoadScene();
    }

    private void LoadScene() {
        Scene scene = SceneManager.GetActiveScene();
        OnDisable();
        string skeneName;
        if (GameManager.Instance.playerData.CurrentChapter == 0)
        {
            skeneName = GameConsts.Skenes.Map;
        }
        else
        {
            skeneName = GameConsts.Skenes.Home;
        }
#if SOFTCEN_DEBUG
        Debug.Log("StartUp LoadScene current scene: " + scene.name + ", load scene: " + skeneName);
#endif
        SceneManager.LoadSceneAsync(skeneName);
    }
	
    public void QuitButton()
    {
        #if SOFTCEN_DEBUG
        Debug.Log("App QUIT");
        #endif
        Application.Quit();
    }

    public void ReInstallButton()
    {
        if (!string.IsNullOrEmpty(GameConsts.Links.RateGame))
            Application.OpenURL(GameConsts.Links.RateGame);
    }

}
