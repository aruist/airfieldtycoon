using UnityEngine;
using System.Collections;

public class RewardArea : MonoBehaviour {
	public enum rewardTypes {
		RewardEvent,
		LuckyDraw,
        AutoTap
	}
	public float timeToDisplay = 60f;
	public rewardTypes rewardType;

	public GameObject goParticle;
	public Transform trRewardButton;

    private bool closeCalled = false;
	private float _timer;
    private SCUIAnim _uiAnim;
    private SCUIAnim uiAnim
    {
        get
        {
            if (_uiAnim == null)
            {
                _uiAnim = GetComponent<SCUIAnim>();
            }
            return _uiAnim;
        }
    }
    void Awake()
    {
        _uiAnim = GetComponent<SCUIAnim>();
    }

    // Use this for initialization
    void OnEnable () {
        if (goParticle != null)
		    goParticle.SetActive(true);
		_timer = 0f;
        closeCalled = false;
    }

    void Update() {
        if (closeCalled)
            return;

		_timer += Time.deltaTime;
		if (_timer >= timeToDisplay)
        {
            closeCalled = true;
            CloseReward();
		}
	}

	public void CloseReward() {
        uiAnim.MoveOut();
	}

	public void CloseDone() {
		gameObject.SetActive(false);
        if (rewardType == rewardTypes.RewardEvent)
            HomeManager.Instance.ResetRevardTimer();
        else if (rewardType == rewardTypes.AutoTap)
            HomeManager.Instance.ResetAutoTapRewardTimer();
	}
}
