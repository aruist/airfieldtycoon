using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Beebyte.Obfuscator;

public class IlmainenArkku : MonoBehaviour {
    private float mTimer = 0f;
    public TextMeshProUGUI arkkuAika;
    private ArKKuHaLLitSija ah;
    private bool timerRun;
    public Button btn;
    public GameObject goParticle;
    public AudioClip avaamisAudio;

    private bool m_Inialized = false;

    void OnEnable () {
        if (m_Inialized) {
            Init ();
        }
        ArKKuHaLLitSija.OnIlmainenArkkuChanged += ArKKuHaLLitSija_OnIlmainenArkkuChanged;
	}

    void OnDisable() {
        ArKKuHaLLitSija.OnIlmainenArkkuChanged -= ArKKuHaLLitSija_OnIlmainenArkkuChanged;
    }

    void ArKKuHaLLitSija_OnIlmainenArkkuChanged ()
    {
        paivitaAika ();
    }

    void Start() {
        Init ();
    }

    private void Init() {
        m_Inialized = true;
        timerRun = true;
        ah = ArKKuHaLLitSija.instance;
        paivitaAika ();
        if (timerRun) {
            goParticle.SetActive (false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!timerRun)
            return;
        mTimer += Time.deltaTime;
        if (mTimer >= 1f) {
            mTimer = mTimer - 1f;
            paivitaAika ();
        }
	}

    private void paivitaAika() {
        if (ah == null)
            return;
        arkkuAika.SetText (ah.aika (ArKKuTyyPPi.tyyppi.ILMAINEN));
        if (ah.voikoAvata (ArKKuTyyPPi.tyyppi.ILMAINEN)) {
            timerRun = false;
            btn.interactable = true;
            goParticle.SetActive (true);
            AudioManager.Instance.PlayClip (avaamisAudio);
        } else {
            timerRun = true;
            btn.interactable = false;
        }
    }

    [SkipRename]
    public void avaaArkku() {
        AudioManager.Instance.PlayButtonClick();
        goParticle.SetActive (false);
        HomeManager.Instance.m_DialogManager.OpenDialog ("FreeChestDlg");
        timerRun = true;
        btn.interactable = false;
        mTimer = 0;
        ArKKuHaLLitSija.instance.KaynnistaIlmainenArkku ();
    }

}
