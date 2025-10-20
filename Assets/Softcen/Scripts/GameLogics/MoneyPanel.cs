using UnityEngine;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class MoneyPanel : MonoBehaviour {
	public TextMeshProUGUI txtMoney;
	public TextMeshProUGUI txtIdleMoney;
    public Image imageCoinBig;
    public float coinFlashTimeout = 0.3f;

    private double updatedIdleValue = -1f;
    private double updatetMoneyValue = -1f;

    private double prevMoney = -1f;
    private GameManager gm;
    private float secTimer;

    private bool eventSubscribed = false;
    private bool m_UpdateMoneyPending = false;
    private float m_coinTimer;

    private Color coinColor;
    private bool _initialized = false;

    void Start() {
        gm = GameManager.Instance;
        coinColor = imageCoinBig.color;
        secTimer = 0f;
        prevMoney = gm.playerData.Money;
        if (txtIdleMoney != null)
            txtIdleMoney.SetText ("");
        UpdateMoney();
        setCoin(false);
        _initialized = true;
    }

    private void setCoin(bool state)
    {
        if (state)
        {
            coinColor.a = 1f;
        } else
        {
            coinColor.a = 0f;
        }
        imageCoinBig.color = coinColor;
    }

    void OnEnable() {
        if (txtMoney != null && !eventSubscribed) {
            eventSubscribed = true;
            PlayerData.OnMoneyChanged += HandleOnMoneyChanged;
            PlayerData.OnBonusesChanged += HandleOnBonusesChanged;
        }
        if (_initialized)
        {
            setCoin(false);
            UpdateMoney();
            if (gm != null && txtIdleMoney != null)
            {
                prevMoney = gm.playerData.Money;
            }
        }
    }
    void OnDisable() {
        if (eventSubscribed) {
            eventSubscribed = false;
            PlayerData.OnMoneyChanged -= HandleOnMoneyChanged;
            PlayerData.OnBonusesChanged -= HandleOnBonusesChanged;
        }
    }


    void Update() {
        if (txtIdleMoney == null) {
            return;
        }
        secTimer += Time.deltaTime;
        if (secTimer >= 1f) {
            secTimer -= 1f;
            double diff = gm.playerData.Money - prevMoney;
            if (txtIdleMoney != null && updatedIdleValue != diff) {
                updatedIdleValue = diff;
                if (diff < 0) {
                    txtIdleMoney.SetText ("0/sec");
                }
                else {
                    txtIdleMoney.SetText (NumToStr.GetNumStr(diff) + "/sec");
                }
            }

            prevMoney = gm.playerData.Money;
        }

        m_coinTimer += Time.deltaTime;
        if (m_coinTimer > coinFlashTimeout)
        {
            m_coinTimer = 0;
            setCoin(false);
            //if (imageCoinBig.enabled)
            //{
            //    imageCoinBig.enabled = false;
            //}
        }

        if (m_UpdateMoneyPending)
        {
            setCoin(true);
            m_coinTimer = 0;
            //if (!imageCoinBig.enabled)
            //{
            //    imageCoinBig.enabled = true;
            //}
            UpdateMoney();
        }

    }


    private void UpdateMoney() {
        m_UpdateMoneyPending = false;
        if (GameManager.Instance == null)
			return;
        double money = GameManager.Instance.playerData.Money;
		if (txtMoney != null && money != updatetMoneyValue) {
            updatetMoneyValue = money;
            txtMoney.SetText(NumToStr.GetNumStr(money));
        }
	}

    void HandleOnBonusesChanged ()
	{
        m_UpdateMoneyPending = true;
	}

	void HandleOnMoneyChanged (double val)
	{
        m_UpdateMoneyPending = true;
	}


}
