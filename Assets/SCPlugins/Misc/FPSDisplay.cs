using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class FPSDisplay : MonoBehaviour {
	public Text txtFPS;
	public TextMeshProUGUI txtFPSTMP;
	private int _oldFPS;
	private TextMesh tmFPS;
	void Awake()
	{
		tmFPS = GetComponent<TextMesh>();
#if !SOFTCEN_DEBUG
		Destroy(this.gameObject);
#endif

	}
	
	// Update is called once per frame
	void Update () 
	{
		FPS.ProcessInUpdate();
		DisplayFPS();
	}

	void DisplayFPS()
	{
		int num = (int)Mathf.Ceil ( FPS.fps );
		
		//if ( num  > 60 )
		//	num = 60;
		
		if ( _oldFPS != num )
		{
			_oldFPS = num;
			if (txtFPSTMP != null)
				txtFPSTMP.SetText(num.ToString());
			if (txtFPS != null)
				txtFPS.text = num.ToString();
			if (tmFPS != null)
				tmFPS.text = num.ToString();
		}
	}

}
