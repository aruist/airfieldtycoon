using UnityEngine;
using System.Collections;
using System;

public class IdleManager : MonoBehaviour {
	public PopUpCoinPool idleCoinPool;
	public Transform trPosPopUp;
    [SerializeField]
	private double currentIdleValue;
    [SerializeField]
    private float timer;

    private GameManager gm;
    private Pilvipalvelut pp;
	//private GameObject go;
	//private string strValue;
	private Vector3 scorePos;

    private bool eventsSubscribed = false;
	// Use this for initialization
	void Start () {
		gm = GameManager.Instance;
        pp = Pilvipalvelut.Instance;
		currentIdleValue = gm.playerData.GetIdleValue();
		timer = 0f;
        if (trPosPopUp != null)
		    scorePos = trPosPopUp.position;
	}
	void OnEnable() {
        SubscribeEvents();
	}

	void OnDisable() {
        UnSubscribeEvents();
	}

	void HandleOnBonusesChanged ()
	{
        if (GameManager.Instance == null || GameManager.Instance.playerData == null)
            return;
		currentIdleValue = GameManager.Instance.playerData.GetIdleValue();
    }

    private void SubscribeEvents() {
        if (!eventsSubscribed) {
            eventsSubscribed = true;
            PlayerData.OnBonusesChanged += HandleOnBonusesChanged;
        }
    }
    private void UnSubscribeEvents() {
        if (eventsSubscribed) {
            eventsSubscribed = false;
            PlayerData.OnBonusesChanged -= HandleOnBonusesChanged;
        }
    }

	// Update is called once per frame
	void Update () {
        if (pp.loggedOut)
            return;

		timer += Time.deltaTime;
		if (timer >= 1f) {
			timer -= 1f;
			if (currentIdleValue > 0)
            {
                if (idleCoinPool != null)
                {
                    idleCoinPool.ActivateCoin(scorePos, currentIdleValue);
                }
                else
                {
                    gm.playerData.IncMoney(currentIdleValue);
                }
            }
		}
	}

}
