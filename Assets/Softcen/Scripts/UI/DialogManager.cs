using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogManager : MonoBehaviour {
    public GameObject[] goFullScreenDisable;
	public GameObject[] m_Dialogs;
    public MainMenuButton mainMenuButton;
    //private bool m_FullScreenActive = false;
	//public GraphicRaycaster m_MainGraphicRaycaster;

	// Use this for initialization
	void Awake () {
		CloseAllDialogs();
	}

	public void OpenDialog(string name) {
		GameObject go = GetDialog(name);
		if (go != null) {
			/*
			DialogControl dctrl = go.GetComponent<DialogControl>();
			if (dctrl != null) {
				m_MainGraphicRaycaster.enabled = false;
				GameManager.Instance.tapDisabled = true;
				go.SetActive(true);
				return;
			}*/
			CommonDialog cd = go.GetComponent<CommonDialog>();
			if (cd != null) {
				cd.m_DialogManager = this;
				//m_MainGraphicRaycaster.enabled = false;
				GameManager.Instance.tapDisabled = true;
				go.SetActive(true);
				cd.Show();
				return;
			}
		}
#if SOFTCEN_DEBUG
        Debug.Log("<color=yellow>DialogManager OpenDialog DIALOG NOT FOUND: </color>" + name);
#endif
    }

	public void CloseDialog(string name) {
		CloseDialog(GetDialog(name));
	}

    public void ToggleDialog(string name)
    {
        GameObject go = GetDialog(name);
        if (go.activeSelf)
        {
            CloseDialog(name);
        }
        else
        {
            OpenDialog(name);
        }
    }

	public void CloseDialog(GameObject go) {
		if (go != null) {
			/*
			DialogControl dctrl = go.GetComponent<DialogControl>();
			if (dctrl != null) {
				dctrl.CloseDialog();
				return;
			}*/
			CommonDialog cd = go.GetComponent<CommonDialog>();
			if (cd != null) {
#if SOFTCEN_DEBUG
                //Debug.Log("DialogManager CloseDialog " + name + " <CommonDialog>");
#endif
                cd.Button_Close();
				return;
			}
		}
#if SOFTCEN_DEBUG
        // Debug.LogWarning("DialogManager CloseDialog DIALOG NOT FOUND " + go.name);
#endif
    }

	public void DialogClosed() {
		CheckDialogs();
	}

	public GameObject GetDialog(string name) {
		for (int i=0; i < m_Dialogs.Length; i++) {
			if (m_Dialogs[i].name == name) {
				return m_Dialogs[i];
			}	
		}
		return null;
	}

	public void CloseAllDialogs() {
		for (int i=0; i < m_Dialogs.Length; i++) {
            if (m_Dialogs[i] != null)
            {
                if (m_Dialogs[i].activeSelf == true)
                {
                    m_Dialogs[i].SetActive(false);
                }
            }
        }
	}

	public void CheckDialogs() {
        if (mainMenuButton != null)
            mainMenuButton.CheckMainMenuButtonImage();

        if (AnyDialogOpen()) {
			//m_MainGraphicRaycaster.enabled = false;
			GameManager.Instance.tapDisabled = true;
		}
		else {
			//m_MainGraphicRaycaster.enabled = true;
			GameManager.Instance.tapDisabled = false;
		}
		// Debug.Log("DialogManager CheckDialogs " + m_MainGraphicRaycaster.enabled + ", " + GameManager.Instance.tapDisabled);
	}

	public bool AnyDialogOpen() {
		for (int i=0; i < m_Dialogs.Length; i++)
        {
            if (m_Dialogs[i] != null)
            {
                if (m_Dialogs[i].activeSelf == true)
                {
                    return true;
                }
            }
        }
        return false;
	}

	public bool CheckEscape() {
		for (int i= m_Dialogs.Length-1; i >= 0; i--) {
			if (m_Dialogs[i].activeSelf == true) {
				CloseDialog(m_Dialogs[i]);
				return true;
			}	
		}
		return false;
	}

    public void FullScreenDialogActive()
    {
        Debug.Log("FullScreenDialogActive");
        //m_FullScreenActive = true;
        for (int i=0; i < goFullScreenDisable.Length; i++)
        {
            if (goFullScreenDisable[i] != null)
                goFullScreenDisable[i].SetActive(false);
        }
        if (HomeManager.Instance != null)
            HomeManager.Instance.SetLevelActive(false);

    }

    public void FullScreenDialogClose()
    {
        Debug.Log("FullScreenDialogClose");
        //m_FullScreenActive = false;
        for (int i = 0; i < goFullScreenDisable.Length; i++)
        {
            if (goFullScreenDisable[i] != null)
                goFullScreenDisable[i].SetActive(true);
        }
        if (HomeManager.Instance != null)
            HomeManager.Instance.SetLevelActive(true);
    }

}
