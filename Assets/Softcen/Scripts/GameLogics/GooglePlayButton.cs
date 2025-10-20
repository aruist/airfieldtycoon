using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GooglePlayButton : MonoBehaviour {
    public TextMeshProUGUI txtStatus;

    void Awake()
    {
#if !UNITY_ANDROID
        Destroy(gameObject);
        return;
#endif
    }


#if UNITY_ANDROID
    void OnEnable()
    {
        Pelikeskus.OnStateChanged += Pelikeskus_OnStateChanged;
        Pelikeskus_OnStateChanged();
    }

    void OnDisable()
    {
        Pelikeskus.OnStateChanged -= Pelikeskus_OnStateChanged;
    }

    private void Pelikeskus_OnStateChanged()
    {
        if (Pelikeskus.Instance.Authenticated())
        {
            txtStatus.SetText ("Your are signed in. Log Out");
        }
        else
        {
            txtStatus.SetText ("Your are logged out. Log In");
        }
    }
#endif

    public void ButtonPress()
    {
        if (Pelikeskus.Instance.Authenticated())
        {
            GameManager.Instance.playerData.LoggedOut = true;
            Pelikeskus.Instance.Signout();
        }
        else
        {
            GameManager.Instance.playerData.LoggedOut = false;
            Pelikeskus.Instance.Authenticate();
        }
    }
}
