using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Beebyte.Obfuscator;

public class IlmainenArkkuDlg : MonoBehaviour {

    private int m_TimanttiLuku;
    private double m_Kolikoita;
    private bool m_lunastettu;

    public TextMeshProUGUI timantteja;
    public TextMeshProUGUI kolikoita;
    public GameObject goTuplaaNappi;

	// Use this for initialization
	void OnEnable () {
        m_lunastettu = false;
        double idle = GameManager.Instance.playerData.GetIdleValue ();
        double tap = GameManager.Instance.playerData.GetTapValue();
        float rnd = Random.Range (10f, 15f);
        m_Kolikoita = rnd * 60 * (idle + tap);
        int level = GameManager.Instance.playerData.Level;
        if (level < 9)
            m_TimanttiLuku = Random.Range (1,3);
        else if (level < 20)
            m_TimanttiLuku = Random.Range (1,4);
        else
            m_TimanttiLuku = Random.Range (2,4);
        
        timantteja.SetText (m_TimanttiLuku.ToString ());
        kolikoita.SetText(NumToStr.GetNumStr(m_Kolikoita));
        if (Tienistit.Instance != null && Tienistit.Instance.HasRewarded()) {
            goTuplaaNappi.SetActive (true);
        } else {
            goTuplaaNappi.SetActive (false);
        }
	}
	
    [SkipRename]
    public void hyvaksyPalkinto() {
        LunastaPalkinto ();
        GetComponent<CommonDialog> ().Button_Close ();
    }

    private void LunastaPalkinto() {
        AudioManager.Instance.PlayButtonClick ();
        m_lunastettu = true;
        GameManager.Instance.playerData.ChangeDiamonds (m_TimanttiLuku);
        GameManager.Instance.playerData.IncMoney (m_Kolikoita);
        GameManager.Instance.Save ();
    }

    [SkipRename]
    void Suljetaan() {
        Debug.Log ("Suljetaan");
        if (!m_lunastettu) {
            LunastaPalkinto ();
        }
    }

    [SkipRename]
    public void tuplaa() {
        AudioManager.Instance.PlayButtonClick ();
        m_lunastettu = true;
        gameObject.SetActive (false);
        HomeManager.Instance.TuplaaIlmainenArkku (m_Kolikoita, m_TimanttiLuku);
    }

}
