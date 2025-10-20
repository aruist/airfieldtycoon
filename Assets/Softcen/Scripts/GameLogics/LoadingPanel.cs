using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingPanel : MonoBehaviour {
    public float bgAlphaSpeed = 1f;
    public float bgAlphaMax = 0.8f;
    public Image imgProgress;

    private Color m_color;
    private Image _bgImage;
    public string m_scenename;
    private Image bgImage
    {
        get {
            if (_bgImage == null)
                _bgImage = GetComponent<Image>();
            return _bgImage;
        }
    }
    public bool reset;
    private bool loadCalled;
    // Use this for initialization
    private bool m_Initialized = false;
	void Start ()
    {
        m_Initialized = true;
        ResetLoadingPanel();
    }

    void OnEnable()
    {
        if (m_Initialized)
        {
            ResetLoadingPanel();
        }
    }

    private void ResetLoadingPanel()
    {
        loadCalled = false;
        m_color = bgImage.color;
        m_color.a = 0f;
        imgProgress.fillAmount = 0;
        bgImage.color = m_color;
    }

    // Update is called once per frame
    void Update () {
        if (reset)
        {
            reset = false;
            m_color.a = 0;
            bgImage.color = m_color;
        }
        if (m_color.a < bgAlphaMax)
        {
            m_color.a += Time.deltaTime * bgAlphaSpeed;
            bgImage.color = m_color;
        }
        else if (!loadCalled)
        {
            LoadScene(m_scenename);
        }
    }

    public void LoadScene(string scenename)
    {
        loadCalled = true;
        //Debug.Log("<color=yellow>LoadScene Start</color>");
        StartCoroutine(AsynchronousLoad(scenename));
        //Debug.Log("<color=yellow>LoadScene Called</color>");
    }

    IEnumerator AsynchronousLoad(string scene)
    {
        //Debug.Log("<color=yellow>AsynchronousLoad Start</color>");
        yield return null;
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            imgProgress.fillAmount = progress;

            //Debug.Log("Loading progress: " + (progress * 100) + "%");

            // Loading completed
            if (ao.progress == 0.9f)
            {
                ao.allowSceneActivation = true;
            }

            yield return null;
        }
        //Debug.Log("<color=yellow>LoadScene Done</color>");

    }
}
