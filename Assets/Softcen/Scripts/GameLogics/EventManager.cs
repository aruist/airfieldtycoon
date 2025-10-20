using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Beebyte.Obfuscator;

public class EventManager : MonoBehaviour {
	public static EventManager Instance;
	public List<EventData> eventList;

	public enum boostType {
		TapBoost,
		IdleBoost,
		InstantTap,
		DoubleBonus,
        AutoTap,
        TuplaaIlmainenArkku
	}

	private int m_RewardStart;
	private int m_RewardEnd;
	private int m_DoubleBonusId;
    private int m_AutoTapId;
    private int m_IlmainenArkkuId;

	void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
		InitList();
	}

	public EventData GetRandomRewardEvent() {
		int index = Random.Range(m_RewardStart, m_RewardEnd);
		return eventList[index];
	}

	public EventData GetDoubleBonusEvent() {
		return eventList[m_DoubleBonusId];
	}

    public EventData GetIlmainenArkkuEvent() {
        return eventList[m_IlmainenArkkuId];
    }

    public int IlmainenArkkuEventId() {
        return m_IlmainenArkkuId;
    }

    public EventData GetAutoTapEvent()
    {
        return eventList[m_AutoTapId];
    }

    [ObfuscateLiterals]
    private void InitList() {
		if (eventList == null) {
			eventList = new List<EventData>();
		}
		else {
			eventList.Clear();
		}
		int id = 0;
		// Reward events
		m_RewardStart = id;
        string instantboost = "INSTANT BOOST";
        string tapboost = "TAP BOOST";
        string seconds = " seconds!";
        string tapIncomeIncreased = "Tap Income Increased by ";
        eventList.Add(new EventData(id++, boostType.InstantTap, instantboost, "Earn 100x Tap Income Immediately!", 100d, 0f, 0));
		eventList.Add(new EventData(id++, boostType.InstantTap, instantboost, "Earn 150x Tap Income Immediately!", 150d, 0f, 0));
        eventList.Add(new EventData(id++, boostType.InstantTap, instantboost, "Earn 200x Tap Income Immediately!", 200d, 0f, 0));
        eventList.Add(new EventData(id++, boostType.InstantTap, instantboost, "Earn 250x Tap Income Immediately!", 250d, 0f, 0));
        eventList.Add(new EventData(id++, boostType.InstantTap, instantboost, "Earn 300x Tap Income Immediately!", 300d, 0f, 0));
        eventList.Add(new EventData(id++, boostType.InstantTap, instantboost, "Earn 350x Tap Income Immediately!", 350d, 0f, 0));
        eventList.Add(new EventData(id++, boostType.InstantTap, instantboost, "Earn 400x Tap Income Immediately!", 400d, 0f, 0));

        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "1.1X for 60" + seconds, 1.1d, 60f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "1.2X for 60" + seconds, 1.2d, 60f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "1.3X for 60" + seconds, 1.3d, 60f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "1.4X for 60" + seconds, 1.4d, 60f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "1.5X for 60" + seconds, 1.5d, 60f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "1.6X for 60" + seconds, 1.6d, 60f, 0));

        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "2X for 30" + seconds, 2d, 30f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "2.4X for 30" + seconds, 2.4d, 30f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "2.7X for 30" + seconds, 2.7d, 30f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "3X for 30" + seconds, 3d, 30f, 0));

        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "4X for 15" + seconds, 4d, 15f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "4.5X for 15" + seconds, 4.5d, 15f, 0));
        eventList.Add(new EventData(id++, boostType.TapBoost, tapboost, tapIncomeIncreased + "5X for 15" + seconds, 5d, 15f, 0));
        string idleboost = "IDLE BOOST";
        string idleIncomeIncreased = "Idle Income Increased by ";
        string for120seconds = " for 120 seconds!";
        eventList.Add(new EventData(id++, boostType.IdleBoost, idleboost, idleIncomeIncreased +"1.1X" + for120seconds, 1.1d, 120f, 0));
        eventList.Add(new EventData(id++, boostType.IdleBoost, idleboost, idleIncomeIncreased + "1.2X" + for120seconds, 1.2d, 120f, 0));
        eventList.Add(new EventData(id++, boostType.IdleBoost, idleboost, idleIncomeIncreased + "1.3X" + for120seconds, 1.3d, 120f, 0));
        eventList.Add(new EventData(id++, boostType.IdleBoost, idleboost, idleIncomeIncreased + "1.4X" + for120seconds, 1.4d, 120f, 0));
        eventList.Add(new EventData(id++, boostType.IdleBoost, idleboost, idleIncomeIncreased + "1.5X" + for120seconds, 1.5d, 120f, 0));
        eventList.Add(new EventData(id++, boostType.IdleBoost, idleboost, idleIncomeIncreased + "1.6X" + for120seconds, 1.6d, 120f, 0));
        eventList.Add(new EventData(id++, boostType.IdleBoost, idleboost, idleIncomeIncreased + "1.7X" + for120seconds, 1.7d, 120f, 0));
        eventList.Add(new EventData(id++, boostType.IdleBoost, idleboost, idleIncomeIncreased + "1.8X" + for120seconds, 1.8d, 120f, 0));
        eventList.Add(new EventData(id++, boostType.IdleBoost, idleboost, idleIncomeIncreased + "1.9X" + for120seconds, 1.9d, 120f, 0));
		m_RewardEnd = id;
        m_DoubleBonusId = id;
        eventList.Add(new EventData(id++, boostType.DoubleBonus, "DOUBLE BONUS","2 X Bonus!", 0d, 0f, 0));
        m_AutoTapId = id;
        eventList.Add(new EventData(id++, boostType.AutoTap, "Auto Tap", "Hold finger down to auto tap!", GameConsts.AutoTap.TryTime, GameConsts.AutoTap.TryTime, 0));

        m_IlmainenArkkuId = id;
        eventList.Add (new EventData(id++, boostType.TuplaaIlmainenArkku, "", "", 0d, 0f, 0));
    }
}
