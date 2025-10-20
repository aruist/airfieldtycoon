using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevChapterText : MonoBehaviour {
    public Text txtChapter;
    public TextMeshProUGUI txtTMP;

    private GameManager gm;

	// Use this for initialization
	void Start () {
        gm = GameManager.Instance;
        UpdateText();
    }

    void OnEnable()
    {
        PlayerData.OnLevelChanged += PlayerData_OnLevelChanged;
    }
    void OnDisable()
    {
        PlayerData.OnLevelChanged -= PlayerData_OnLevelChanged;
    }

    private void PlayerData_OnLevelChanged()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (txtChapter != null)
        {
            txtChapter.text = "Chapter: " + gm.playerData.CurrentChapter.ToString()
                + ", Phase: " + gm.playerData.CurrentPhase.ToString();
        }
        if (txtTMP != null)
        {
            txtTMP.SetText("Chapter: " + gm.playerData.CurrentChapter.ToString()
                + ", Phase: " + gm.playerData.CurrentPhase.ToString());
        }
    }
	
}
