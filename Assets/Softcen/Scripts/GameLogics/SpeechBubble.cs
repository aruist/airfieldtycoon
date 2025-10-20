using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour {
    public Camera cam;
    public CanvasGroup canvasGroup;
    public Text txtSpeech;
    public Image imgSpeech;
    public float fadeSpeed;
    private bool fadeIn;
    public bool fadeOut;
    private float m_alpha;

    public string newText;
    public bool startTextChange;
    private bool changeText;
    private bool textFadeOut;
    private bool textFadeIn;
    public float textChangeSpeed = 1f;
    private Color textColor;
    public AudioSource audioSrc;

    void Start()
    {
        imgSpeech.sprite = BonusManager.Instance.GetChapterSprite(GameManager.Instance.selectedChapter);
    }

    void OnEnable()
    {
        m_alpha = 0f;
        fadeIn = true;
        canvasGroup.alpha = m_alpha;
        cam = Camera.main;
        audioSrc.Play();
    }
	
	// Update is called once per frame
	void Update () {
        transform.rotation = cam.transform.rotation;
        if (startTextChange)
        {
            startTextChange = false;
            changeText = true;
            textColor = txtSpeech.color;
            textFadeIn = false;
            textFadeOut = true;
        }
        if (changeText)
        {
            if (textFadeOut)
            {
                textColor.a -= Time.deltaTime * textChangeSpeed;
                if (textColor.a <= 0f)
                {
                    textColor.a = 0f;
                    textFadeOut = false;
                    txtSpeech.text = newText;
                    textFadeIn = true;
                    textFadeOut = false;
                    audioSrc.Play();
                }
                txtSpeech.color = textColor;
            }
            else if (textFadeIn)
            {
                textColor.a += Time.deltaTime * textChangeSpeed;
                if (textColor.a >= 1f)
                {
                    textColor.a = 1f;
                    textFadeIn = false;
                }
                txtSpeech.color = textColor;
            }
        }
        if (fadeIn)
        {
            m_alpha += Time.deltaTime * fadeSpeed;
            if (m_alpha >= 1f)
            {
                m_alpha = 1f;
                fadeIn = false;
            }
            canvasGroup.alpha = m_alpha;
        }
        if (fadeOut)
        {
            m_alpha -= Time.deltaTime * fadeSpeed;
            if (m_alpha <= 0f)
            {
                m_alpha = 0f;
                fadeOut = false;
                gameObject.SetActive(false);
            }
            canvasGroup.alpha = m_alpha;
        }
    }
}
