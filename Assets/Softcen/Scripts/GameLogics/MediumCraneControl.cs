using UnityEngine;
using System.Collections;

public class MediumCraneControl : MonoBehaviour {
    #if SOFTCEN_DEBUG
    private Animator m_anim;
    #endif
    public bool m_Test;
	// Use this for initialization
	void Start () {
    #if SOFTCEN_DEBUG
       m_anim = GetComponent<Animator>();
        #endif
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Test)
        {
            m_Test = false;
            CheckState();
        }
	}

    private void CheckState()
    {
#if SOFTCEN_DEBUG
        AnimatorStateInfo stateInfo = m_anim.GetCurrentAnimatorStateInfo(0);
        Debug.Log(stateInfo);
#endif
    }
}
