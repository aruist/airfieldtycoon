using UnityEngine;
using System.Collections;

public class GoDestroyOptions : MonoBehaviour {
	public bool notSoftcenDebug;
	// Use this for initialization
	void Awake () {
#if !SOFTCEN_DEBUG
		if (notSoftcenDebug) {
			Destroy(gameObject);
		}
#endif
	}

	public void DeactiveteGO() {
		StartCoroutine("Delay");
	}
	
	private IEnumerator Delay() {
		yield return new WaitForEndOfFrame();
		gameObject.SetActive(false);
	}

}
