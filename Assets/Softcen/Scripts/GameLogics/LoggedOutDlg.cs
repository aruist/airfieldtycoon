using UnityEngine;
using System.Collections;

public class LoggedOutDlg : MonoBehaviour {

    public void QuitAppButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}
