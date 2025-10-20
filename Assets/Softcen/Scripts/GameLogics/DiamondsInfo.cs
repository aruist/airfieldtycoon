using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiamondsInfo : MonoBehaviour {
	public TextMeshProUGUI txtDiamondCount;
    private bool m_UpdateDiamondsPending = false;
    void OnEnable() {
		PlayerData.OnDiamondsChanged += HandleOnDiamondsChanged;
        UpdateDiamondsCount();
    }

    void OnDisable() {
		PlayerData.OnDiamondsChanged -= HandleOnDiamondsChanged;
	}

    void Update()
    {
        if (m_UpdateDiamondsPending)
        {
            UpdateDiamondsCount();
        }
    }

	void HandleOnDiamondsChanged ()
	{
        m_UpdateDiamondsPending = true;
    }

    private void UpdateDiamondsCount()
    {
        m_UpdateDiamondsPending = false;
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.playerData != null)
            {
                txtDiamondCount.SetText(GameManager.Instance.playerData.Diamonds.ToString());
            }
            else
            {
                m_UpdateDiamondsPending = true;
            }
        }
    }
}
