using UnityEngine;
using UnityEngine.Audio;

using System.Collections;

public class SCBackgroundMusic : MonoBehaviour {
    public static SCBackgroundMusic Instance = null;
    public bool playOnStart = false;
    public AudioClip[] acBackgroundMusics;

    public AudioMixerSnapshot gameOnSnapshot;
    public float snapshottimeScale = 2f;

    private AudioSource m_audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        m_audioSource = GetComponent<AudioSource>();
    }
	// Use this for initialization
	void Start () {
        if (gameOnSnapshot != null)
        {
            gameOnSnapshot.TransitionTo(snapshottimeScale);
        }
	    if (playOnStart && acBackgroundMusics.Length > 0)
        {
            m_audioSource.clip = acBackgroundMusics[0];
            m_audioSource.Play();
        }
	}
	
}
