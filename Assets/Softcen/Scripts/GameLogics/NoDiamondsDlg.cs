using UnityEngine;
using Beebyte.Obfuscator;

public class NoDiamondsDlg : MonoBehaviour
{
    [SkipRename]
    public void OpenShopDlg()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        SceneActions.OpenShopDlg();
        GetComponent<CommonDialog>().Button_Close();
    }
}
