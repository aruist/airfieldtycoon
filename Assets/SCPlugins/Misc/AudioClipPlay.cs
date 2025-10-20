using UnityEngine;
using System.Collections;

public class AudioClipPlay : MonoBehaviour {
	public AudioClip clip;
	public bool silenceAwake = false;

	private bool isAwake = true;

	void OnEnable() {
		if (silenceAwake && isAwake) {
			isAwake = false;
			return;
		}
		isAwake = false;
		AudioManager.Instance.PlayClip(clip);

	}
}
