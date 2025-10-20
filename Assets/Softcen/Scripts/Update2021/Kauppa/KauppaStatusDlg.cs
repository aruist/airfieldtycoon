using TMPro;
using UnityEngine;

public class KauppaStatusDlg : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI tmpDescription;

    public void ShowDialog(string message) {
        #if KAUPPA_DEBUG
        Debug.Log($"KauppaStatusDlg ShowDialog({message})");
        #endif
        tmpDescription.SetText(message);
        gameObject.SetActive(true);
    }
}
