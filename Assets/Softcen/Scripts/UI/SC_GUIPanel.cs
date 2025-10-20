using UnityEngine;
using UnityEngine.UI;
using Beebyte.Obfuscator;

public class SC_GUIPanel : MonoBehaviour {
    public Component[] scAnims;

    public DialogManager m_DialogManager;
	public Canvas[] m_CanvasesToDeactivated;
	public bool m_ReactivateCanvasesWhenFinished = true;
    public bool m_EffectsOnChildren = true;

    public bool m_PlayAudio = true;

    public bool m_UseBGFade = false;
    public float m_FadeTime = 1f;
    public Image imgBG1;
    public Image imgBG2;
    public GameObject goMain;
    public bool useFullScreenClose = false;

    private float m_fadeTimer;
    private bool m_HideCalled = false;

    public bool TestHide = false;
	void Awake ()
    {
        scAnims = gameObject.GetComponentsInChildren(typeof(SCUIAnim), true);
	}

    void OnEnable()
    {
        m_HideCalled = false;
    }

    void Update()
    {
        if (TestHide)
        {
            TestHide = false;
            Hide();
            return;
        }
        if (m_UseBGFade && m_fadeMode != 0)
        {
            m_fadeTimer += Time.deltaTime;
            if (m_fadeMode == 1)
            {
                float t = Mathf.Min(1, m_fadeTimer / m_FadeTime);
                m_bg1Color.a = Mathf.Lerp(0, 1, t);
                imgBG1.color = m_bg1Color;
                if (t == 1)
                {
                    m_fadeTimer = 0;
                    m_fadeMode = 2;
                    if (!goMain.activeSelf)
                        goMain.SetActive(true);
                    if (!imgBG2.gameObject.activeSelf)
                        imgBG2.gameObject.SetActive(true);
                }
            }
            else if (m_fadeMode == 2)
            {
                float t = Mathf.Max(0, (m_FadeTime - m_fadeTimer) / m_FadeTime);
                m_bg2Color.a = Mathf.Lerp(0, 1, t);
                imgBG2.color = m_bg2Color;
                if (t == 0)
                {
                    if (imgBG2.gameObject.activeSelf)
                        imgBG2.gameObject.SetActive(false);
                    m_fadeMode = 0;
                }
            }
            else if (m_fadeMode == 3)
            {
                float t = Mathf.Min(1, m_fadeTimer / m_FadeTime);
                m_bg2Color.a = Mathf.Lerp(0, 1, t);
                imgBG2.color = m_bg2Color;
                if (t == 1)
                {
                    m_fadeTimer = 0;
                    m_fadeMode = 4;
                    if (goMain.activeSelf)
                        goMain.SetActive(false);
                    imgBG2.gameObject.SetActive(false);
                }
            }
            else if (m_fadeMode == 4)
            {
                float t = Mathf.Max(0, (m_FadeTime - m_fadeTimer) / m_FadeTime);
                m_bg1Color.a = Mathf.Lerp(0, 1, t);
                imgBG1.color = m_bg1Color;
                if (t == 0)
                {
                    gameObject.SetActive(false);
                    m_fadeMode = 0;
                }
            }

        }
    }


    void OnDisable() {
		//Debug.Log("SC_GUIPanel OnDisable " +gameObject.name);
		if (m_DialogManager != null) {
			m_DialogManager.CheckDialogs();
		}
	}

    private Color m_bg1Color;
    private Color m_bg2Color;
    private int m_fadeMode; // 0 = idle, 1 = fadein, 2 = fadeout
    public virtual void Show()
	{
        if (m_PlayAudio)
            AudioManager.Instance.PlayDialogOpen();

        if (m_UseBGFade)
        {
            m_fadeTimer = 0f;
            m_fadeMode = 1;
            if (goMain.activeSelf)
                goMain.SetActive(false);
            if (imgBG2.gameObject.activeSelf)
                imgBG2.gameObject.SetActive(false);
            m_bg2Color = imgBG2.color;
            m_bg2Color.a = 1f;
            imgBG2.color = m_bg2Color;
            m_bg1Color = imgBG1.color;
            m_bg1Color.a = 0f;
            imgBG1.color = m_bg1Color;

            if (!imgBG1.gameObject.activeSelf)
                imgBG1.gameObject.SetActive(true);
            return;
        }
		ShowWithoutAnim();

	}

    [SkipRename]
    public void ReadyForFullScreenClose()
    {
        if (useFullScreenClose && m_DialogManager != null && !m_HideCalled)
        {
            m_DialogManager.FullScreenDialogActive();
        }
    }

    public void ShowWithoutAnim() {
		if (!gameObject.activeSelf)
			gameObject.SetActive(true);
        /*
		if(m_CanvasesToDeactivated!=null)
		{
			if(m_CanvasesToDeactivated.Length>0)
			{
				// Disable GraphicRaycaster of Canvas in m_CanvasesToDeactivated
				for (int i=0; i < m_CanvasesToDeactivated.Length; i++) {
					// Disable GraphicRaycasters of Canvas in child of m_CanvasesToDeactivated
					GEAnimSystem.Instance.SetGraphicRaycasterEnable(m_CanvasesToDeactivated[i], false);
                    
				}
			}
		}*/
		
		// Enable GraphicRaycasters of Canvas in this.gameObject
		//GEAnimSystem.Instance.SetGraphicRaycasterEnable(this.gameObject, true);
	}

    [SkipRename]
	public virtual void Hide()
	{
        m_HideCalled = true;
        if (scAnims != null)
        {
            for (int i = 0; i < scAnims.Length; i++)
            {
                (scAnims[i] as SCUIAnim).MoveOut();
            }
        }
        if (useFullScreenClose && m_DialogManager != null)
        {
            m_DialogManager.FullScreenDialogClose();
        }


        if (m_PlayAudio)
            AudioManager.Instance.PlayDialogClose();
        if (m_UseBGFade)
        {
            m_bg2Color.a = 0f;
            imgBG2.color = m_bg2Color;
            imgBG2.gameObject.SetActive(true);
            m_fadeTimer = 0f;
            m_fadeMode = 3;
            return;
        }

        //Debug.Log("SC_GUIPanel Hide");
        // Disable GraphicRaycasters of Canvas in this.gameObject
        //GEAnimSystem.Instance.SetGraphicRaycasterEnable(this.gameObject, false);
        /*
		if(m_CanvasesToDeactivated!=null && m_ReactivateCanvasesWhenFinished==true)
		{ 
			if(m_CanvasesToDeactivated.Length>0)
			{
				for (int i=0; i < m_CanvasesToDeactivated.Length; i++) {
					GEAnimSystem.Instance.SetGraphicRaycasterEnable(m_CanvasesToDeactivated[i], true);
				}
			}
		}*/
		
		// Play Move Out animation
		//GEAnimSystem.Instance.MoveOut(this.transform, m_EffectsOnChildren);
	}

    [SkipRename]
	public void Closed() {
		//Debug.Log("Closed " + gameObject.name);
		gameObject.SetActive(false);
        if (m_DialogManager != null)
        {
            m_DialogManager.CheckDialogs();
        }

    }

    [SkipRename]
    public void WindowClosed() {
		//Debug.Log("WindowClosed " + gameObject.name);
		gameObject.SetActive(false);
		if (m_DialogManager != null) {
			m_DialogManager.CheckDialogs();
		}
        if (HomeManager.Instance != null)
		    HomeManager.Instance.DialogClosed();
	}

}
