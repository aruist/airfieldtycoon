using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class ArKKuHaLLitSija : MonoBehaviour {
    public static event Action OnIlmainenArkkuChanged;

    public static ArKKuHaLLitSija instance;
    public ArKKu ilmainenArkku;
    public ArKKu[] slots;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad (gameObject);
            for (int i=0; i < slots.Length; i++) {
                slots [i].Tyyppi = ArKKuTyyPPi.tyyppi.TYHJA;
            }
            ilmainenArkku = new ArKKu (ArKKuTyyPPi.tyyppi.ILMAINEN);
        } else {
            Destroy (gameObject);
        }    
    }

    private void OnDisable()
    {
        GameManager.OnGameLoaded -= GameManager_OnGameLoaded;
    }
    private void OnEnable()
    {
        GameManager.OnGameLoaded += GameManager_OnGameLoaded;
    }

    private void GameManager_OnGameLoaded()
    {
        #if SOFTCEN_DEBUG
        Debug.Log("ArKKuHaLLitSija GameManager_OnGameLoaded");
        #endif
        ilmainenArkku.asetaAika(GameManager.Instance.playerData.IlmainenArkkuAvaamisAika);
    }

    public string aika(ArKKuTyyPPi.tyyppi t) {
        if (t == ArKKuTyyPPi.tyyppi.ILMAINEN) {
            return ilmainenArkku.aika ();
        }
        return "";
    }

    public bool voikoAvata(ArKKuTyyPPi.tyyppi t) {
        if (t == ArKKuTyyPPi.tyyppi.ILMAINEN) {
            return ilmainenArkku.voikoAvata ();
        }
        return false;
    }

    public void KaynnistaIlmainenArkku() {
        long uusiAika = ArKKuTyyPPi.uusiAika (ArKKuTyyPPi.tyyppi.ILMAINEN);
        GameManager.Instance.playerData.IlmainenArkkuAvaamisAika = uusiAika;
        ilmainenArkku.asetaAika (uusiAika);
    }

    public void PaivitaAika(ArKKuTyyPPi.tyyppi t, long uusiAika) {
        if (t == ArKKuTyyPPi.tyyppi.ILMAINEN) {
            ilmainenArkku.asetaAika (uusiAika);
            if (OnIlmainenArkkuChanged != null) {
                OnIlmainenArkkuChanged ();
            }
        }
    }
	
}
