using UnityEngine;
using System.Collections;
using Beebyte.Obfuscator;

public class IdleCharAnimCtrl : MonoBehaviour {
    private Animator anim;
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
	}

    void OnDisable()
    {
        //Debug.Log("OnDisable");
        CancelInvoke();
    }

    [SkipRename]
    public void AnimCrouchDown(float time)
    {
        //Debug.Log("AnimCrouchDown time: " + time.ToString());
        anim.SetTrigger("CrouchDown");
        Invoke("AnimWalk", time);

    }
    [SkipRename]
    public void AnimWalk()
    {
        //Debug.Log("AnimWalk");
        anim.SetTrigger("Walk");

    }
    [SkipRename]
    public void AnimIdle(float time)
    {
        //Debug.Log("AnimIdle time: " + time.ToString());
        anim.SetTrigger("Idle");
        Invoke("AnimWalk", time);
    }
}
