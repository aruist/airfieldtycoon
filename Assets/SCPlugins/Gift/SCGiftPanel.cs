#if USE_SCGIFT

using UnityEngine;
using UnityEngine.UI;


public class SCGiftPanel : MonoBehaviour {

    public Text txtRemainTime;

    private SCGift scGift;
    private float m_timer;
    private bool m_Initialized = false;
	// Use this for initialization
	void Start () {
        scGift = SCGift.get;
        m_timer = 0f;
        txtRemainTime.text = scGift.GetRemainTimeStr();
        m_Initialized = true;
    }

    void OnEnable()
    {
        if (m_Initialized)
            txtRemainTime.text = scGift.GetRemainTimeStr();
    }

    // Update is called once per frame
    void Update () {
        if (scGift.GetCurrentGift() == null)
        {
            gameObject.SetActive(false);
        }

        m_timer += Time.deltaTime;
        if (m_timer > 1)
        {
            m_timer = 0f;
            txtRemainTime.text = scGift.GetRemainTimeStr();
        }
	}
}
#endif
