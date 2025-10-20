using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerData
{
    private const int _version = 23;

    public static event Action<double> OnMoneyChanged;
    public static event Action OnBonusesChanged;
    public static event Action OnDiamondsChanged;
    public static event Action OnLevelChanged;
    public static event Action OnStoreItemChanged;
    public static event Action OnScoreChanged;
    public static event Action OnMissionUpdated;

    public static event Action OnTapUpgrade;
    public static event Action OnIdleUpgrade;

    public List<ItemCount> IdleBonusCount;
    public List<ItemCount> TapBonusCount;
    public List<ItemCount> UpgradeCount;
    public List<int> PowerUpList;

    public MissionItem CurrentMission;
    private long m_ilmainenArkkuAvaamisAika;
    public long IlmainenArkkuAvaamisAika {
        get { return m_ilmainenArkkuAvaamisAika; }
        set { 
            m_ilmainenArkkuAvaamisAika = value;
            _changed = true;
        }
    }

    public PlayerData()
    {
        Init();
    }

    public bool CloudEnabled = true;

    private int m_currentChapter = 0;
    public int CurrentChapter
    {
        get { return m_currentChapter; }
        set
        {
            if (value != m_currentChapter)
            {
                m_currentChapter = value;
            }
        }
    }
    public void IncCurrentChapter()
    {
        m_currentChapter++;
        m_CurrentPhase = 0;
        IncLevel();
#if SOFTCEN_DEBUG
        Debug.Log("IncCurrentChapter Chapter: "
            + CurrentChapter
            + ", Phase: " + CurrentPhase
            + ", Level: " + Level 
            );
#endif
    }

    public bool LoggedOut;

    public long _Score;
    public long Score
    {
        get {  return (1251 ^ _Score) - 117; }
        set
        {
            long v = (value + 117) ^ 1251;
            if (v != _Score && value >= 0)
            {
                _Score = v;
                if (OnScoreChanged != null)
                    OnScoreChanged();
            }
        }
    }

    public int _PrestigeCount;
    public int PrestigeCount
    {
        get { return (237531 ^ _PrestigeCount) - 3315; }
        set {
            int newCount = (value + 3315) ^ 237531;
            if (newCount != _PrestigeCount)
            {
                _PrestigeCount = newCount;
                _changed = true;
            }
        }
    }
    public int _PrestigeLevel;
    public int PrestigeLevel
    {
        get { return (235533 ^ _PrestigeLevel) - 4317; }
        set
        {
            int newValue = (value + 4317) ^ 235533;
            if (newValue != _PrestigeLevel)
            {
                _PrestigeLevel = newValue;
                _changed = true;
            }
        }
    }

    public int m_CurrentPhase = 0;
    public int CurrentPhase
    {
        get { return m_CurrentPhase; }
        set
        {
            if (value != m_CurrentPhase)
            {
                m_CurrentPhase = value;
                if (OnLevelChanged != null)
                {
                    OnLevelChanged();
                }
            }
        }
    }

    public int version
    {
        get { return _version; }
    }
    private bool _changed;
    public bool Changed { get { return _changed; } }

    public bool m_autotapOwned = false;
    public bool AutoTapOwned {
        get { return m_autotapOwned; }
        set
        {
            if (value != m_autotapOwned)
            {
                m_autotapOwned = value;
                if (OnStoreItemChanged != null)
                    OnStoreItemChanged();
            }
        }
    }


    private float m_AudioVol;
    public float AudioVol
    {
        get { return m_AudioVol;  } 
        set
        {
            if (value != m_AudioVol)
            {
                _changed = true;
                m_AudioVol = value;
            }
        }
    }
    private float m_MusicVol;
    public float MusicVol
    {
        get { return m_MusicVol; }
        set
        {
            if (value != m_MusicVol)
            {
                _changed = true;
                m_MusicVol = value;
            }
        }
    }

    public void CallOnLevelChanged()
    {
        if (OnLevelChanged != null)
            OnLevelChanged();
    }

    public void CallOnMissionUpdated() {
        if (OnMissionUpdated != null)
            OnMissionUpdated ();
    }

    public void Init()
    {
        m_ilmainenArkkuAvaamisAika = ArKKuTyyPPi.uusiAika (ArKKuTyyPPi.tyyppi.ILMAINEN);

        CloudEnabled = true;
        LoggedOut = false;
        Score = 0;
        PrestigeCount = 0;
        PrestigeLevel = 0;
        MissionCounter = 0;
        CurrentChapter = 0;
        CurrentPhase = 0;
        AutoTapOwned = false;
        CurrentMission = new MissionItem();

        m_AudioVol = 0f;
        m_MusicVol = 0f;

        ResetPowerUps();
        if (IdleBonusCount == null)
            IdleBonusCount = new List<ItemCount>();
        else
        {
            for (int i = 0; i < IdleBonusCount.Count; i++)
                IdleBonusCount[i] = null;

            IdleBonusCount.Clear();
        }
        for (int i= Item.IdleIdStart; i <= Item.IdleIdEnd; i++)
        {
            IdleBonusCount.Add(new ItemCount(i, 0));
        }

        if (TapBonusCount == null)
            TapBonusCount = new List<ItemCount>();
        else
        {
            for (int i = 0; i < TapBonusCount.Count; i++)
                TapBonusCount[i] = null;

            TapBonusCount.Clear();
        }
        for (int i = Item.TapIdStart; i <= Item.TapIdEnd; i++)
        {
            TapBonusCount.Add(new ItemCount(i, 0));
        }



        if (UpgradeCount == null)
            UpgradeCount = new List<ItemCount>();
        else
        {
            for (int i = 0; i < UpgradeCount.Count; i++)
                UpgradeCount[i] = null;

            UpgradeCount.Clear();
        }
        for (int i = Item.LvlIdStart; i <= Item.LvlIdEnd; i++)
        {
            UpgradeCount.Add(new ItemCount(i, 0));
        }

        InitDiamonds();
        AdFree = false;
        _changed = true;
        InitMoney();
        if (OnBonusesChanged != null)
        {
            OnBonusesChanged();
        }
        SetGigCoins(0);
        SetLevel(0);
        SetFamous(0);
        SetSubLevel(0);
        CalculateUpdates();
        likeAsked = false;
        playerLike = false;
        askRateLater = false;
        powerSave = false;
    }


    public bool _likeAsked = false;
    public bool _playerLike = false;
    public bool _askRateLater = false;
    public bool _powerSave = false;

    public bool askRateLater
    {
        get { return _askRateLater; }
        set
        {
            if (_askRateLater != value)
                _changed = true;

            _askRateLater = value;
        }
    }
    public bool playerLike
    {
        get { return _playerLike; }
        set
        {
            if (_playerLike != value)
                _changed = true;
            _playerLike = value;
        }
    }
    public bool powerSave
    {
        get { return _powerSave; }
        set
        {
            if (_powerSave != value)
                _changed = true;
            _powerSave = value;
        }
    }
    public bool likeAsked
    {
        get { return _likeAsked; }
        set
        {
            if (_likeAsked != value)
                _changed = true;

            _likeAsked = value;
        }
    }

    public int _famous;
    public int Famous
    {
        get { return (327531 ^ _famous) - 2315; }
    }
    public void SetFamous(int c)
    {
        _famous = (c + 2315) ^ 327531;
        _changed = true;
    }
    public void IncFamous(int c)
    {
        int v = Famous;
        v += c;
        SetFamous(v);
        //Debug.Log("IncFamous " + c + " => " + Famous);
    }
    // level defines gig place

    /// <summary>
    /// MissionCounter. Total number of finished missions.
    /// </summary>
    public int MissionCounter = 0;

    public int _level;
    public int Level
    {
        get { return (256471 ^ _level) - 2839; }
        //get { return CurrentChapter + CurrentPhase; }
    }
    public string LevelString
    {
        get { return (Level + 1).ToString(); }
    }
    public void SetLevel(int c)
    {
        _level = (c + 2839) ^ 256471;
        _changed = true;
    }
    public void IncLevel()
    {
        int l = Level;
        if (l < GameConsts.maxLevel)
        {
            l++;
            SetLevel(l);
            Score += GameConsts.Score.LevelChange * l;
            if (OnLevelChanged != null)
            {
                OnLevelChanged();
            }
        }
    }

    public int _sublevel;
    public int SubLevel
    {
        get { return (156471 ^ _sublevel) - 2835; }
    }
    public void SetSubLevel(int c)
    {
        _sublevel = (c + 2835) ^ 156471;
        _changed = true;
    }
    public void IncSubLevel()
    {
        int l = SubLevel;
        l++;
        SetSubLevel(l);
    }


    public long _gigCoins;
    public long GigCoins
    {
        get { return (43673 ^ _gigCoins) - 9113; }
    }
    public void SetGigCoins(long c)
    {
        _gigCoins = (c + 9113) ^ 43673;
        _changed = true;
    }
    public void IncGigCoins(long c)
    {
        long gcoins = GigCoins;
        gcoins += c;
        SetGigCoins(gcoins);
    }

    public int _diamondsPurchased;
    public int _diamondsSpend;
    public int DiamondsPurchased
    {
        get {  return (26572 ^ _diamondsPurchased) - 1713; }
        set
        {
            int newvalue = (value + 1713) ^ 26572;
            if (newvalue  != _diamondsPurchased)
            {
                _diamondsPurchased = newvalue;
                if (OnDiamondsChanged != null)
                    OnDiamondsChanged();
                _changed = true;

            }
        }
    }
    public int DiamondsSpend
    {
		get { return (36571 ^ _diamondsSpend) - 1915; }
        set
        {
            int newvalue = (value + 1915) ^ 36571;
            if (newvalue != _diamondsSpend)
            {
                _diamondsSpend = newvalue;
                if (OnDiamondsChanged != null)
                    OnDiamondsChanged();
                _changed = true;
            }
        }
    }

    public int _diamonds;
    public int Diamonds
    {
//#if UNITY_IOS
        get { return GameConsts.StartupDiamonds + DiamondsPurchased - DiamondsSpend; }
//#else
//        get { return (726571 ^ _diamonds) - 9815; }
//#endif
    }
    public void SetDiamonds(int c)
    {
        _diamonds = (c + 9815) ^ 726571;
        if (OnDiamondsChanged != null)
            OnDiamondsChanged();
        _changed = true;

    }
    public void ChangeDiamonds(int count)
    {
//#if UNITY_IOS
        if (count < 0)
        {
			DiamondsSpend += Mathf.Abs( count );
            _changed = true;
        }
        else
        {
            DiamondsPurchased += count;
            _changed = true;
        }
//#else
//        int dc = Diamonds;
//        dc += count;
//        SetDiamonds(dc);
//        _changed = true;
//#endif
    }
    public void InitDiamonds()
    {
        SetDiamonds(GameConsts.StartupDiamonds);
        DiamondsPurchased = 0;
        DiamondsSpend = 0;
    }


    public void ResetPowerUps()
    {
        if (PowerUpList == null)
            PowerUpList = new List<int>();
        else
        {
            PowerUpList.Clear();
        }

    }

    public void ClearPowerUp(int id)
    {
        if (!PowerUpList.Contains(id))
        {
            PowerUpList.Remove(id);
            if (OnBonusesChanged != null)
            {
                OnBonusesChanged();
            }

        }

    }
    public void AddPowerUp(int id)
    {
        if (!PowerUpList.Contains(id))
        {
            PowerUpList.Add(id);
        }
        CalculateUpdates();
        if (OnBonusesChanged != null)
        {
            OnBonusesChanged();
        }
        _changed = true;
    }

    public bool PowerUpOwned(int id)
    {
        return PowerUpList.Contains(id);
    }

    public void SetUpgrade(int id, int count)
    {
        bool found = false;
        for (int i = 0; i < UpgradeCount.Count; i++)
        {
            if (UpgradeCount[i].id == id)
            {
                UpgradeCount[i].count = count;
                found = true;
                break;
            }
        }
        if (!found)
        {
            UpgradeCount.Add(new ItemCount(id, count));
        }
    }

    public void AddUpgrade(int id)
    {
        bool found = false;
        for (int i = 0; i < UpgradeCount.Count; i++)
        {
            if (UpgradeCount[i].id == id)
            {
                UpgradeCount[i].count++;
                found = true;
                break;
            }
        }
        if (!found)
        {
            UpgradeCount.Add(new ItemCount(id, 1));
        }

        if (OnBonusesChanged != null)
        {
            OnBonusesChanged();
        }
        _changed = true;

    }

    public void ClearUpgradeCount(int id)
    {
        for (int i = 0; i < UpgradeCount.Count; i++)
        {
            if (UpgradeCount[i].id == id)
            {
                UpgradeCount[i].count = 0;
                if (OnBonusesChanged != null)
                {
                    OnBonusesChanged();
                }
                break;
            }
        }

    }

    public int GetUpgradeCount(int id)
    {
        for (int i = 0; i < UpgradeCount.Count; i++)
        {
            if (UpgradeCount[i].id == id)
            {
                return UpgradeCount[i].count;
            }
        }
        return 0;
    }

    public void SetTapBonus(int id, int count)
    {
        bool found = false;
        for (int i = 0; i < TapBonusCount.Count; i++)
        {
            if (TapBonusCount[i].id == id)
            {
                TapBonusCount[i].count = count;
                found = true;
                break;
            }
        }
        if (!found)
        {
            TapBonusCount.Add(new ItemCount(id, count));
        }
    }
    public void AddTapBonus(int id, int count)
    {
        _tapValueNeedsUpdate = true;
        bool found = false;
        for (int i = 0; i < TapBonusCount.Count; i++)
        {
            if (TapBonusCount[i].id == id)
            {
                TapBonusCount[i].count = count;
                MissionManager.Instance.IncreaseTapUpgrade();
                found = true;
                break;
            }
        }
        if (!found)
        {
            TapBonusCount.Add(new ItemCount(id, count));
        }

    }

    public void AddTapBonus(int id)
    {
        bool found = false;
        for (int i = 0; i < TapBonusCount.Count; i++)
        {
            if (TapBonusCount[i].id == id)
            {
                TapBonusCount[i].count++;
                Score += TapBonusCount[i].count * GameConsts.Score.TapUpgrade;
                MissionManager.Instance.IncreaseTapUpgrade();
                found = true;
                break;
            }
        }
        if (!found)
        {
            TapBonusCount.Add(new ItemCount(id, 1));
            MissionManager.Instance.IncreaseTapUpgrade();
        }
        _tapValueNeedsUpdate = true;
        _changed = true;
        if (OnTapUpgrade != null)
            OnTapUpgrade();

        if (OnBonusesChanged != null)
        {
            OnBonusesChanged();
        }

    }

    public void CallOnTapUpgrade()
    {
        if (OnTapUpgrade != null)
            OnTapUpgrade();
    }

    public int GetTapBonusCount(int id)
    {
        for (int i = 0; i < TapBonusCount.Count; i++)
        {
            if (TapBonusCount[i].id == id)
            {
                return TapBonusCount[i].count;
            }
        }
        return 0;
    }
    public void ResetTapBonusCount(int id)
    {
        for (int i = 0; i < TapBonusCount.Count; i++)
        {
            if (TapBonusCount[i].id == id)
            {
                TapBonusCount[i].count = 0;
                if (OnBonusesChanged != null)
                {
                    OnBonusesChanged();
                }
                break;
            }
        }
    }

    public void ResetIdleBonusCount(int id)
    {
        for (int i = 0; i < IdleBonusCount.Count; i++)
        {
            if (IdleBonusCount[i].id == id)
            {
                IdleBonusCount[i].count = 0;
                if (OnBonusesChanged != null)
                {
                    OnBonusesChanged();
                }
                break;
            }
        }
    }

    public void SetIdleBonus(int id, int count)
    {
        bool found = false;
        for (int i = 0; i < IdleBonusCount.Count; i++)
        {
            if (IdleBonusCount[i].id == id)
            {
                IdleBonusCount[i].count = count;
                found = true;
                break;
            }
        }
        if (!found)
        {
            IdleBonusCount.Add(new ItemCount(id, count));
        }

    }
    public void AddIdleBonus(int id, int count)
    {
        _IdlevalueNeedsUpdate = true;
        bool found = false;
        for (int i = 0; i < IdleBonusCount.Count; i++)
        {
            if (IdleBonusCount[i].id == id)
            {
                IdleBonusCount[i].count = count;
                MissionManager.Instance.IncreaseIdleUpgrade();
                found = true;
                break;
            }
        }
        if (!found)
        {
            IdleBonusCount.Add(new ItemCount(id, count));
        }

    }

    public void CallOnIdleUpgrade()
    {
        if (OnIdleUpgrade != null)
            OnIdleUpgrade();
    }

    public void AddIdleBonus(int id)
    {
        bool found = false;
        for (int i = 0; i < IdleBonusCount.Count; i++)
        {
            if (IdleBonusCount[i].id == id)
            {
                IdleBonusCount[i].count++;
                Score += GameConsts.Score.IdleUpgrade + IdleBonusCount[i].count;
                MissionManager.Instance.IncreaseIdleUpgrade();
                found = true;
                break;
            }
        }
        if (!found)
        {
            IdleBonusCount.Add(new ItemCount(id, 1));
            MissionManager.Instance.IncreaseIdleUpgrade();
        }
        _IdlevalueNeedsUpdate = true;
        _changed = true;
        if (OnIdleUpgrade != null)
            OnIdleUpgrade();

        if (OnBonusesChanged != null)
        {
            OnBonusesChanged();
        }

    }
    public int GetIdleBonusCount(int id)
    {
        for (int i = 0; i < IdleBonusCount.Count; i++)
        {
            if (IdleBonusCount[i].id == id)
            {
                return IdleBonusCount[i].count;
            }
        }
        return 0;
    }

    public void CalculateUpdates()
    {
        _tapValueNeedsUpdate = true;
        _IdlevalueNeedsUpdate = true;
    }

    private int GetTapPwrUpBonus(int id)
    {
        return 1;
    }

    private double _calculatedTapValue;
    public bool _tapValueNeedsUpdate = true;
    private double _tapMultipler = 1;
    public void SetTapMultipler(double v)
    {
        _tapMultipler = v;
        _tapValueNeedsUpdate = true;
        if (OnBonusesChanged != null)
        {
            OnBonusesChanged();
        }
    }
    /// <summary>
    /// Returns Current Tap value without bonuses.
    /// </summary>
    public double GetCurrentTapValue()
    {
        /*
        float retVal;
        int maxId = -1;
        int maxIndex = -1;
        for (int i = 0; i < TapBonusCount.Count; i++)
        {
			if (TapBonusCount[i].id > maxId && TapBonusCount[i].count > 0)
            {
                maxId = TapBonusCount[i].id;
                maxIndex = i;
            }
        }
        float bonus = BonusManager.Instance.m_TapBonusBaseStart;
        if (maxId >= 0 && maxIndex >= 0)
        {
            bonus = BonusManager.Instance.GetBonusMultiply(BonusTypes.Type.Tap, TapBonusCount[maxIndex].id, TapBonusCount[maxIndex].count);
        }
        retVal = bonus;
        return retVal;
        */
        return CalculateTapValue();
    }

    private double CalculateTapValue()
    {
        double tapval = (double)BonusManager.Instance.m_TapBonusBaseStart;
        if (PrestigeLevel > Level)
        {
            tapval *= 1.3d;
        }

        for (int i = 0; i < TapBonusCount.Count; i++)
        {
            if (TapBonusCount[i].count > 0)
            {
                tapval += BonusManager.Instance.GetTapBonus(TapBonusCount[i].id, TapBonusCount[i].count);
            }
        }
#if SOFTCEN_DEBUG
        Debug.Log("<color=lightblue>CalculateTapValue: " + tapval + "</color>");
#endif
        return tapval;
    }

    public double GetTapValue()
    {
        double retVal = 1;
        if (_tapValueNeedsUpdate)
        {
            /*int maxId = -1;
            int maxIndex = -1;
            for (int i = 0; i < TapBonusCount.Count; i++)
            {
				if (TapBonusCount[i].count > 0 && TapBonusCount[i].id > maxId)
                {
                    maxId = TapBonusCount[i].id;
                    maxIndex = i;
                }                    
            }
            float bonus = BonusManager.Instance.m_TapBonusBaseStart;
            if (maxId >= 0 && maxIndex >= 0)
            {
                bonus = BonusManager.Instance.GetBonusMultiply(BonusTypes.Type.Tap, TapBonusCount[maxIndex].id, TapBonusCount[maxIndex].count);
            }
            _calculatedTapValue = bonus * _tapMultipler;
            retVal = _calculatedTapValue;
            _tapValueNeedsUpdate = false;*/
            double bonus = CalculateTapValue();
            _calculatedTapValue = bonus * _tapMultipler;
            retVal = _calculatedTapValue;
            _tapValueNeedsUpdate = false;

        }
        else
        {
            retVal = _calculatedTapValue;
        }

#if SOFTCEN_DEBUG
        /*Debug.Log("GetTapValue " + retVal.ToString()
            + ", PrestigeLevel: " + PrestigeLevel
            + ", Level: " + Level
            );*/
#endif
        //if (PrestigeLevel > Level)
        //    retVal = retVal * 1.3f;

        return retVal;
    }


    private double _calculatedIdleValue;
    public bool _IdlevalueNeedsUpdate = true;
    private double _idleMultipler = 1;
    public void SetIdleMultipler(double v)
    {
        _idleMultipler = v;
        _IdlevalueNeedsUpdate = true;
        if (OnBonusesChanged != null)
        {
            OnBonusesChanged();
        }
    }

    public double GetIdleValue()
    {
        double retVal = 0;
        if (_IdlevalueNeedsUpdate)
        {
            for (int i = 0; i < IdleBonusCount.Count; i++)
            {
                if (IdleBonusCount[i].count > 0)
                {
                    retVal += BonusManager.Instance.GetBonusMultiply(BonusTypes.Type.Idle, IdleBonusCount[i].id, IdleBonusCount[i].count);
                }
            }

            _calculatedIdleValue = retVal * _idleMultipler;
            //if (PrestigeLevel > Level)
            //    _calculatedIdleValue = _calculatedIdleValue * 1.3f;
            retVal = _calculatedIdleValue;
            _IdlevalueNeedsUpdate = false;
        }
        else
        {
            retVal = _calculatedIdleValue;
        }
        //Debug.Log("GetIdleValue " + retVal);

        return retVal;
    }

    public double _moneyearned;
    public double MoneyEarned
    {
        get { return _moneyearned - 2193.35f; }
        set { _moneyearned = value + 2193.35f; }
    }

    public double _moneyused;
    public double MoneyUsed
    {
        get { return _moneyused - 1938.57f; }
        set { _moneyused = value + 1938.57f;  }
    }

    //public float _money;
    public double Money
    {
        get { return MoneyEarned - MoneyUsed; }
    }

    public void InitMoney()
    {
        MoneyEarned = GameConsts.StartupMoney;
        MoneyUsed = 0f;
        if (OnMoneyChanged != null)
        {
            OnMoneyChanged(Money);
        }
        _changed = true;
    }

    public void IncMoney(double count)
    {
        MoneyEarned += count;
        if (OnMoneyChanged != null)
            OnMoneyChanged(Money);
        _changed = true;
    }

    public void DecMoney(double count)
    {
        MoneyUsed += count;
        if (OnMoneyChanged != null)
            OnMoneyChanged(Money);
        _changed = true;
    }

    public int _adfree;
    /*public int mAdFree {
		get { return _adfree; }
		set { 
                _adfree = value; 
            }
	}*/
    public bool AdFree
    {
        get
        {
            if (_adfree == 13689)
                return true;
            else
                return false;
        }
        set
        {
            if (value == true)
            {
                if (_adfree != 13689)
                {
                    _adfree = 13689;
                    _changed = true;
                    if (OnStoreItemChanged != null)
                        OnStoreItemChanged();
                }
            }
            else
            {
                if (_adfree != 24679)
                {
                    _changed = true;
                    _adfree = 24679;
                }
            }
        }
    }

    public void SetSaved()
    {
        _changed = false;
    }
    public void SetDirty()
    {
        _changed = true;
    }

    public void Prestige()
    {
        _tapValueNeedsUpdate = true;
        InitMoney();
        IdleBonusCount.Clear();
        TapBonusCount.Clear();
        UpgradeCount.Clear();
        ResetPowerUps();
        MissionCounter = 0;
        CurrentChapter = 0;
        CurrentPhase = 0;
        SetGigCoins(0);
        SetLevel(0);
        SetFamous(0);
        SetSubLevel(0);
        CalculateUpdates();
        CurrentMission = new MissionItem();
        _changed = true;
    }
}

