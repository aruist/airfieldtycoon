using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopPanel : MonoBehaviour {
    public TextMeshProUGUI txtHarborAndLevel;
    public TextMeshProUGUI txtScore;

    private bool m_Initialized = false;
	// Use this for initialization
	void Start () {
        UpdateTexts();
        UpdateScore();
        m_Initialized = true;
    }

    void OnEnable()
    {
        PlayerData.OnLevelChanged += PlayerData_OnLevelChanged;
        PlayerData.OnScoreChanged += PlayerData_OnScoreChanged;
        if (m_Initialized)
        {
            UpdateTexts();
            UpdateScore();
        }
    }

    private void PlayerData_OnScoreChanged()
    {
        UpdateScore();
    }

    private void PlayerData_OnLevelChanged()
    {
        UpdateTexts();
    }

    void OnDisable()
    {
        PlayerData.OnLevelChanged -= PlayerData_OnLevelChanged;
        PlayerData.OnScoreChanged -= PlayerData_OnScoreChanged;
    }

    private void UpdateTexts()
    {
        string str = "";
        if (GameManager.Instance != null)
            str += "LEVEL: " + GameManager.Instance.playerData.Level.ToString() + "\n";
        else
            str = "\n";

        if (BonusManager.Instance != null)
            str += BonusManager.Instance.GetChapterName(GameManager.Instance.selectedChapter);
        else
            str += "";
        txtHarborAndLevel.SetText(str);
    }

    private void UpdateScore()
    {
        txtScore.SetText("SCORE: " + GameManager.Instance.playerData.Score.ToString());
    }

}
