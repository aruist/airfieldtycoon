using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoTapDlg : MonoBehaviour {

    public TextMeshProUGUI txtAutoTapTryBtn;
    public TextMeshProUGUI txtAutoTapPrice;
	// Use this for initialization
	void Start () {
        txtAutoTapTryBtn.SetText ("Try for " + GameConsts.AutoTap.TryTime.ToString("F0") + " seconds!");
    }

    void OnEnable()
    {
        string price = "-";
        if (Kauppa.Instance != null)
        {
            price = Kauppa.Instance.Tuotteenhinta(Kauppa.ID.INN_APP_AUTO_TAP);
        }
        txtAutoTapPrice.SetText (price);
    }

}
