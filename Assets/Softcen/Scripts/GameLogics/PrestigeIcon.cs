using UnityEngine;
using TMPro;

public class PrestigeIcon : MonoBehaviour {
    public TextMeshProUGUI txtPrestigeLevel;

    void OnEnable()
    {
        if (GameManager.Instance != null)
            txtPrestigeLevel.SetText("LVL " + GameManager.Instance.playerData.PrestigeLevel.ToString());
    }
}
