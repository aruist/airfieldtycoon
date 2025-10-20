using System;
using UnityEngine;

public class TOS : MonoBehaviour
{
    public static event Action OnTOSHyvaksytty;

    public void AvaaTos()
    {
        Application.OpenURL(M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.ko));
    }

    public void AvaaPrivacy()
    {
        Application.OpenURL(M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.yo));
    }

    public void Hyvaksy()
    {
        if (OnTOSHyvaksytty != null)
        {
            OnTOSHyvaksytty();
        }
    }
    public void Lopeta()
    {
        Application.Quit();
    }
}
