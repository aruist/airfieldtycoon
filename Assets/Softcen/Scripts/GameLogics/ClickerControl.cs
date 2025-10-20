using UnityEngine;
using System.Collections;
//using System;

public class ClickerControl : MonoBehaviour {

	public ObjectPooler clickPressPool;
	public PopUpCoinPool coinPool;

	//public TapScorePool poolTapScore;
	public Transform trBottom;
	public Transform trTop;
	public AudioClip acTap;
	public Camera cam;

	private int[] fingerIDs;
	private Touch t;
	private Vector3 tpos;
	private GameManager gm;
	private Pilvipalvelut pp;

    public Animator animFisherman;

	private double currentTapValue;

    public float autoClickSensitivy = 0.5f;
    private float m_AutoClickTimer;

    private bool m_AutoClickerOwned;

    public bool DemoClickerEnable = false;
    public float DemoClickDelta = 1f;

    public Transform[] trDemoClickerPos;
    public float DemoautoClickSensitivy = 0.5f;

    // Use this for initialization
    void Start () {
		gm = GameManager.Instance;
		pp = Pilvipalvelut.Instance;
        m_AutoClickTimer = 0;
        //cam = Camera.main;
        fingerIDs = new int[10];
		for (int i=0; i < fingerIDs.Length; i++) {
			fingerIDs[i] = -1;
		}
		currentTapValue = gm.playerData.GetTapValue();
        m_AutoClickerOwned = GameManager.Instance.playerData.AutoTapOwned;
    }
	
	// Update is called once per frame
	void Update () {
        if (pp.loggedOut)
            return;

        m_AutoClickTimer += Time.deltaTime;
        if (!gm.tapDisabled)
			CheckTap();
        if (m_AutoClickTimer >= autoClickSensitivy)
            m_AutoClickTimer = 0;
    }

    void OnEnable() {
		PlayerData.OnBonusesChanged += HandleOnBonusesChanged;
        PlayerData.OnStoreItemChanged += PlayerData_OnStoreItemChanged;
	}

    private void PlayerData_OnStoreItemChanged()
    {
        m_AutoClickerOwned = GameManager.Instance.playerData.AutoTapOwned;
    }


    void OnDisable() {
		PlayerData.OnBonusesChanged -= HandleOnBonusesChanged;
        PlayerData.OnStoreItemChanged -= PlayerData_OnStoreItemChanged;
    }

    void HandleOnBonusesChanged ()
	{
        if (gm != null) {
            if (gm.playerData != null)
                currentTapValue = gm.playerData.GetTapValue();
        }
	}



