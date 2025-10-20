using UnityEngine;
using System.Collections;

public class Seagull : MonoBehaviour {
    public float audioMinTime;
    public float audioMaxTime;
    public AudioClip acSeagull;
    public AudioSource m_AudioSource;

    void OnEnable()
    {
        Invoke("PlaySeagullSound", Random.Range(audioMinTime, audioMaxTime));
    }
    void OnDisable()
    {
        CancelInvoke();
    }

    private void PlaySeagullSound()
    {
        if (AudioManager.Instance.SoundsOn)
            m_AudioSource.Play();
        //AudioManager.Instance.PlayClipAtPoint(acSeagull, transform.position);
        Invoke("PlaySeagullSound", Random.Range(audioMinTime, audioMaxTime));
    }

}
