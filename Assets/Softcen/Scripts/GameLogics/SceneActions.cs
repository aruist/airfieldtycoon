using System;

public class SceneActions
{
    public static event Action OnOpenShopDlg;

    public static void OpenShopDlg()
    {
        if (OnOpenShopDlg != null )
        {
            OnOpenShopDlg();
        }
    }
}