	private void CheckTap() {
        bool autoclicked = false;
        int touchCount = Input.touchCount;
		if (touchCount > 0) {
			for (int i=0; i < touchCount; i++) {
				t = Input.GetTouch(i);
				if (t.phase == TouchPhase.Began) {
					RegisterFingerId(t.fingerId, cam.ScreenToWorldPoint(t.position));
					/*if (t.position.y < trTop.position.y && t.position.y > trBottom.position.y) {
						RegisterFingerId(t.fingerId);
					}*/
				}
				else if (t.phase == TouchPhase.Ended) {
					tpos = cam.ScreenToWorldPoint(t.position);
					if (UnRegisterFingerId(t.fingerId, tpos)) {
						//Debug.Log("Score++");
						//GameObject go = poolTapScore.GetPooledObject();
						tpos.z = 0;
						//go.transform.position = tpos;
						//go.SetActive(true);
						//AudioManager.Instance.PlayClip(acTap);
						//gm.playerData.IncMoney(currentTapValue, true);
						Clicked(tpos, currentTapValue);
					}
				}
				else if (t.phase == TouchPhase.Canceled) {
					CancelFingerId(t.fingerId);
				}
                else if (t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved)
                {
                    if (m_AutoClickerOwned || gm.autotap_tryout)
                    {
                        if (!autoclicked && m_AutoClickTimer >= autoClickSensitivy)
                        {
                            autoclicked = true;
                            tpos = cam.ScreenToWorldPoint(t.position);
                            tpos.z = 0;
                            Clicked(tpos, currentTapValue);
                        }

                    }
                }
            }
			return;
		}
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) {
			RegisterFingerId(10, cam.ScreenToWorldPoint(Input.mousePosition));
            return;
		}
		if (Input.GetMouseButtonUp(0)) {
			tpos = cam.ScreenToWorldPoint(Input.mousePosition);
			if (UnRegisterFingerId(10, tpos)) {
				//Debug.Log("Score++");
				//GameObject go = poolTapScore.GetPooledObject();
				tpos.z = 0;
				//go.transform.position = tpos;
				//go.SetActive(true);
				//gm.playerData.IncMoney(currentTapValue, true);
				//AudioManager.Instance.PlayClip(acTap);
				Clicked(tpos, currentTapValue);
                return;
			}
		}
        if (Input.GetMouseButton(0))
        {
            if (m_AutoClickerOwned || gm.autotap_tryout)
            {
                if (m_AutoClickTimer >= autoClickSensitivy)
                {
                    tpos = cam.ScreenToWorldPoint(Input.mousePosition);
                    tpos.z = 0;
                    Clicked(tpos, currentTapValue);
                }
            }

        }
#endif
#if SOFTCEN_DEBUG
        if (DemoClickerEnable)
        {
            autoClickSensitivy = DemoautoClickSensitivy;
            if (m_AutoClickTimer >= autoClickSensitivy)
            {
                for (int i=0; i < trDemoClickerPos.Length; i++)
                {
                    tpos = trDemoClickerPos[i].position; //cam.ScreenToWorldPoint(trDemoClickerPos[i].position);
                    tpos.x = Random.Range(tpos.x - DemoClickDelta, tpos.x + DemoClickDelta);
                    tpos.z = 0;
                    Clicked(tpos, currentTapValue);
                }
            }

        }
#endif
    }

    private void RegisterFingerId(int id, Vector3 pos) {
		bool found = false;
		int freeIndex = -1;

        if ((pos.y < trTop.position.y && pos.y > trBottom.position.y) ) {
			for (int i=0; i < fingerIDs.Length; i++) {
				if (fingerIDs[i] == -1) {
					freeIndex = i;
				}
				if (fingerIDs[i] == id) {
					found = true;
					break;
				}
			}
			if (!found && freeIndex != -1) {
				//Debug.Log("Finger Id: " + id + " registered!");
				fingerIDs[freeIndex] = id;
			    return;    
			}
		}
		/*Debug.Log("Fail Finger Id: " + id
		          + ", y: " + pos.y
		          + ", range: " +trTop.position.y
		          + " - " + trBottom.position.y
		          );*/


	}
	private bool IsFingerIdRegistered(int id) {
		for (int i=0; i < fingerIDs.Length; i++) {
			if (fingerIDs[i] == id) {
				return true;
			}
		}
		return false;

	}
	private void CancelFingerId(int id) {
		for (int i=0; i < fingerIDs.Length; i++) {
			if (fingerIDs[i] == id) {
				fingerIDs[i] = -1;
			}
		}
	}
	private bool UnRegisterFingerId(int id, Vector3 pos) {
		for (int i=0; i < fingerIDs.Length; i++) {
			if (fingerIDs[i] == id) {
				fingerIDs[i] = -1;
                if ((pos.y < trTop.position.y && pos.y > trBottom.position.y)  ) {
					return true;
				}
			}
		}
		return false;
	}

	private void Clicked(Vector3 pos, double tapVal) {
        if (animFisherman != null)
        {
            animFisherman.SetTrigger("Catch");
        }
		coinPool.ActivateCoin(pos, tapVal);
		AudioManager.Instance.PlayClip(acTap);
		if (clickPressPool != null) {
			GameObject go = clickPressPool.GetPooledObject();
			go.transform.position = pos;
			go.SetActive(true);
            MissionManager.Instance.IncreaseTap();

#if SOFTCEN_DEBUG && SOFTCEN_SPEEDTEST
            long speedVal = 20;
			go = clickPressPool.GetPooledObject();
			pos.x += 0.5f;
			go.transform.position = pos;
			go.SetActive(true);
            MissionManager.Instance.IncreaseTap();
            coinPool.ActivateCoin(pos, tapVal* speedVal);
			go = clickPressPool.GetPooledObject();
			pos.x += 0.5f;
			go.transform.position = pos;
			go.SetActive(true);
            MissionManager.Instance.IncreaseTap();
            coinPool.ActivateCoin(pos, tapVal* speedVal);
			go = clickPressPool.GetPooledObject();
			pos.x += 0.5f;
			go.transform.position = pos;
			go.SetActive(true);
            MissionManager.Instance.IncreaseTap();
            coinPool.ActivateCoin(pos, tapVal* speedVal);
			go = clickPressPool.GetPooledObject();
			pos.x += 1.0f;
			go.transform.position = pos;
			go.SetActive(true);
            MissionManager.Instance.IncreaseTap();
            coinPool.ActivateCoin(pos, tapVal* speedVal);
#endif
		}
	}

}
