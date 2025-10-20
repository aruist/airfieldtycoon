using UnityEngine;
using TMPro;
using System.Text;

public class PopupCoin : MonoBehaviour {
    public float m_RotateSpeed = 1f;
    public Vector3 m_RotateDirection;
    public Transform m_trToRotate;
    public TextMeshPro txtPrice;
    //public TextMesh txtPrice;
	public Transform trTarget1;
	public Transform trTarget2;
	public float speed = 1.0f;
	public float timeToGoUp = 1f;
	public float deltaUp = 1f;
	public float timeToGoCoins = 2f;
	public PopUpCoinPool popUpCoinPool;
	public AudioClip coinAudio;

	//private Transform trTarget;
	private float deltaTime;
	private int state;
	private Transform trOwn;
	private Vector3 pos;
	private Color txtColor;
	private Vector3 startPos;
	private Vector3 endPos;

	private double setMoney = -1;

	public float dblTime;

    private StringBuilder sbMoney;
    private string strMoney;

    public bool startInit;

    void Start()
    {
        sbMoney = new StringBuilder(16);
        sbMoney.Append("xxxxxxxxxx.xxK");
        NumToStr.GetNumStr(setMoney, sbMoney);
        //strMoney = NumToStr.GetNumStr GarbageFreeString(sbMoney);
		if (txtPrice != null)
        {
			txtPrice.SetText(sbMoney.ToString());
		}
	}

	void OnEnable() {
		//trTarget = trTarget1;
		deltaTime = 0f;
		state = 0;
		trOwn = transform;
		pos = transform.position;
		startPos.y = pos.y;
		endPos.y = pos.y+deltaUp;
		//txtColor = txtPrice.color;
		//txtColor.a = 0f;
		//txtPrice.color = txtColor;
	}

	public void ActivateCoin(Vector3 pos, double m) {
		SetMoney(m);
		transform.position = pos;
		gameObject.SetActive(true);
	}

	public void SetMoney(double m) {
		//Debug.Log ("SetMoney: " + m + ", sbMoney: " + sbMoney.ToString());
		if (setMoney != m) {
			setMoney = m;
			if (sbMoney == null)
            {
				sbMoney = new StringBuilder(16);
				sbMoney.Append("xxxxxxxxxx.xxK");
			}
			NumToStr.GetNumStr(setMoney, sbMoney);
            sbMoney.Append("\0");
			//strMoney = NumToStr.GarbageFreeString(sbMoney);
			if (txtPrice != null)
            {
				txtPrice.SetText(sbMoney.ToString());
			}
			//txtPrice.SetText("+" + NumToStr.GetNumStr(setMoney));
			//txtPrice.text = "+" + NumToStr.GetNumStr(setMoney);
		}
	}
	// Update is called once per frame
	void Update () {
        if (startInit)
        {
            startInit = false;
            gameObject.SetActive(false);
        }

		deltaTime += Time.deltaTime;
		if (state == 0) {
			dblTime = deltaTime/timeToGoUp;
			pos.y = Mathf.Lerp(startPos.y,endPos.y,dblTime);
			//txtColor.a = Mathf.Lerp(0f,1f,dblTime);
			//txtPrice.color = txtColor;
			trOwn.position = pos;
			if (deltaTime > timeToGoUp ) {
				deltaTime = 0f;
				state = 1;
				startPos = pos;
				endPos = trTarget1.position;
			}

			return;
		}
		if (state == 1) {
			dblTime = deltaTime/timeToGoCoins;
			trOwn.position = Vector3.LerpUnclamped(startPos, endPos, dblTime);
            //txtColor.a = Mathf.Lerp(1f,0f,dblTime);
            //txtPrice.color = txtColor;
            m_trToRotate.Rotate(m_RotateDirection * m_RotateSpeed * Time.deltaTime);

            if (deltaTime > timeToGoCoins ) {
				state = 0;
				gameObject.SetActive(false);
				popUpCoinPool.AddToList(this);
				if (GameManager.Instance != null && GameManager.Instance.playerData != null)
                {
					GameManager.Instance.playerData.IncMoney(setMoney);
				}
				if (AudioManager.Instance != null)
                {
					AudioManager.Instance.PlayClip(coinAudio);
				}
			}
		}
	}
}
