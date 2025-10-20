using UnityEngine;
using System.Collections;

public class GameMusicManagerScript : MonoBehaviour {
	public AudioClip NewMusic;
	public bool fadeSwap = false;
	public float delaySwap = 0f;

	void Start() {
		//Debug.Log("GameMusicManagerScript Start()");
		if (NewMusic == null)
			return;

		GameObject go = GameObject.Find("MusicManager");
		if (go != null) {
			if (go.GetComponent<AudioSource>().clip == null || go.GetComponent<AudioSource>().clip.name != NewMusic.name) {
				GameMusicScript script = go.GetComponent<GameMusicScript>();
				if (script != null) {
					if (fadeSwap)
						script.ChangeAudioTrack(NewMusic, delaySwap);
					else 
						script.SwapAudioTrack(NewMusic, delaySwap);
				}
			}
		}
	}
}
