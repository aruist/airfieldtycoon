using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemShopHeader : MonoBehaviour {
    public TextMeshProUGUI txtHeader;
    public TextMeshProUGUI txtSubHeader;
    public int chapterId;
    public CanvasGroup canvasGroup;

    private bool m_Initialized = false;

    void Start()
    {
        m_Initialized = true;
        CheckActiveStatus();
    }

    void OnEnable()
    {
        CheckActiveStatus();
    }

    private void CheckActiveStatus()
    {
        if (GameManager.Instance != null && m_Initialized == true)
        {
            if (chapterId > GameManager.Instance.playerData.CurrentChapter)
            {
                canvasGroup.alpha = 0.8f;
            }
            else
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
}
