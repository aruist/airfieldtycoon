using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beebyte.Obfuscator;

public class MissionDlg : MonoBehaviour {

    [SkipRename]
    public void TuplaaBonusNappi() {
        if (MissionManager.Instance != null) {
            MissionManager.Instance.MissionDlgDoubleBonus ();
            gameObject.SetActive (false);
        }
    }

    [SkipRename]
    public void HyvaksyNappi() {
        if (MissionManager.Instance != null) {
            MissionManager.Instance.MissionDlgAccept ();
            gameObject.SetActive (false);
        }
    }

    [SkipRename]
    public void Ok() {
        GetComponent <CommonDialog>().Button_Close ();
    }

}
