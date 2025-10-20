using UnityEngine;
using System.Collections;

public class DeactivateAfter : MonoBehaviour {
	public float deactiveTime;

	private float _timer;
	// Use this for initialization
	void OnEnable () {
		_timer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		_timer += Time.deltaTime;
		if (_timer >= deactiveTime) {
			gameObject.SetActive(false);
		}
	}
}
