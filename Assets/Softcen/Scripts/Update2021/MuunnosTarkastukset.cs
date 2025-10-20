using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuunnosTarkastukset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID && SOFTCEN_DEBUG
        // Check Admob IDs
        string strResult = "GameManager <color=yellow>";
        if (GameConsts.amhix2wb == "ca-app-pub-7159985667273944/6973783119")
        {
            strResult += "AdMob Banner OK!";
        }
        else
        {
            strResult += "AdMob Banner FAIL!";
        }
        if (GameConsts.amhix2wi == "ca-app-pub-7159985667273944/7972838310")
        {
            strResult += ", AdMob Interstitial OK!";
        }
        else
        {
            strResult += ", AdMob Interstitial Fail!";
        }
        strResult += "</color>";
        Debug.Log(strResult);
#endif
#if UNITY_IOS && SOFTCEN_DEBUG
        // Check Admob IDs
        string strResult = "<color=yellow>";
        if (GameConsts.amhix2wb == "ca-app-pub-7159985667273944/1926304716")
        {
            strResult += "AdMob Banner OK!";
        }
        else
        {
            strResult += "AdMob Banner FAIL!";
        }
        if (GameConsts.amhix2wi == "ca-app-pub-7159985667273944/3403037919")
        {
            strResult += ", AdMob Interstitial OK!";
        }
        else
        {
            strResult += ", AdMob Interstitial Fail!";
        }
        strResult += "</color>";
        Debug.Log(strResult);
#endif

    }

}
