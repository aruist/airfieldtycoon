using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingTxtProgress : MonoBehaviour {
    public float changeTime = 1f;
    public Text txtLoading;
    private float m_timer;
    private int index;
	// Use this for initialization
	void OnEnable () {
        m_timer = 0f;
        index = 0;
        txtLoading.text = "Loading";
    }
	
	// Update is called once per frame
	void Update () {
        m_timer += Time.deltaTime;
        if (m_timer >= changeTime)
        {
            m_timer -= changeTime;
            index++;
            if (index > 3)
                index = 0;
            if (index == 0)
                txtLoading.text = "Loading";
            else if (index == 1)
                txtLoading.text = "Loading.";
            else if (index == 2)
                txtLoading.text = "Loading..";
            else if (index == 3)
                txtLoading.text = "Loading...";
        }
    }
}
