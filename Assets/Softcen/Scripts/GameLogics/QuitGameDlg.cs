using UnityEngine;
using Beebyte.Obfuscator;

public class QuitGameDlg : MonoBehaviour {

    [SkipRename]
    public void YesButtonPress()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitButtonPressed(true);
            GameManager.Instance.Save();
        }
        if (Tienistit.Instance != null)
        {
            Tienistit.Instance.TuhoaMainokset();
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		//Application.Quit();
#endif

    }

    [SkipRename]
    public void NoButtonPress()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        GetComponent<CommonDialog>().Button_Close();
    }
}
