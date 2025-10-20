using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System;

public class AudioManager : MonoBehaviour {
	public static event Action OnMusicSettingChanged;

	public GameObject goSoundsDisable;
	public GameObject goMusicDisable;

	public Text soundsButtonTxt;
	public Text musicButtonTxt;

    public AudioClip audioButtonClick;
    public AudioClip audioBackButton;

    public AudioClip audioOpenDialog;
    public AudioClip audioCloseDialog;

    public float audioVolume = 1f;

    public bool usePlayerPrefs = true;


    public enum AudioSX {
		Button = 0,
		DialogOpen = 1,
		DialogClose = 2,
		SubMenuOpen = 3,
		SubMenuClose = 4,
		TouchOk = 5,
		TouchError = 6,
	}
	private bool _soundsOn;
	public bool SoundsOn {
		get { return _soundsOn; }
		set { 
			if (value != _soundsOn) {
				_soundsOn = value;
                if (usePlayerPrefs)
                {
                    try
                    {
                        if (_soundsOn)
                            PlayerPrefs.SetInt("Sounds", 1);
                        else
                            PlayerPrefs.SetInt("Sounds", 0);
                        PlayerPrefs.Save();
                    }
                    catch
                    {

                    }

                }
                UpdateDisableSprites();
			}
			//Debug.Log("Set Sounds: " + _soundsOn);
		}
	}
	private bool _musicOn;
	public bool MusicOn {
		get { return _musicOn; }
		set {
			//Debug.Log("MusicOn SET: " + value.ToString());
			if (value != _musicOn) {
				_musicOn = value;
                if (usePlayerPrefs)
                {
                    try
                    {
                        if (_musicOn)
                            PlayerPrefs.SetInt("Music", 1);
                        else
                            PlayerPrefs.SetInt("Music", 0);
                        PlayerPrefs.Save();
                    }
                    catch
                    {

                    }
                }
                UpdateDisableSprites();
				if (OnMusicSettingChanged != null) {
					OnMusicSettingChanged();
				}
			}
		}
	}
	public int pooledAmount = 5;
	public GameObject pooledObject;
	public AudioClip[] clips;
	public bool willGrow = true;

	//private List<GameObject> pooledObjects;

	private static AudioManager _instance = null;
	private int i1;

    private AudioSource ownAudioSource;

	public static AudioManager Instance {
		get {
			return _instance;
		}
	}


	void Awake() {
        ownAudioSource = GetComponent<AudioSource>();

        if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			_instance = this;
		}
		//Invoke("AllAudioSources", 3.0f);
		/*pooledObjects = new List<GameObject>();
		for (int i=0; i < pooledAmount; i++) {
			GameObject go = (GameObject)Instantiate (pooledObject);
			go.transform.parent = this.transform;
			go.SetActive(true);
			pooledObjects.Add(go);
		}*/

        DontDestroyOnLoad(this.gameObject);
		Load();

	}

	void Start() {
        UpdateDisableSprites();
	}

	public void ToggleSounds() {
		SoundsOn = !SoundsOn;
	}

	public void ToggleMusic() {
		MusicOn = !MusicOn;
	}

#if UNITY_WP8
	void OnApplicationPause(bool pauseStatus)
	{
		Debug.Log("AudioManager OnApplicationPause " + pauseStatus);
		if (pauseStatus) {
			for (int i=0; i < pooledObjects.Count; i++) {
				if (!pooledObjects[i1].audio.isPlaying) {
					pooledObjects[i1].audio.Stop();
				}
			}
		}
	}
