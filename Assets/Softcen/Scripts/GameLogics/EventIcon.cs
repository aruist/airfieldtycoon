using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Beebyte.Obfuscator;
using TMPro;

public class EventIcon : MonoBehaviour {

	public TextMeshProUGUI txtDescription;

	public Image imgDuration;
	public GameObject goParticle;
    public AudioClip acStart;

	//private float _timer;
	//private float _duration;
	private EventManager.boostType currentType;
	//private bool m_MoveOut;

    private SCUIAnim uiAnim;

    void Awake()
    {
        uiAnim = GetComponent<SCUIAnim>();
    }

    // Update is called once per frame
    /*void Update () {
        if (m_MoveOut)
            return;

		_timer += Time.deltaTime;
		if (_timer >= _duration) {
			m_MoveOut = true;
			imgDuration.fillAmount = 1f;
			if (currentType == EventManager.boostType.IdleBoost) {
				GameManager.Instance.playerData.SetIdleMultipler(1f);
			}
			else if (currentType == EventManager.boostType.TapBoost) {
				GameManager.Instance.playerData.SetTapMultipler(1f);
			}
            else if (currentType == EventManager.boostType.AutoTap)
            {
                GameManager.Instance.autotap_tryout = false;
            }
            goParticle.SetActive(true);
            if (uiAnim != null)
            {
                uiAnim.MoveOut();
            }
        }
		else {
			imgDuration.fillAmount = _timer / _duration;            
        }
	}*/
    /*private void OnEnable()
    {
        StartBoost(30, 1.5, EventManager.boostType.TapBoost);
    }*/
    public void StartBoost(float time, double value, EventManager.boostType type) {
        if (acStart != null)
        {
            AudioManager.Instance.PlayClip(acStart);
        }
		//m_MoveOut = false;
		//_timer = 0f;
		//_duration = time;
		currentType = type;
		string txt = value.ToString("F1") + "X";
#if SOFTCEN_DEBUG
		Debug.Log("ClaimReward StartBoost value txt: " + txt + ", time: " + time + ", value: " + value.ToString() + ", type: " + type);
#endif
		txtDescription.SetText(txt);
		imgDuration.fillAmount = 0f;
		if (currentType == EventManager.boostType.IdleBoost) {
			GameManager.Instance.playerData.SetIdleMultipler(value);
		}
		else if (currentType == EventManager.boostType.TapBoost) {
			GameManager.Instance.playerData.SetTapMultipler(value);
		}
        else if (currentType == EventManager.boostType.AutoTap)
        {
            GameManager.Instance.autotap_tryout = true;
        }

		goParticle.SetActive(true);
		gameObject.SetActive(true);
	}

    public void MoveOut()
    {
        if (uiAnim != null)
        {
            uiAnim.MoveOut();
        }
    }

    [SkipRename]
	public void CloseDone() {
		//m_MoveOut = false;
		gameObject.SetActive(false);
	}
}
