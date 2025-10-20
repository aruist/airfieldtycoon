using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenshotBanner : MonoBehaviour {
    public Image bannerImage;
    public Sprite[] bannerSprites;

    private int index = 0;
	// Use this for initialization
	void Start () {
        bannerImage.sprite = bannerSprites[index];
    }

    // Update is called once per frame
    void Update () {
	    if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus))
        {
            index = Mathf.Max(0, index - 1);
            bannerImage.sprite = bannerSprites[index];
        }
        else if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            index = Mathf.Min(index +1 , bannerSprites.Length-1);
            bannerImage.sprite = bannerSprites[index];
        }
    }
}
