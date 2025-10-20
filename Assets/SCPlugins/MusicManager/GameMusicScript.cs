using UnityEngine;
using System.Collections;


public class GameMusicScript : MonoBehaviour {
	public enum MusicClips {
		MainMenu = 0,
		GamePlay,
		None
	}
	public MusicClips currentMusic;
	public AudioClip[] GameGgMusic;
	public bool playOnAwake = true;

	public float fadeInSpeed = 0.8f;
	public float fadeOutSpeed = 1f;

	public static GameMusicScript instance = null;
	private AudioClip newAudioClip = null;
	private bool bChangeOngoing;
	private bool bFadeIn;
	private bool bFadeOut;
	private float swapDelay = 0f;
	private float swapDelayTimer;

	private bool audioStoppedOnPause = false;

	void Awake() {
		currentMusic = MusicClips.None;
		//Debug.Log("GameMusicScript Awake");
		bChangeOngoing = false;
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
		AudioManager.OnMusicSettingChanged += HandleOnMusicSettingChanged;
		if (playOnAwake) {
			PlayBackgroundMusic(MusicClips.MainMenu, false);
		}
	}


	public void PlayBackgroundMusic(MusicClips clip, bool useFade) {
		if (AudioManager.Instance == null || GameGgMusic.Length == 0 )
			return;

		AudioSource audio = GetComponent<AudioSource>();

		if (clip == MusicClips.None && audio.isPlaying) {
			audio.Stop();
		}
		else {
			if (AudioManager.Instance.MusicOn) {
				if (useFade)
					ChangeAudioTrack(GameGgMusic[(int)clip], 0f);
				else {
					if (audio.isPlaying) {
						audio.Stop();
					}
					audio.clip = GameGgMusic[(int)clip];
					audio.Play();
				}
			}
		}
	}


	void HandleOnMusicSettingChanged ()
	{
		if (GetComponent<AudioSource>().isPlaying) {
			if (!AudioManager.Instance.MusicOn) {
				GetComponent<AudioSource>().Stop();
			}
		}
		else {
			if (AudioManager.Instance.MusicOn) {
				AudioSource audios = GetComponent<AudioSource>();
				if (audios.clip != null) {
					audios.Play();
				}
				else {
					PlayBackgroundMusic(MusicClips.MainMenu, false);
				}

			}
		}
	}

	public void SwapAudioTrack(AudioClip aclip, float swapdelay) {
		//Debug.Log("SwapAudioTrack Music: " + AudioManager.Instance.MusicOn);
		swapDelay = swapdelay;
		swapDelayTimer = 0f;

		AudioSource audios = GetComponent<AudioSource>();
		if (audios != null) {
			if (audios.clip != null && audios.clip.name == aclip.name) {
				if (AudioManager.Instance.MusicOn && !audios.isPlaying)
					audios.Play();
				return;
			}

			if (audios.isPlaying) {
				audios.Stop();
			}
			audios.clip = aclip;
			if (AudioManager.Instance.MusicOn) {
				audios.Play();
			}

		}
	}
		
	public void ChangeAudioTrack(AudioClip aclip, float swapdelay) {
		//Debug.Log("ChangeAudioTrack Music: " + AudioManager.Instance.MusicOn);
		swapDelay = swapdelay;
		swapDelayTimer = 0f;
		if (aclip == null) {
			return;
		}
		if (aclip != GetComponent<AudioSource>().clip) {
			newAudioClip = aclip;
			bChangeOngoing = true;
			if (GetComponent<AudioSource>().clip == null) {
				// No previous audio start new audio
				bChangeOngoing = true;
				bFadeOut = false;
				bFadeIn = true;
				GetComponent<AudioSource>().clip = aclip;
				GetComponent<AudioSource>().volume = 0f;
				if (AudioManager.Instance.MusicOn) {
					GetComponent<AudioSource>().Play();
				}

			} else {
				// Change audio track fadeout current and then fadein new audio.
				bChangeOngoing = true;
				bFadeOut = true;
				bFadeIn = false;
			}
		}
	}

	void Update() {
		if (!bChangeOngoing) {
			return;
		}
		if (bFadeOut) {
			if (GetComponent<AudioSource>().volume > 0f) {
				float vol = GetComponent<AudioSource>().volume - fadeOutSpeed * Time.deltaTime;
				if (vol > 0) {
					GetComponent<AudioSource>().volume = vol;
				} else {
					GetComponent<AudioSource>().volume = 0;
				}
			} else {
				bFadeOut = false;
				bFadeIn = true;
				if (GetComponent<AudioSource>().isPlaying)
					GetComponent<AudioSource>().Stop();
				GetComponent<AudioSource>().clip = newAudioClip;
				if (AudioManager.Instance.MusicOn) {
					GetComponent<AudioSource>().Play();
				}

			}
			return;
		}
		if (bFadeIn && swapDelayTimer < swapDelay) {
			swapDelayTimer += Time.deltaTime;
		}
		else if (bFadeIn) {
			if (GetComponent<AudioSource>().volume < 1f) {
				float vol = GetComponent<AudioSource>().volume + fadeInSpeed * Time.deltaTime;
				if (vol > 1f) {
					GetComponent<AudioSource>().volume = 1f;
					bChangeOngoing = false;
					bFadeIn = false;
					bFadeOut = false;
				} else {
					GetComponent<AudioSource>().volume = vol;
				}
			}
		}
	}

	void OnApplicationQuit()
	{
		if (GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Stop();
		}
		AudioManager.OnMusicSettingChanged -= HandleOnMusicSettingChanged;
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			if (GetComponent<AudioSource>().isPlaying) {
				audioStoppedOnPause = true;
				GetComponent<AudioSource>().Stop();
			}
		} else {
			if (audioStoppedOnPause) {
				audioStoppedOnPause = false;
				GetComponent<AudioSource>().Play();
			}
		}
	}


}
