using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelChange : MonoBehaviour {
    public TextMeshProUGUI txtChapterName;
    public TextMeshProUGUI txtLevel;

	// Use this for initialization
	void OnEnable ()
    {
        #if SOFTCEN_DEBUG
        Debug.Log("LevelChange OnEnable()");
        #endif
        txtChapterName.SetText (BonusManager.Instance.GetChapterName(GameManager.Instance.selectedChapter));
        txtLevel.SetText ("LEVEL " + GameManager.Instance.playerData.Level.ToString());
	}
	
}
