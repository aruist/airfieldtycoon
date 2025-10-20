using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SCUIAnim : MonoBehaviour {

    public enum SCMoveFrom
    {
        UpperScreenEdge,
        BottomScreenEdge,
        LeftScreenEdge,
        RightScreenEdge
    }

    [System.Serializable]
    public struct SCCallback
    {
        public GameObject receiver;
        public string Method;
    }

    [System.Serializable]
    public struct SCScale
    {
        public bool enabled;
        public Interpolate.EaseType type;
        public Vector2 scaleStart;
        public bool bounce;
        public float duration;
        public float startDelay;
        public SCCallback callback;
        public bool deactivateFinished;
    }

    [System.Serializable]
    public struct Fade
    {
        public bool enabled;
        public Interpolate.EaseType type;
        public Image fadeImage;
        public float fadeTime;
        public float fadeMax;
        private bool running;
        public SCCallback callback;
        public bool deactivateFinished;
    };
    public Fade fadeIn;
    public Fade fadeOut;

    [System.Serializable]
    public struct SCMove
    {
        public bool enabled;
        public Interpolate.EaseType type;
        public AudioClip moveAudio;
        public SCMoveFrom moveFrom;
        //public Vector2 startPos;
        //public Vector2 endPos;
        public bool bounce;
        public float duration;
        public float startDelay;
        public SCCallback callback;
        public bool deactivateFinished;
    }
    public SCMove moveIn;
    public SCMove moveOut;

    public SCScale scaleIn;
    public SCScale scaleOut;

    public float bounceTime = 0.2f;
    public float bounceAmount = 0.2f;

    private RectTransform rt;
    private bool m_moveIn = false;
    private bool m_moveOut = false;
    private bool m_fadeIn = false;
    private bool m_fadeOut = false;
    private float fadeTimer;
    private Color fadeColor;
    private Vector3 m_MoveDistance;
    private Vector2 m_MoveStartPos;
    private Vector2 m_MoveOutPos = new Vector2();
    private float m_MoveElapsedTime;
    private float m_MoveDuration;
    private Interpolate.Function m_MoveFunction;
    private int m_MoveStep;
    private Interpolate.Function m_FadeFunction;
    private float m_MoveDelayTimer;
    // Use this for initialization
    public bool dbgResetAnim = false;
    public bool dbgStartMoveOut = false;

    public Vector2 anchornedPos;

    public float m_CanvasLeftEdge;
    public float m_CanvasRightEdge;
    public float m_CanvasTopEdge;
    public float m_CanvasBottomEdge;
    public RectTransform _m_ParentCanvasRectTransform = null;
    public RectTransform m_ParentCanvasRectTransform
    {
        get
        {
            if (_m_ParentCanvasRectTransform == null)
            {
                if (m_Parent_Canvas != null)
                    _m_ParentCanvasRectTransform = m_Parent_Canvas.GetComponent<RectTransform>();
            }
            return _m_ParentCanvasRectTransform;
        }
    }
    public Canvas _m_Parent_Canvas;
    public Canvas m_Parent_Canvas
    {
        get
        {
            if (_m_Parent_Canvas == null)
                _m_Parent_Canvas = GetComponentInParent<Canvas>();
            return _m_Parent_Canvas;
        }
    }

    private bool m_Initialized = false;
    void Start()
    {
        m_Initialized = true;
        rt = GetComponent<RectTransform>();
        StartAnim();
    }

    void OnEnable()
    {
        if (!m_Initialized)
            return;

        //if (m_Parent_Canvas != null)
        //{
            //m_ParentCanvasRectTransform = m_Parent_Canvas.GetComponent<RectTransform>();
            //m_CanvasRightEdge = (m_ParentCanvasRectTransform.rect.width / 2);
            //m_CanvasLeftEdge = -m_CanvasRightEdge;
            //m_CanvasTopEdge = (m_ParentCanvasRectTransform.rect.height / 2);
            //m_CanvasBottomEdge = -m_CanvasTopEdge;
        //}
        rt = GetComponent<RectTransform>();
        StartAnim();
    }

    public void ResetAnim()
    {
        rt = GetComponent<RectTransform>();
        StartAnim();
    }

    private void StartAnim()
    {
        if (fadeIn.enabled)
        {
            StartFadeIn();
        }

        if (moveIn.enabled)
        {
            StartMoveIn();
        }

        if (scaleIn.enabled)
        {
            StartScaleIn();
        }
    }


    // Update is called once per frame
    void Update ()
    {
        //GetComponentInParent
        anchornedPos = rt.anchoredPosition;
        if (dbgResetAnim)
        {
            dbgResetAnim = false;
            StartAnim();
            return;
        }
        if (dbgStartMoveOut)
        {
            dbgStartMoveOut = false;
            MoveOut();
            return;
        }

        if ((fadeIn.enabled && m_fadeIn) || (fadeOut.enabled && m_fadeOut))
        {
            UpdateFade();
        }

        if ((moveIn.enabled && m_moveIn) || (moveOut.enabled && m_moveOut))
        {
            UpdateMove();
        }
        if ((scaleIn.enabled && m_scaleIn) || (scaleOut.enabled && m_scaleOut))
        {
            UpdateScale();
        }
    }

    private void StartMoveIn()
    {
        if (m_Parent_Canvas == null)
        {
            //Debug.Log("<color=lightblue>StartMoveIn Canvas is NULL SKIP</color>");
            m_moveOut = false;
            m_moveIn = false;
            return;
        }
        //Debug.Log("<color=lightred>StartMoveIn " + gameObject.name + "</color>", gameObject);
        m_MoveDelayTimer = 0;
        m_moveOut = false;
        m_moveIn = true;
        m_MoveFunction = Interpolate.Ease(moveIn.type);
        m_MoveElapsedTime = 0;
        m_MoveStep = 0;
        if (moveIn.moveFrom == SCMoveFrom.BottomScreenEdge)
        {
            float bottomEdge = -(m_ParentCanvasRectTransform.rect.height / 2);
            m_MoveStartPos.y = bottomEdge - (rt.localPosition.y - rt.anchoredPosition.y) - rt.rect.height / 2;
            m_MoveStartPos.x = 0;
            m_MoveDistance = Vector2.zero - m_MoveStartPos;
            if (moveIn.bounce)
            {
                m_MoveDistance.y += Mathf.Abs(rt.rect.height) * bounceAmount;
            }
        }
        else if (moveIn.moveFrom == SCMoveFrom.UpperScreenEdge)
        {
            float topEdge = (m_ParentCanvasRectTransform.rect.height / 2);
            m_MoveStartPos.y = topEdge - (rt.localPosition.y - rt.anchoredPosition.y) + rt.rect.height / 2;
            m_MoveStartPos.x = 0;
            m_MoveDistance = Vector2.zero - m_MoveStartPos;
            if (moveIn.bounce)
            {
                m_MoveDistance.y -= Mathf.Abs(rt.rect.height) * bounceAmount;
            }
        }
        else if (moveIn.moveFrom == SCMoveFrom.LeftScreenEdge)
        {
            float canvasLeftEdge = -m_ParentCanvasRectTransform.rect.width / 2;
            m_MoveStartPos.x = canvasLeftEdge - (rt.localPosition.x - rt.anchoredPosition.x) - rt.rect.width / 2;
            m_MoveStartPos.y = 0;
            m_MoveDistance = Vector2.zero - m_MoveStartPos;
            /*Debug.Log("m_MoveStartPos.x: " + m_MoveStartPos.x.ToString()
                + ", canvasLeftEdge: " + canvasLeftEdge.ToString()
                + ", rt.localPosition.x: " + rt.localPosition.x
                + ", rt.rect.width / 2: " + (rt.rect.width / 2).ToString()
                );*/
            if (moveIn.bounce)
            {
                m_MoveDistance.x += Mathf.Abs(rt.rect.width) * bounceAmount;
            }
            /*Debug.Log("<color=yellow>StartMoveIn " + gameObject.name
                + ", LeftScreenEdge, rt.localPosition: " + rt.localPosition.ToString()
                + ", canvasLeftEdge: " + canvasLeftEdge
                + ", rt.rect.width: " + rt.rect.width
                + ", m_MoveStartPos :" + m_MoveStartPos.ToString()
                + ", m_MoveDistance: " + m_MoveDistance.ToString()
                + "</color>");*/
        }
        else
        {
            // RightScreenEdge
            float canvasRightEdge = m_ParentCanvasRectTransform.rect.width / 2;
            m_MoveStartPos.x = canvasRightEdge - (rt.localPosition.x - rt.anchoredPosition.x) + rt.rect.width/2;
            m_MoveStartPos.y = 0;
            // Move distance endpos - startpos
            m_MoveDistance = Vector2.zero - m_MoveStartPos;
            if (moveIn.bounce)
            {
                m_MoveDistance.x -= Mathf.Abs(rt.rect.width) * bounceAmount;
            }

            /*Debug.Log("<color=yellow>StartMoveIn " + gameObject.name
                + ", RightScreenEdge, rt.localPosition: " + rt.localPosition.ToString()
                + ", rt.anchoredPosition: " + rt.anchoredPosition.ToString()
                + ", canvasRightEdge: " + canvasRightEdge
                + ", rt.rect.width: " + rt.rect.width
                + ", m_MoveStartPos :" + m_MoveStartPos.ToString()
                + ", m_MoveDistance: " + m_MoveDistance.ToString()
                + "</color>");*/
        }
        if (!moveIn.bounce)
        {
            m_MoveDuration = moveIn.duration;
        }
        else
        {
            m_MoveDuration = moveIn.duration * (1 - Mathf.Clamp(bounceTime, 0f, 1f)); 
        }
        //Debug.Log("rt.anchoredPosition: " + rt.anchoredPosition.ToString());
        if (m_MoveDistance == Vector3.zero)
        {
            //Debug.Log("<color=lightred>m_MoveDistance 0 SKIP " + gameObject.name +"</color>", gameObject);
            m_moveIn = false;
            return;
        }
        rt.anchoredPosition = m_MoveStartPos; // Vector2.Lerp(moveIn.startPos, moveIn.endPos, 0);
        if (AudioManager.Instance != null && moveIn.moveAudio != null)
        {
            AudioManager.Instance.PlayClip(moveIn.moveAudio);
        }

    }

    private void StartMoveOut()
    {
        if (rt == null || m_ParentCanvasRectTransform == null)
        {
            return;
        }

        m_moveOut = true;
        m_moveIn = false;
        m_MoveDelayTimer = 0;
        m_MoveFunction = Interpolate.Ease(moveOut.type);
        m_MoveElapsedTime = 0;
        m_MoveStep = 0;
        m_MoveStartPos = rt.anchoredPosition;

        if (moveOut.moveFrom == SCMoveFrom.BottomScreenEdge)
        {
            float bottomEdge = -(m_ParentCanvasRectTransform.rect.height / 2);
            m_MoveOutPos.y = bottomEdge - (rt.localPosition.y - rt.anchoredPosition.y) - rt.rect.height / 2;
            m_MoveOutPos.x = 0;
            // Move distance endpos - startpos
            if (moveOut.bounce)
            {
                m_MoveDistance = Vector2.zero;
                m_MoveDistance.y += Mathf.Abs(rt.rect.height) * bounceAmount;
            }
            else
            {
                m_MoveDistance = m_MoveOutPos - m_MoveStartPos;
            }
        }
        else if (moveOut.moveFrom == SCMoveFrom.UpperScreenEdge)
        {
            float topEdge = (m_ParentCanvasRectTransform.rect.height / 2);
            m_MoveOutPos.y = topEdge - (rt.localPosition.y - rt.anchoredPosition.y) + rt.rect.height / 2;
            m_MoveOutPos.x = 0;
            // Move distance endpos - startpos
            if (moveOut.bounce)
            {
                m_MoveDistance = Vector2.zero;
                m_MoveDistance.y -= Mathf.Abs(rt.rect.height) * bounceAmount;
            }
            else
            {
                m_MoveDistance = m_MoveOutPos - m_MoveStartPos;
            }
        }
        else if (moveOut.moveFrom == SCMoveFrom.LeftScreenEdge)
        {
            float canvasLeftEdge = -m_ParentCanvasRectTransform.rect.width / 2;
            m_MoveOutPos.x = canvasLeftEdge - rt.localPosition.x - rt.rect.width / 2;
            m_MoveOutPos.y = 0;
            if (moveOut.bounce)
            {
                m_MoveDistance = Vector2.zero;
                m_MoveDistance.x += Mathf.Abs(rt.rect.width) * bounceAmount;
            }
            else
            {
                m_MoveDistance = m_MoveOutPos - m_MoveStartPos;
            }
        }
        else // Right edge
        {            
            float canvasRightEdge = m_ParentCanvasRectTransform.rect.width / 2;
            m_MoveOutPos.x = canvasRightEdge - (rt.localPosition.x - rt.anchoredPosition.x) + rt.rect.width / 2;
            m_MoveOutPos.y = 0;
            // Move distance endpos - startpos
            if (moveOut.bounce)
            {
                m_MoveDistance = Vector2.zero;
                m_MoveDistance.x -= Mathf.Abs(rt.rect.width) * bounceAmount;
                //m_MoveDistance.x -= Mathf.Abs(rt.rect.width) * bounceAmount;
            }
            else
            {
                m_MoveDistance = m_MoveOutPos - m_MoveStartPos;
            }
        }
        if (!moveOut.bounce)
        {
            m_MoveDuration = moveOut.duration;
        }
        else
        {
            m_MoveDuration = moveOut.duration * bounceTime;
        }

        if (AudioManager.Instance != null && moveOut.moveAudio != null)
        {
            AudioManager.Instance.PlayClip(moveOut.moveAudio);
        }
    }

    private Vector2 m_anchoredPosition = new Vector2();
    private void UpdateMove()
    {
        if (m_moveIn)
        {
            if (m_MoveDelayTimer < moveIn.startDelay)
            {
                m_MoveDelayTimer += Time.deltaTime;
                return;
            }
        }
        else
        {
            if (m_MoveDelayTimer < moveOut.startDelay)
            {
                m_MoveDelayTimer += Time.deltaTime;
                return;
            }
        }

        bool sendMessage = false;
        m_MoveElapsedTime += Time.deltaTime;
        m_anchoredPosition.x = m_MoveFunction(m_MoveStartPos.x, m_MoveDistance.x, m_MoveElapsedTime, m_MoveDuration);
        m_anchoredPosition.y = m_MoveFunction(m_MoveStartPos.y, m_MoveDistance.y, m_MoveElapsedTime, m_MoveDuration);
        rt.anchoredPosition = m_anchoredPosition;
        if (m_MoveStep == 0 && m_MoveElapsedTime >= m_MoveDuration)
        {

            m_MoveStep++;
            m_MoveElapsedTime = 0;
            if (m_moveIn)
                m_MoveDuration = moveIn.duration - m_MoveDuration;
            else
                m_MoveDuration = moveOut.duration - m_MoveDuration;

            if (m_MoveDuration <= 0)
            {
                // Anim ends
                sendMessage = true;
            }
            else
            {
                m_MoveStartPos = m_anchoredPosition;
                if (m_moveIn)
                {
                    m_MoveDistance = Vector2.zero - m_MoveStartPos;
                }
                else
                {
                    m_MoveDistance = m_MoveOutPos - m_MoveStartPos;
                }
            }

        }
        else if (m_MoveStep > 0 && m_MoveElapsedTime >= m_MoveDuration)
        {
            sendMessage = true;
        }

        if (sendMessage)
        {
            if (m_moveIn)
            {
                //Debug.Log("<color=lightred>m_moveIn = false" + gameObject.name + "</color>", gameObject);
                m_moveIn = false;
                if (moveIn.callback.receiver != null && !moveIn.callback.Method.IsNullOrEmpty())
                {
                    moveIn.callback.receiver.SendMessage(moveIn.callback.Method, SendMessageOptions.DontRequireReceiver);
                }
                if (moveIn.deactivateFinished)
                {
                    gameObject.SetActive(false);
                }

            }
            else if (m_moveOut)
            {
                m_moveOut = false;
                if (moveOut.callback.receiver != null && !moveOut.callback.Method.IsNullOrEmpty())
                {
                    //Debug.Log("Send moveout message");
                    moveOut.callback.receiver.SendMessage(moveOut.callback.Method, SendMessageOptions.DontRequireReceiver);
                }
                if (moveOut.deactivateFinished)
                {
                    gameObject.SetActive(false);
                }

            }
        }
    }

    public void MoveOut()
    {
        //Debug.Log("<color=yellow>" + gameObject.name + ", MoveOut()</color>");
        if (fadeOut.enabled)
        {
            StartFadeOut();
        }
        if (moveOut.enabled)
        {
            StartMoveOut();
        }
        if (scaleOut.enabled)
        {
            StartScaleOut();
        }
    }


    private void StartFadeIn()
    {
        m_fadeOut = false;
        m_fadeIn = true;
        m_FadeFunction = Interpolate.Ease(fadeIn.type);
        fadeTimer = 0;
        fadeColor = fadeIn.fadeImage.color;
        fadeColor.a = 0f;
        fadeIn.fadeImage.color = fadeColor;

    }
    private void StartFadeOut()
    {
        m_FadeFunction = Interpolate.Ease(fadeOut.type);
        fadeColor = fadeOut.fadeImage.color;
        m_fadeIn = false;
        m_fadeOut = true;
        fadeTimer = 0f;
    }

    private void UpdateFade()
    {
        if (fadeTimer < fadeIn.fadeTime)
        {
            fadeTimer += Time.deltaTime;
            if (m_fadeIn)
            {
                fadeColor.a = m_FadeFunction(0, fadeIn.fadeMax, fadeTimer, fadeIn.fadeTime);
                fadeIn.fadeImage.color = fadeColor;
            }
            else
            {

                fadeColor.a = m_FadeFunction(0, fadeOut.fadeMax, Mathf.Max(0, fadeOut.fadeTime- fadeTimer), fadeOut.fadeTime);
                fadeOut.fadeImage.color = fadeColor;
            }
        }
        else
        {
            if (m_fadeIn)
            {
                m_fadeIn = false;
                fadeColor.a = fadeIn.fadeMax;
                fadeIn.fadeImage.color = fadeColor;
                if (fadeIn.callback.receiver != null && !fadeIn.callback.Method.IsNullOrEmpty())
                {
                    fadeIn.callback.receiver.SendMessage(fadeIn.callback.Method, SendMessageOptions.DontRequireReceiver);
                }
                if (fadeIn.deactivateFinished)
                {
                    gameObject.SetActive(false);
                }

            }
            else
            {
                m_fadeOut = false;
                fadeColor.a = 0;
                fadeOut.fadeImage.color = fadeColor;

                if (fadeOut.callback.receiver != null && !fadeOut.callback.Method.IsNullOrEmpty())
                {
                    fadeOut.callback.receiver.SendMessage(fadeOut.callback.Method, SendMessageOptions.DontRequireReceiver);
                }

                if (fadeOut.deactivateFinished)
                {
                    gameObject.SetActive(false);
                }
            }
        }

    }

    #region Scale functions
    private bool m_scaleIn = false;
    private bool m_scaleOut = false;
    private float m_scaleDelayTimer;
    private float m_scaleTimer;
    private float m_scaleDuration;
    private int m_scaleStep;
    private Interpolate.Function m_ScaleFunction;
    private Vector2 m_scaleDistance = new Vector2();
    private Vector2 m_scaleCurrent = new Vector2();
    private Vector2 m_scaleStart = new Vector2();

    private void StartScaleIn()
    {
        m_scaleStep = 0;
        m_ScaleFunction = Interpolate.Ease(scaleIn.type);
        m_scaleIn = true;
        m_scaleOut = false;
        m_scaleDelayTimer = 0;
        m_scaleTimer = 0;
        m_scaleStart = scaleIn.scaleStart;
        if (scaleIn.bounce)
        {
            m_scaleDuration = scaleIn.duration * (1 - Mathf.Clamp(bounceTime, 0f, 1f));
            m_scaleDistance = Vector2.one + (Vector2.one * bounceAmount) - m_scaleStart;
        }
        else
        {
            m_scaleDuration = scaleIn.duration;
            m_scaleDistance = Vector2.one - scaleIn.scaleStart;
        }

        rt.localScale = scaleIn.scaleStart;
    }

    private void StartScaleOut()
    {
        m_scaleStep = 0;
        m_ScaleFunction = Interpolate.Ease(scaleOut.type);
        m_scaleIn = false;
        m_scaleOut = true;
        m_scaleDelayTimer = 0;
        m_scaleTimer = 0;
        m_scaleStart = rt.localScale;
        if (scaleOut.bounce)
        {
            m_scaleDuration = scaleOut.duration * bounceTime;
            m_scaleDistance = (m_scaleStart + m_scaleStart * bounceAmount) - m_scaleStart;
        }
        else
        {
            m_scaleDistance = scaleOut.scaleStart - m_scaleStart;
            m_scaleDuration = scaleOut.duration;
        }
    }

    private void UpdateScale()
    {
        bool sendMessage = false;
        if (m_scaleIn)
        {
            if (m_scaleDelayTimer < scaleIn.startDelay)
            {
                m_scaleDelayTimer += Time.deltaTime;
                return;
            }
        }
        else
        {
            if (m_scaleDelayTimer < scaleOut.startDelay)
            {
                m_scaleDelayTimer += Time.deltaTime;
                return;
            }
        }
        m_scaleTimer += Time.deltaTime;
        m_scaleCurrent.x = m_ScaleFunction(m_scaleStart.x, m_scaleDistance.x, m_scaleTimer, m_scaleDuration);
        m_scaleCurrent.y = m_ScaleFunction(m_scaleStart.y, m_scaleDistance.y, m_scaleTimer, m_scaleDuration);
        rt.localScale = m_scaleCurrent;

        if (m_scaleStep == 0 && m_scaleTimer >= m_scaleDuration)
        {
            m_scaleStep++;
            m_scaleTimer = 0;
            if (m_scaleIn)
                m_scaleDuration = scaleIn.duration - m_scaleDuration;
            else
                m_scaleDuration = scaleOut.duration - m_scaleDuration;

            if (m_scaleDuration <= 0)
            {
                // Anim ends
                sendMessage = true;
            }
            else
            {
                m_scaleStart = rt.localScale;
                if (m_scaleIn)
                {
                    m_scaleDistance = Vector2.one - m_scaleStart;
                }
                else
                {
                    m_scaleDistance = scaleOut.scaleStart - m_scaleStart;
                }
            }
        }
        else if (m_scaleStep > 0 && m_scaleTimer >= m_scaleDuration)
        {
            sendMessage = true;
        }

        if (sendMessage)
        {
            if (m_scaleIn)
            {
                m_scaleIn = false;
                if (scaleIn.callback.receiver != null && !scaleIn.callback.Method.IsNullOrEmpty())
                {
                    scaleIn.callback.receiver.SendMessage(scaleIn.callback.Method, SendMessageOptions.DontRequireReceiver);
                }
                if (scaleIn.deactivateFinished)
                {
                    gameObject.SetActive(false);
                }

            }
            else if (m_scaleOut)
            {
                m_scaleOut = false;
                if (scaleOut.callback.receiver != null && !scaleOut.callback.Method.IsNullOrEmpty())
                {
                    scaleOut.callback.receiver.SendMessage(scaleOut.callback.Method, SendMessageOptions.DontRequireReceiver);
                }
                if (scaleOut.deactivateFinished)
                {
                    gameObject.SetActive(false);
                }

            }
        }

    }
    #endregion
}
