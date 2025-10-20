using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Beebyte.Obfuscator;

public class CommonDialog : SC_GUIPanel {
    public GameObject closeReceiver;

    [SkipRename]
	public void Button_Close() {
		AudioManager.Instance.PlayBackButtonClick();
        if (closeReceiver != null)
        {
            closeReceiver.SendMessage("Suljetaan", SendMessageOptions.DontRequireReceiver);
        }
		Hide();
	}

}
