using UnityEngine;
using System.Collections;

public class SpriteScreenResize : MonoBehaviour {
	public bool resizeWidth = true;
	public bool resizeHeight = true;

	// Use this for initialization
	void Awake () {
		Resize();
	}

	private void Resize() {
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		//transform.localScale = new Vector3(1,1,1);
		float width = sr.sprite.bounds.size.x;
		float height = sr.sprite.bounds.size.y;

		float worldScreenHeight = Camera.main.orthographicSize * 2f;
		float worldScreenWidth = worldScreenHeight/Screen.height*Screen.width;

		if (resizeWidth) {
			Vector3 xWidth = transform.localScale;
			xWidth.x = worldScreenWidth / width;
			transform.localScale = xWidth;
		}
		if (resizeHeight) {
			Vector3 yHeight = transform.localScale;
			yHeight.y = worldScreenHeight / height;
			transform.localScale = yHeight;
		}

	}
}
