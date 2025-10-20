using UnityEngine;

public class MissionItem {

    public MissionTypes.type type;
    private int targetCount;
    private int currentCount;
    public FishTypes.type currentFishType;
    private double bonusCoins;
    private bool palkintoLaskettu;

    public MissionItem()
    {
        type = MissionTypes.type.None;
        targetCount = 0;
        currentCount = 0;
        currentFishType = FishTypes.type.None;
        palkintoLaskettu = false;
    }

    public void LaskePalkinto() {
        if (!palkintoLaskettu) {
            palkintoLaskettu = true;
            double idle = GameManager.Instance.playerData.GetIdleValue ();
            double tap = 4d * GameManager.Instance.playerData.GetTapValue();
            float rnd = Random.Range (20f, 50f);
            bonusCoins = rnd * (idle + tap);
        }
    }
    public MissionItem(MissionTypes.type t, int count, FishTypes.type fishType)
    {
        currentFishType = fishType;
        type = t;
        targetCount = count;
        currentCount = 0;
        palkintoLaskettu = false;
    }

    public MissionItem(MissionTypes.type t, int count)
    {
        type = t;
        targetCount = count;
        currentCount = 0;
        currentFishType = FishTypes.type.None;
        palkintoLaskettu = false;
    }

    public void ResetMission(MissionTypes.type t, int count, FishTypes.type fishType, double bonus)
    {
        type = t;
        targetCount = count;
        currentFishType = fishType;
        currentCount = 0;
        bonusCoins = bonus;
        palkintoLaskettu = false;
    }

    public void SetMission(MissionTypes.type t, int targetcount, int currentcount, FishTypes.type fishType, double bonus, bool l)
    {
        type = t;
        targetCount = targetcount;
        currentFishType = fishType;
        currentCount = currentcount;
        bonusCoins = bonus;
        palkintoLaskettu = l;
        if (MissionManager.Instance != null)
            MissionManager.Instance.UpdateAllTexts();
    }

    public double BonusCoins
    {
        get { return bonusCoins; }
    }

    public double GetBonus()
    {
        return bonusCoins;
    }

    public bool IsMissionFinished()
    {
        if (currentCount >= targetCount)
            return true;

        return false;
    }

    public bool AddCount()
    {
        currentCount++;
        if (currentCount >= targetCount)
            return true;
        return false;
    }

    public int TargetCount
    {
        get { return targetCount; }
    }

    public int CurrentCount
    {
        get { return currentCount; }
    }
    public int CountLeft
    {
        get
        {
            if (targetCount > currentCount)
                return targetCount - currentCount;
            return 0;
        }
    }

    public void SetBonusCoins(double coins)
    {
        bonusCoins = coins;
    }

    public bool Laskettu
    {
        get { return palkintoLaskettu; }
    }

    #if UNITY_EDITOR
    public void paataTehtava() {
        currentCount = targetCount + 1;
    }
    #endif
}
