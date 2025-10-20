using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Beebyte.Obfuscator;
using TMPro;

public class RewardEvent : MonoBehaviour {
    public TextMeshProUGUI m_Title;
    public TextMeshProUGUI m_Description;
	public CommonDialog m_CommonDialog;
	public GameObject goIdle;
	public GameObject goTapBoost;
	public GameObject goInstantBoost;
    public GameObject goAutoTap;
    public GameObject goDoubleBonus;
    void OnEnable()
    {

    }

    private void SetGameObject(GameObject go, bool state)
    {
        if (go != null)
        {
            if (go.activeSelf != state)
                go.SetActive(state);
        }
    }

	public void Show(string dlgName, EventData eventData, DialogManager dlgManager) {
		if (eventData.Type == EventManager.boostType.InstantTap) {
            SetGameObject(goIdle, false);
            SetGameObject(goTapBoost, false);
            SetGameObject(goInstantBoost, true);
            SetGameObject(goAutoTap, false);
            SetGameObject (goDoubleBonus, false);
        }
        else if (eventData.Type == EventManager.boostType.IdleBoost) {
            SetGameObject(goIdle, true);
            SetGameObject(goTapBoost, false);
            SetGameObject(goInstantBoost, false);
            SetGameObject(goAutoTap, false);
            SetGameObject (goDoubleBonus, false);
        }
        else if (eventData.Type == EventManager.boostType.TapBoost) {
            SetGameObject(goIdle, false);
            SetGameObject(goTapBoost, true);
            SetGameObject(goInstantBoost, false);
            SetGameObject(goAutoTap, false);
            SetGameObject (goDoubleBonus, false);
        }
        else if (eventData.Type == EventManager.boostType.AutoTap)
        {
            SetGameObject(goIdle, false);
            SetGameObject(goTapBoost, false);
            SetGameObject(goInstantBoost, false);
            SetGameObject(goAutoTap, true);
            SetGameObject (goDoubleBonus, false);
        } 
        else if (eventData.Type == EventManager.boostType.DoubleBonus) {
            SetGameObject(goIdle, false);
            SetGameObject(goTapBoost, false);
            SetGameObject(goInstantBoost, false);
            SetGameObject(goAutoTap, false);
            SetGameObject (goDoubleBonus, true);
        }

        m_Title.SetText (eventData.Name);
        m_Description.SetText (eventData.Description);
		dlgManager.OpenDialog(dlgName);
        dlgManager.CheckDialogs();
		//m_CommonDialog.Show();
	}

	public void Show(string dlgName, string description, DialogManager dlgManager) {
		if (m_Description != null) {
            m_Description.SetText (description);
		}
        dlgManager.OpenDialog(dlgName);
        dlgManager.CheckDialogs();
	}


    public void RewardAccepted(string dlgName, EventData eventData, DialogManager dlgManager)
    {
        Show(dlgName, eventData, dlgManager);
    }

    [SkipRename]
    public void Button_Close() {
		AudioManager.Instance.PlayButtonClick();
		m_CommonDialog.Hide();
	}
    [SkipRename]
	public void AcceptWatchAd() {
		AudioManager.Instance.PlayButtonClick();
        //m_CommonDialog.Hide();
        gameObject.SetActive(false);
        HomeManager.Instance.ShowRewardVideo();
    }

}
