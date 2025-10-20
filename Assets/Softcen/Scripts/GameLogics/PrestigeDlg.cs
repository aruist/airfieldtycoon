using UnityEngine;
using UnityEngine.UI;
using Beebyte.Obfuscator;
using TMPro;

public class PrestigeDlg : MonoBehaviour {
    public GameObject goYesBtn;
    public GameObject goNoBtn;
    public GameObject goLocked;
    public TextMeshProUGUI txtBonus;
    public MainMenuButton mainMenuButton;

    void OnEnable()
    {
        int currentLevel = GameManager.Instance.playerData.Level;
        int prestigeLevel = GameManager.Instance.playerData.PrestigeLevel;
        if (currentLevel > 1 || prestigeLevel > 0)
        {
            int lvl = GameManager.Instance.playerData.Level - 1;
            if (prestigeLevel > lvl)
                lvl = prestigeLevel;

            txtBonus.SetText ("30% bonus up to level " + lvl.ToString());
            goNoBtn.SetActive(true);
            goYesBtn.SetActive(true);
            goLocked.SetActive(false);
        }
        else
        {
            txtBonus.SetText ("30% bonus up to current level");
            goNoBtn.SetActive(false);
            goYesBtn.SetActive(false);
            goLocked.SetActive(true);
        }
    }

    [SkipRename]
    public void NoPrestige()
    {
        AudioManager.Instance.PlayButtonClick();
        GetComponent<CommonDialog>().Button_Close();
    }

    [SkipRename]
    public void YesPrestige()
    {
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.Prestige();
        GetComponent<CommonDialog>().Button_Close();
        mainMenuButton.FinishPrestige();
    }
}