#endif

	/*public void AllAudioSources() {
		string strTmp = "";
		AudioSource[] audios = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		foreach (AudioSource g in audios) {
			strTmp += "," + g.name;
		}
		//Debug.Log("AudioSources: " + strTmp); 

	}*/

	public void UpdateAudioButtons() {
		UpdateDisableSprites();
	}

	private void UpdateDisableSprites() {
		if (goSoundsDisable != null)
			goSoundsDisable.SetActive(!_soundsOn);
		if (goMusicDisable != null)
			goMusicDisable.SetActive(!_musicOn);

		if (soundsButtonTxt != null) {
			if (_soundsOn)
				soundsButtonTxt.text = "SOUNDS ON";
			else
				soundsButtonTxt.text = "SOUNDS OFF";
		}
		if (musicButtonTxt != null) {
			if (_musicOn)
				musicButtonTxt.text = "MUSIC ON";
			else
				musicButtonTxt.text = "MUSIC OFF";
		}
	}

	public void PlayClipAtPoint(int index, Vector3 pos) {
		/*if (index < clips.Length) {
			if (SoundsOn) {
				for (i1=0; i1 < pooledObjects.Count; i1++) {
					AudioSource audS = pooledObjects[i1].GetComponent<AudioSource>();
					if (audS != null) {
						if (!audS.isPlaying) {
							pooledObjects[i1].transform.position = pos;
                            PlayAudioSource(audS, clips[index]);
							return;
						}
					}
				}
				if (willGrow) {
					GameObject go = (GameObject)Instantiate(pooledObject);
					go.transform.parent = this.transform;
					pooledObjects.Add(go);
					go.transform.position = pos;
                    PlayAudioSource(go.GetComponent<AudioSource>(), clips[index]);
				}

			}
		}*/

	}
	public void PlayClip(AudioSX audioSX) {
		if ((int)audioSX >= clips.Length)
			return;

		if (SoundsOn) {
            ownAudioSource.PlayOneShot(clips[(int)audioSX]);
            /*
			for (i1=0; i1 < pooledObjects.Count; i1++) {
				AudioSource audS = pooledObjects[i1].GetComponent<AudioSource>();
				if (audS != null) {
					if (!audS.isPlaying) {
						pooledObjects[i1].transform.position = Vector3.zero;
                        PlayAudioSource(audS, clips[(int)audioSX]);
						return;
					}
				}
				else {
#if SOFTCEN_DEBUG
					Debug.LogWarning("AudioSource component not found!");
#endif
				}
			}
			if (willGrow) {
				GameObject go = (GameObject)Instantiate(pooledObject);
				go.transform.parent = this.transform;
				pooledObjects.Add(go);
				go.transform.position = Vector3.zero;
                PlayAudioSource(go.GetComponent<AudioSource>(), clips[(int)audioSX]);
			}*/
			
		}
	}

    public void PlayDialogOpen()
    {
        PlayClip(audioOpenDialog);
    }
    public void PlayDialogClose()
    {
        PlayClip(audioCloseDialog);
    }

    public void PlayButtonClick() {
        PlayClip(audioButtonClick);
    }

    public void PlayBackButtonClick() {
        PlayClip(audioBackButton);
    }

	public void PlayClip(AudioClip clip) {
		if (clip != null) {
			if (SoundsOn) {
                ownAudioSource.PlayOneShot(clip);
                /*for (i1=0; i1 < pooledObjects.Count; i1++) {
					AudioSource audS = pooledObjects[i1].GetComponent<AudioSource>();
					if (audS != null) {
						if (!audS.isPlaying) {
							pooledObjects[i1].transform.position = Vector3.zero;
                            PlayAudioSource(audS, clip);
							return;
						}
					}
					else {
						#if SOFTCEN_DEBUG
						Debug.LogWarning("AudioSource component not found!");
						#endif
					}
				}
				if (willGrow) {
					GameObject go = (GameObject)Instantiate(pooledObject);
					go.transform.parent = this.transform;
					pooledObjects.Add(go);
					go.transform.position = Vector3.zero;
                    PlayAudioSource(go.GetComponent<AudioSource>(), clip);
				}*/

            }
        }

	}
	public void PlayClipAtPoint(AudioClip clip, Vector3 pos) {
		if (clip != null) {
			if (SoundsOn) {
				/*for (i1=0; i1 < pooledObjects.Count; i1++) {
					if (!pooledObjects[i1].GetComponent<AudioSource>().isPlaying) {
						pooledObjects[i1].transform.position = pos;
                        PlayAudioSource(pooledObjects[i1].GetComponent<AudioSource>(), clip);
                        return;
					}
				}
				if (willGrow) {
					GameObject go = (GameObject)Instantiate(pooledObject);
					go.transform.parent = this.transform;
					pooledObjects.Add(go);
					go.transform.position = pos;
                    PlayAudioSource(go.GetComponent<AudioSource>(), clip);
                }*/
				
			}
		}		
	}

    private void PlayAudioSource(AudioSource auds, AudioClip clip)
    {
        if (auds != null && clip != null)
        {
            auds.clip = clip;
            auds.volume = audioVolume;
            auds.Play();
        }
    }


	private void Load() {
        if (usePlayerPrefs)
        {
            if (PlayerPrefs.HasKey("Sounds"))
            {
                int sound = PlayerPrefs.GetInt("Sounds");
                if (sound == 0)
                    _soundsOn = false;
                else
                    _soundsOn = true;
            }
            else
            {
                _soundsOn = true;
            }

            if (PlayerPrefs.HasKey("Music"))
            {
                int music = PlayerPrefs.GetInt("Music");
                if (music == 0)
                    _musicOn = false;
                else
                    _musicOn = true;
            }
            else
            {
                _musicOn = true;
            }

        }
        else
        {
            _soundsOn = true;
            _musicOn = true;
        }

        //Debug.Log("Sounds: " + _soundsOn.ToString() + ", Music: " + _musicOn.ToString());
    }

}
