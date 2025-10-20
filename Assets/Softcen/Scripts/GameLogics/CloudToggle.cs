using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CloudToggle : MonoBehaviour {
    public Toggle cloudToggle;

    void Awake()
    {
#if UNITY_ANDROID
        Destroy(gameObject);
#endif
    }

#if UNITY_IOS
    void OnEnable()
    {
        if (GameManager.Instance.playerData.CloudEnabled)
            cloudToggle.isOn = true;
        else 
        {
            GameManager.Instance.checkCloud = true;
            cloudToggle.isOn = false;
        }
    }
    void OnDisable()
    {
        if (GameManager.Instance.checkCloud == true)
        {
            GameManager.Instance.checkCloud = false;
            if (GameManager.Instance.playerData.CloudEnabled)
            {
                Pilvipalvelut.Instance.Syncronize();
                // GameManager.Instance.ReadCloud();
            }
        }
    }
#endif


    public void OnToggleChanged()
    {
        GameManager.Instance.playerData.CloudEnabled = cloudToggle.isOn;
    }
}
