using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
#if SOFTCEN_DEBUG
using System;
using System.IO;
#endif

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance = null;

    double[] IdleBase = new double[]
    {
        // Chapter 1:
        50d,        // Idle_C1_Fisherman
        100d,       // Idle_C1_Maintenance
        // Chapter 2:
        200d,       // Idle_C2_Fisherman
        400d,       // Idle_C2_Maintenance
        820d,       // Idle_C2_Smokehouse
        3000d,      // Idle_C2_Lighthouse
        // Chapter 3:
        6000d,      // Idle_C3_PierWorker1
        11000d,     // Idle_C3_HotelWorker1
        15000d,     // Idle_C3_HotelWorker2
        20000d,     // Idle_C3_HotelWorker3
        // Chapter 4:
        32000d,     // Idle_C4_Warehouse1
        60000d,     // Idle_C4_Warehouse2
        150000d,    // Idle_C4_DockWorker
        250000d,    // Idle_C4_Lighthouse
        // Chapter 5:
        410000d,    // Idle_C5_BayWorker1,
        700000d,    // Idle_C5_DockWorker1,
       1000000d,    // Idle_C5_BayWorker2,
       2000000d,    // Idle_C5_DockWorker2,
       // Chapter 6:
       3500000d,    // Idle_C6_Factory1 = 1018,
       7000000d,    // Idle_C6_Factory2 = 1019,
      14000000d,    // Idle_C6_Factory3 = 1020,
      28000000d,    // Idle_C6_Factory4 = 1021,
       // Chapter 7:
      39000000d,    // Idle_C7_1
      58000000d,    // Idle_C7_2,
      88000000d,    // Idle_C7_3,
     132000000d,    // Idle_C7_4
     // Chapter 8:
     152000000d,    // Idle_C8_1
     175000000d,    // Idle_C8_2
     202000000d,    // Idle_C8_3
     232000000d,    // Idle_C8_4

    };

    // Tap base prices:
    double[] TapBase = new double[]
    {
        // Chapter 1:
        100d,       // Tap_C1_SmallFishingBoat = 2000,
        200d,       // Tap_C1_MediumFishingBoat = 2001,
        // Chapter 2:
        400d,       // Tap_C2_MediumFishingBoat1 = 2002,
        800d,       // Tap_C2_MediumFishingBoat2 = 2003,
        1000d,      // Tap_C2_MediumFishingBoat3 = 2004,
        1200d,      // Tap_C2_MediumFishingBoat4 = 2005,
        // Chapter 3:
        8000d,      // Tap_C3_Sailboat1 = 2006,
        11000d,     // Tap_C3_Sailboat2 = 2007,
        17000d,     // Tap_C3_SuperYacht1 = 2009,
        26000d,     // Tap_C3_SuperYacht2 = 2010,
        // Chapter 4:
        38000d,     // Tap_C4_Tugboat1
        66000d,     // Tap_C4_Barge1
       170000d,     // Tap_C4_Tugboat2
       310000d,     // Tap_C4_Barge2
        // Chapter 5:
       450000d,     // Tap_C5_Containers1,
       800000d,     // Tap_C5_ContainerShip1,
      1200000d,     // Tap_C5_Containers2,
      2400000d,     // Tap_C5_ConatinerShip2,
        // Chapter 6:
      4200000d,     // Tap_C6_Lumbership1 = 2018,
      8400000d,     // Tap_C6_Lumbership2 = 2019,
     17000000d,     // Tap_C6_Lumbership3 = 2020,
     33000000d,     // Tap_C6_Lumbership4 = 2021,
      // Chapter 7:
     50000000d,    // Tap_C7_Tankership1 = 2022,
     75000000d,    // Tap_C7_Tankership2 = 2023,
    112000000d,    // Tap_C7_Tankership3 = 2024,
    168000000d,    // Tap_C7_Tankership4 = 2025,
    // Chapter 8:
    193000000d,    // Tap_C8_1
    222000000d,    // Tap_C8_2
    255000000d,    // Tap_C8_3
    293000000d,    // Tap_C8_4

    };

    double[] LevelBase = new double[]
    {
        // Chapter 1:
        500d,       // Lvl_Chapter1_Wharfs = 0,
        600d,       // Lvl_Chapter1_Boatsheds = 1,
        800d,       // Lvl_Chapter1_Lighthouse = 2,
        // Chapter 2:
        1200d,      // Lvl_Chapter2_Location = 3,
        2000d,      // Lvl_Chapter2_Moorings = 4,
        3000d,      // Lvl_Chapter2_Boatsheds = 5,
        4000d,      // Lvl_Chapter2_Smokehouses = 6,
        5000d,      // Lvl_Chapter2_Lighthouse = 7,
        // Chapter 3:
        20000d,     // Lvl_Chapter3_Location = 8,
        22000d,     // Lvl_C3_Hotel1 = 9,
        26000d,     // Lvl_C3_Hotel2 = 10,
        30000d,     // Lvl_C3_Hotel3 = 11,
        36000d,     // Lvl_C3_Lighthouse = 12,
        // Chapter 4:
        250000d,    // Lvl_C4_Location = 13,
        300000d,    // Lvl_C4_Warehouse1 = 14,
        350000d,    // Lvl_C4_Warehouse2 = 15,
        400000d,    // Lvl_C4_MediumCrane = 16,
        450000d,    // Lvl_C4_Lighthouse = 17,
        // Chapter 5:
        500000d,    // Lvl_C5_Location = 18,
        700000d,    // Lvl_C5_LoadingBay1 = 19,
       1100000d,    // Lvl_C5_CraneMedium1 = 20,
       1800000d,    // Lvl_C5_LoadingBay2 = 21,
       3000000d,    // Lvl_C5_CraneMedium2 = 22,
       4000000d,    // Lvl_C5_Lighthouse
        // Chapter 6:
       4800000d,    // Lvl_C6_Location
       9600000d,    // Lvl_C6_Factory1 = 25,
      20000000d,    // Lvl_C6_Factory2 = 26,
      40000000d,    // Lvl_C6_Factory3 = 27,
      80000000d,    // Lvl_C6_Factory4 = 28,
      90000000d,    // Lvl_C6_Lighthouse = 29,
        // Chapter 7:
      9200000d,    // Lvl_C7_Location = 30,
     138000000d,    // Lvl_C7_FuelSilos1 = 31,
     207000000d,    // Lvl_C7_FuelSilos2 = 32,
     310000000d,    // Lvl_C7_FuelSilos3 = 33,
     465000000d,    // Lvl_C7_FuelSilos4 = 34,
     700000000d,    // Lvl_C7_Lighthouse = 35,
        // Chapter 8:
     800000000d,    // Lvl_C8_Location,
     920000000d,    // Lvl_C8_1,
    1060000000d,    // Lvl_C8_2,
    1220000000d,    // Lvl_C8_3,
    1400000000d,    // Lvl_C8_4,
    1620000000d,    // Lvl_C8_5,


    };

    // Tap value base:
    double[] TapValueBase = new double[]
    {
        // Chapter 1:
        1d,             // Tap_C1_SmallFishingBoat = 2000,
        1.05d,          // Tap_C1_MediumFishingBoat = 2001,
        // Chapter 2:
        1.45d,          // Tap_C2_MediumFishingBoat1 = 2002,
        2.2d,           // Tap_C2_MediumFishingBoat2 = 2003,
        3.4d,           // Tap_C2_MediumFishingBoat3 = 2004,
        4d,             // Tap_C2_MediumFishingBoat4 = 2005,
        // Chapter 3:
        6.2d,           // Tap_C3_Sailboat1 = 2006,
        11d,            // Tap_C3_Sailboat2 = 2007,
        14.7d,          // Tap_C3_SuperYacht1 = 2009,
        24d,            // Tap_C3_SuperYacht2 = 2010,
        // Chapter 4:
        34d,            // Tap_C4_Tugboat1
        53d,            // Tap_C4_Barge1
        78d,            // Tap_C4_Tugboat2
        118d,           // Tap_C4_Barge2
        // Chapter 5:
        175d,           // Tap_C5_Containers1,
        270d,           // Tap_C5_ContainerShip1,
        400d,           // Tap_C5_Containers2,
        600d,           // Tap_C5_ConatinerShip2,
        // Chapter 6:
        1000d,          // Tap_C6_Lumbership1 = 2018,
        1310d,          // Tap_C6_Lumbership2 = 2019,
        1850d,          // Tap_C6_Lumbership3 = 2020,
        3100d,          // Tap_C6_Lumbership4 = 2021,
      // Chapter 7:
        4500d,          // Tap_C7_Tankership1 = 2022,
        7000d,          // Tap_C7_Tankership2 = 2023,
        10000d,         // Tap_C7_Tankership3 = 2024,
        15000d,         // Tap_C7_Tankership4 = 2025,
        // Chapter 8:
        18000d,         // Tap_C8_1
        21600d,         // Tap_C8_2
        25900d,         // Tap_C8_3
        31100d,         // Tap_C8_4
    };


    public Chapters chapters;
    public Transform trLevel;
    public List<UpgradeItemLevel> levelItemList;
    public Transform trIdleItems;
    public List<UpgradeItemIdle> idleItemList;
    public Transform trTapItems;
    public List<UpgradeItemTap> tapItemList;

    public int maxLevel = 38;

    public float increaseValue = 0.1f;
    //private long neededMoney = -1;

    public float m_IdleCoin_IncreasePercentage = 1.15f;
    public float m_TapCoin_IncreasePercentage = 1.15f;

    public int m_Diamond_Start = 2;
    public float m_DiamondIncrease = 1.15f;

    public float m_MultiplyIncrease = 2f;

    public int m_LevelDiamondBaseStart = 10;
    public float m_LevelDiamondIncrease = 1.15f;

    //private float[] m_BonusMultiply;


    private GameManager _gm;
    private GameManager m_gm
    {
        get
        {
            if (_gm == null)
                _gm = GameManager.Instance;
            return _gm;
        }
    }

    public Dictionary<int, int> IdleStartLevels;
    public Dictionary<int, int> TapStartLevels;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        /*
        float multiply = 2;
        m_BonusMultiply = new float[maxLevel];
        for (int i = 0; i < maxLevel; i++)
        {
            m_BonusMultiply[i] = multiply;
            multiply = multiply * m_MultiplyIncrease;
        }
#if SOFTCEN_DEBUG
        string str = "BonusMultiply :\n";
        for (int i=0; i < maxLevel; i++)
        {
            str += i.ToString() + ": " + m_BonusMultiply[i].ToString() + "\n";
        }
        Debug.Log(str);
#endif
*/
    }

    void Start()
    {
        IdleStartLevels = new Dictionary<int, int>();

        TapStartLevels = new Dictionary<int, int>();

        GetLevelItems();
        GetIdleItems();
        GetTapItems();
    }

#if SOFTCEN_DEBUG && UNITY_EDITOR
    private void OnEnable()
    {
        GameManager.OnGameLoaded += GameManager_OnGameLoaded;
    }

    private void OnDisable()
    {
        GameManager.OnGameLoaded -= GameManager_OnGameLoaded;
    }

    private void GameManager_OnGameLoaded()
    {
        AnalyseBonuses();

        string str = "IdleStartLevels\n";
        foreach (KeyValuePair<int, int> kv in IdleStartLevels)
        {
            str += kv.Key.ToString() + " : " + kv.Value.ToString() + "\n";
        }
        Debug.Log(str);
        str = "TapStartLevels\n";
        foreach (KeyValuePair<int, int> kv in TapStartLevels)
        {
            str += kv.Key.ToString() + " : " + kv.Value.ToString() + "\n";
        }
        Debug.Log(str);
    }
#endif

    public int GetCurrentChapterStartLevel()
    {
        int c = GameManager.Instance.playerData.CurrentChapter - 1;
        if (c >= 0 && c < chapters.chapterInfo.Length)
        {
            return chapters.chapterInfo[c].startLevel;
        }
        return 1;
    }

    public string GetCurrentChapterName()
    {
        int c = GameManager.Instance.playerData.CurrentChapter-1;
        if (c >= 0 && c < chapters.chapterInfo.Length)
        {
            return chapters.chapterInfo[c].ChapterName;
        }
        return "";
    }
    public string GetChapterName(int id)
    {
        return chapters.GetChapterName(id);
    }
    public ChapterInfo GetChapterInfo(int id)
    {
        return chapters.GetChapterInfo(id);
    }

    public int GetChapterStartLevel(int chapter)
    {
        return chapters.GetChapterStartLevel(chapter);
    }

    public Sprite GetChapterSprite(int id)
    {
        return chapters.GetChapterSprite(id);
    }

    public bool IsChapterCompleted(int chapterId)
    {
        bool retVal = true;
        for (int i = 0; i < levelItemList.Count; i++)
        {
            if ((int)levelItemList[i].chapterId == chapterId)
            {
                if (!levelItemList[i].isItemActive())
                {
                    retVal = false;
#if SOFTCEN_DEBUG
                    /*Debug.Log("<color=green>BonusManager Not completed id: " + chapterId 
                        + ", item: " + levelItemList[i].Id.ToString()
                        //+ ", owned: " + levelItemList[i].ownedCount.ToString()
                        //+ ", maxcount: " + levelItemList[i].maxCount.ToString()
                        + " Not active</color>");*/
#endif
                }
            }
        }
#if SOFTCEN_DEBUG
        Debug.Log("BonusManager IsChapterCompleted id: " + chapterId + ", value: " + retVal.ToString());
#endif
        return retVal;
    }

    private UpgradeItemLevel FindLevelItem(int id)
    {
        for (int i = 0; i < levelItemList.Count; i++)
        {
            if ((int)levelItemList[i].Id == id)
            {
                return levelItemList[i];
            }

        }
        return null;
    }

    private void GetLevelItems()
    {
        if (levelItemList == null)
        {
            levelItemList = new List<UpgradeItemLevel>();
        }
        else
        {
            levelItemList.Clear();
        }

        for (int i=0; i < trLevel.childCount; i++)
        {
            Transform trChild = trLevel.GetChild(i);
            for (int j=0; j < trChild.childCount; j++)
            {
                UpgradeItemLevel uil = trChild.GetChild(j).GetComponent<UpgradeItemLevel>();
                if (uil != null)
                {
                    levelItemList.Add(uil);
                }
            }
        }

#if SOFTCEN_DEBUG
		/*
        int maxDiamonds = 0;
        string str = "Level Count: " + levelItemList.Count + ", Prices:\n";
        string name = "";
        for (int i = 0; i < Item.MaxLevel; i++)
        {
            UpgradeItemLevel uil = FindLevelItem(i);
            if (uil != null)
            {
                name = uil.Id.ToString();
            }
            else
            {
                name = "New Location";
            }
            str += "Level: " + name + " : " + NumToStr.GetNumStr(GetLevelCoinPrice(i));
            str += ", DiamondBase: " + GetLevelDiamondPrice(i).ToString();
            maxDiamonds += GetLevelDiamondPrice(i);
            str += "\n";
        }
        str += "Maximum diamonds: " + maxDiamonds.ToString() + "\n";
        Debug.Log(str);*/
#endif
    }

    public double GetLevelCoinPrice(int level)
    {
        if (level >= 0 && level < LevelBase.Length)
            return LevelBase[level];
#if SOFTCEN_DEBUG
        Debug.Log("<color=red>Fatal error: MAX GetLevelCoinPrice level:" + level.ToString() + "</color>");
#endif
        return LevelBase[LevelBase.Length - 1];
    }

    private void GetTapItems()
    {
        if (tapItemList == null)
        {
            tapItemList = new List<UpgradeItemTap>();
        }
        else
        {
            tapItemList.Clear();
        }
        TapStartLevels.Clear();

        for (int i = 0; i < trTapItems.childCount; i++)
        {
            Transform trChild = trTapItems.GetChild(i);
            for (int j = 0; j < trChild.childCount; j++)
            {
                UpgradeItemTap uit = trChild.GetChild(j).GetComponent<UpgradeItemTap>();
                if (uit != null)
                {
                    tapItemList.Add(uit);
                    TapStartLevels.Add((int)uit.Id, uit.GetAvailableLevel());
                }
            }
        }
#if SOFTCEN_DEBUG
		/*
        string dbgStr = "Tap bonus: \n";
        for (int i=0; i < tapItemList.Count; i++)
        {
            for (int j=0; j < 5; j++)
            {
                dbgStr += "("+i + ","+j+ ") : " + GetBonusMultiply(BonusTypes.Type.Tap, i + Item.TapIdStart, j).ToString() + ", ";
            }
            dbgStr += "\n";
        }
        Debug.Log(dbgStr);*/
#endif

        GetTapPrices();
    }

    private void GetTapPrices()
    {
        // tap items prices
        for (int i = 0; i < tapItemList.Count; i++)
        {
            UpgradeItemTap uit = FindTapItem(Item.TapIdStart + i);
            if (uit != null)
            {
                uit.basePrice = TapBase[i];
                uit.baseDiamonds = GetItemDiamondPrice(m_Diamond_Start, i);
            }
#if SOFTCEN_DEBUG
            else
            {
                Debug.Log("<color=red>Fatal error: uit is NULL i:" + i.ToString() + "</color>");
            }
#endif
        }
#if SOFTCEN_DEBUG
        /*
        int maxDiamonds = 0;
        string str = "Tap Items Count: " + tapItemList.Count + ", Prices:\n";
        for (int i = 0; i < tapItemList.Count; i++)
        {
            UpgradeItemTap uit = FindTapItem(Item.TapIdStart + i);
            str += "id: " + uit.Id.ToString() + " -> BasePrice: " + NumToStr.GetNumStr(uit.basePrice) + ", Upgrades: ";
            for (int j = 0; j < uit.maxCount + 1; j++)
            {
                double nPrice = GetTapItemCoinPrice(uit.basePrice, j);
                str += " (" + j.ToString() + ", " + NumToStr.GetNumStr(nPrice) + ")";
            }
            str += ", Diamonds: " + uit.baseDiamonds.ToString();
            maxDiamonds += uit.baseDiamonds * uit.maxCount;
            str += "\n";
        }
        str += "Maximun diamonds count: " + maxDiamonds.ToString() + "\n";

        Debug.Log(str);*/
#endif

    }

    private UpgradeItemIdle FindIdleItem(int id)
    {
        for (int i = 0; i < idleItemList.Count; i++)
        {
            if ((int)idleItemList[i].Id == id)
            {
                return idleItemList[i];
            }

        }
        return null;
    }
    private UpgradeItemTap FindTapItem(int id)
    {
        for (int i = 0; i < tapItemList.Count; i++)
        {
            if ((int)tapItemList[i].Id == id)
            {
                return tapItemList[i];
            }

        }
        return null;
    }

    private void GetIdleItems()
    {
        if (idleItemList == null)
        {
            idleItemList = new List<UpgradeItemIdle>();
        }
        else
        {
            idleItemList.Clear();
        }
        IdleStartLevels.Clear();

        for (int i = 0; i < trIdleItems.childCount; i++)
        {
            Transform trChild = trIdleItems.GetChild(i);
            for (int j = 0; j < trChild.childCount; j++)
            {
                UpgradeItemIdle uit = trChild.GetChild(j).GetComponent<UpgradeItemIdle>();
                if (uit != null)
                {
                    idleItemList.Add(uit);
                    IdleStartLevels.Add((int)uit.Id, uit.GetAvailableLevel());
                }
            }
        }
        // Idle items prices
        for (int i=0; i < idleItemList.Count; i++)
        {
            UpgradeItemIdle uit = FindIdleItem(Item.IdleIdStart + i);
            if (uit != null)
            {
                uit.basePrice = IdleBase[i];
                uit.baseDiamonds = GetItemDiamondPrice(m_Diamond_Start, i);
            }
#if SOFTCEN_DEBUG
            else
            {
                Debug.Log("<color=red>Fatal error: uit is NULL i:"+ i.ToString()  +"</color>");
            }
#endif
        }
#if SOFTCEN_DEBUG
        /*int maxDiamonds = 0;
        string str = "Idle Items Count: " + idleItemList.Count + ", Prices:\n";
        for (int i = 0; i < idleItemList.Count; i++)
        {
            UpgradeItemIdle uit = FindIdleItem(Item.IdleIdStart + i);
            str += "id: " + uit.Id.ToString() + " -> BasePrice: " + NumToStr.GetNumStr(uit.basePrice) + ", Upgrades: ";
            for (int j=0; j < uit.maxCount+1; j++)
            {
                double nPrice = GetIdleItemCoinPrice(uit.basePrice, j);
                str += " (" + j.ToString() + ", " + NumToStr.GetNumStr(nPrice) + ")";
            }
            maxDiamonds += uit.baseDiamonds * uit.maxCount;
            str += ", Diamonds: " + uit.baseDiamonds.ToString();
            str += "\n";
        }
        str += "Maximun diamonds count: " + maxDiamonds.ToString() + "\n";
        Debug.Log(str);*/
#endif
    }

    public float m_IdleBonusBaseStart = 1;
    public float m_IdleBonusIncrease = 1.1f;
    public float m_TapBonusBaseStart = 1;
    public float m_TapBonusBaseInc = 2f;
    public float m_TapBonusIncrease = 1.1f;

    // Returns single id tap value increase;
    public double GetTapBonus(int id, int count)
    {
        double retVal = 0;
        int adjustedId = id - Item.TapIdStart;
        double baseval = TapValueBase[Mathf.Min(adjustedId, TapValueBase.Length-1)];
        double totalval = baseval;
        for (int i=0; i < count; i++)
        {
            totalval = totalval * m_TapBonusIncrease;
        }
        retVal = totalval - baseval;

        return TapPrestige(retVal, id);
    }

    private double IdlePrestige(double baseValue, int id)
    {
        if (IdleStartLevels.ContainsKey(id))
        {
            if (m_gm.playerData.PrestigeLevel > IdleStartLevels[id])
            {
#if SOFTCEN_DEBUG
                //Debug.Log("IdlePrestige retVal: " + (baseValue * 1.3d).ToString());
#endif
                return baseValue * 1.3d;
            }
        }
        return baseValue;
    }
    private double TapPrestige(double baseValue, int id)
    {
        if (TapStartLevels.ContainsKey(id))
        {
            if (m_gm.playerData.PrestigeLevel > TapStartLevels[id])
            {
#if SOFTCEN_DEBUG
                //Debug.Log("TapPrestige Tap retVal: " + (baseValue*1.3d).ToString());
#endif
                return baseValue * 1.3d;
            }
        }
        return baseValue;
    }

    public bool IsTapPrestiged(int id)
    {
        if (TapStartLevels.ContainsKey(id))
        {
            return (m_gm.playerData.PrestigeLevel > TapStartLevels[id]);
        }
        return false;
    }
    public bool IsIdlePrestiged(int id)
    {
        if (IdleStartLevels.ContainsKey(id))
        {
            return (m_gm.playerData.PrestigeLevel > IdleStartLevels[id]);
        }
        return false;
    }

    public double GetBonusMultiply(BonusTypes.Type type, int id, int count)
    {
        if (type == BonusTypes.Type.Idle)
        {
            double retVal = 0;
            int adjustedId = id - Item.IdleIdStart;
            if (adjustedId == 0)
            {
                retVal = IdlePrestige(m_IdleBonusBaseStart * count, id);
                return retVal;
            }
            retVal = IdlePrestige(adjustedId * m_IdleBonusIncrease * count, id);

            return retVal;
        }
        else if (type == BonusTypes.Type.Tap)
        {
            int adjustedId = id - Item.TapIdStart;
            double newbase = m_TapBonusBaseStart * Mathf.Pow(m_TapBonusBaseInc, (float)adjustedId);
            double retval = newbase * Mathf.Pow(m_TapBonusIncrease, (float)count);
#if SOFTCEN_DEBUG
            Debug.Log("<color=yellow>GetBonusMultiply TAP id:" + id + ", count: " + count + ", m_TapBonusBaseStart" + m_TapBonusBaseStart
                + ", newbase: " + newbase + ", retval: " + retval
                + "</color>"
                );
#endif
            retval = TapPrestige(retval, id);
            return retval;
            /*
            int adjustedId = id - Item.TapIdStart;
            if (adjustedId == 0)
                return m_TapBonusBaseStart * count;
            return adjustedId * m_TapBonusIncrease * count;
            */
        }
#if SOFTCEN_DEBUG
        Debug.Log("<color=red>Not implemeted yet</color>");
#endif
        return 0;
    }
    /*
    public float GetBonusMultiply(BonusTypes.Type type, int id)
    {
        int adjustedId = id;
        if (type == BonusTypes.Type.Idle)
        {
            adjustedId = id - Item.IdleIdStart;
            //if (adjustedId == 0)
            //    return 
            
        }
        else if (type == BonusTypes.Type.Tap)
            adjustedId = id - Item.TapIdStart;

        if (adjustedId < m_BonusMultiply.Length)
        {
            return m_BonusMultiply[adjustedId];
        }
        return 1f;
    }*/

    public int GetLevelDiamondPrice(int level)
    {
        return (int)((float)m_LevelDiamondBaseStart * Mathf.Pow(m_LevelDiamondIncrease, (float)level));
    }


    public double GetIdleItemCoinPrice(double basePrice, int count)
    {
        return basePrice * Mathf.Pow(m_IdleCoin_IncreasePercentage, (float)count);
    }

    public double GetTapItemCoinPrice(double basePrice, int count)
    {
        return basePrice * Mathf.Pow(m_TapCoin_IncreasePercentage, (float)count);
    }

    public int GetItemDiamondPrice(int baseDiamonds, int count)
    {        
        return (int)((float)baseDiamonds * Mathf.Pow(m_DiamondIncrease, (float)count));
    }


    public UnityEngine.Object GetItemObject(Item.Identifications id)
    {
        for (int i=0; i < levelItemList.Count; i++)
        {
            if (levelItemList[i].Id == id)
            {
                return levelItemList[i];
            }
        }
        for (int i=0; i < idleItemList.Count; i++)
        {
            if (idleItemList[i].Id == id)
            {
                return idleItemList[i];
            }
        }
        for (int i = 0; i < tapItemList.Count; i++)
        {
            if (tapItemList[i].Id == id)
            {
                return tapItemList[i];
            }
        }
        return null;
    }


    public double GetLowestIdlePrice()
    {
        double retVal = -1f;

        for (int i=0; i < idleItemList.Count; i++ )
        {
            if ((int)idleItemList[i].chapterId == m_gm.playerData.CurrentChapter)
            {
                if (idleItemList[i].ActivePhase <= m_gm.playerData.CurrentPhase)
                {
                    if (idleItemList[i].ownedCount < idleItemList[i].maxCount)
                    {
                        double coinPrice = idleItemList[i].GetCoinPrice();
                        if (retVal < 0 || coinPrice < retVal )
                        {
                            retVal = coinPrice;
                        }
                    }
                }
            }
        }
        return retVal;
    }

    public double GetLowestTapPrice()
    {
        double retVal = -1;

        for (int i = 0; i < tapItemList.Count; i++)
        {
            if ((int)tapItemList[i].chapterId == m_gm.playerData.CurrentChapter)
            {
                if (tapItemList[i].ActivePhase <= m_gm.playerData.CurrentPhase)
                {
                    if (tapItemList[i].ownedCount < tapItemList[i].maxCount)
                    {
                        double coinPrice = tapItemList[i].GetCoinPrice();
                        if (retVal < 0 || coinPrice < retVal)
                        {
                            retVal = coinPrice;
                        }
                    }
                }
            }
        }
        return retVal;
    }

    public double GetLowestLevelPrice()
    {
        double retVal = -1;

        for (int i = 0; i < levelItemList.Count; i++)
        {
            if ((int)levelItemList[i].chapterId == m_gm.playerData.CurrentChapter)
            {
                if ((levelItemList[i].ActivePhase-1) >= m_gm.playerData.CurrentPhase)
                {
                    double coinPrice = levelItemList[i].GetCoinPrice();
                    if (retVal < 0 || coinPrice < retVal)
                    {
                        retVal = coinPrice;
                    }
                }
            }
        }
        return retVal;
    }

#if SOFTCEN_DEBUG
    public float Analyses_TapPerSec = 10f;
    public float Analyses_TimeInMenu = 5f;
    public string fileName = "Analyses.txt";
    private void AnalyseBonuses()
    {
        StreamWriter sw = File.CreateText(fileName);
        int level = 0;
        string dbgStr = "Analyses:\n";
        double totalGameTime = 0f;
        double idleBonus = 0;
        double tapBonus = m_TapBonusBaseStart;
        //float currentBonus = idleBonus + tapBonus;
        double coinsPerSec = tapBonus * Analyses_TapPerSec;
        dbgStr += "Settings: m_IdleCoin_IncreasePercentage: " + m_IdleCoin_IncreasePercentage;
        dbgStr += ", m_TapCoin_IncreasePercentage: " + m_TapCoin_IncreasePercentage;
        dbgStr += "\n";
        sw.Write(dbgStr);
        for (int ch = 0; ch < chapters.chapterInfo.Length; ch++ )
        {
            int chapter = (int)chapters.chapterInfo[ch].Id;
            dbgStr = "";
            double chapterTime = 0;
            //float bonusValue = GetBonusMultiply(BonusTypes.Type.Idle, 0, 1);
            int maxPhase = Analyse_GetMaxChapterPhase(chapter);
            dbgStr += "------------------------------------\nChapter " + chapter.ToString() + ", Phase count: " + maxPhase +"\n";
            for (int phase = 0; phase < maxPhase; phase++)
            {
                dbgStr += "\nPhase: " + phase + ", Level: " + level.ToString() + " ( ";
                double phaseTime = 0f;
                double phaseIdleCoins = 0;
                double phaseTapCoins = 0;
                int maxCount = 0;
                UpgradeItemTap tapItem = GetTapItem(chapter, phase);
                UpgradeItemIdle idleItem = GetIdleItem(chapter, phase);
                if (tapItem != null)
                {
                    maxCount = tapItem.maxCount;
                    dbgStr += "Tap: " + tapItem.Id.ToString() + " ";
                }
                else
                {
                    dbgStr += "Tap: - ";
                }
                if (idleItem != null)
                {
                    maxCount = Mathf.Max(idleItem.maxCount, maxCount);
                    dbgStr += ", Idle: " + idleItem.Id.ToString() + " )";
                }
                else
                {
                    dbgStr += ", Idle: - )";
                }
                dbgStr += "\n";
                for (int i=0; i < maxCount; i++)
                {
                    double idleCoins = 0;
                    double tapCoins = 0;
                    double coins = 0;
                    double stepTime = 0;
                    //currentBonus = idleBonus + tapBonus;
                    coinsPerSec = tapBonus * Analyses_TapPerSec + idleBonus;
                    if (idleItem != null && i < idleItem.maxCount)
                    {
                        stepTime += Analyses_TimeInMenu;
                        idleCoins += GetIdleItemCoinPrice(idleItem.basePrice, i);
                        idleBonus += GetBonusMultiply(BonusTypes.Type.Idle, idleItem.ItemId, i+1);
                    }
                    if (tapItem != null && i < tapItem.maxCount)
                    {
                        stepTime += Analyses_TimeInMenu;
                        tapCoins += GetIdleItemCoinPrice(tapItem.basePrice, i);
                        int adjustedTapId =  tapItem.ItemId - Item.TapIdStart;
                        tapBonus = 1f;
                        for (int iTap=0; iTap < adjustedTapId; iTap++ )
                        {
                            tapBonus += GetTapBonus(iTap + Item.TapIdStart, 5);
                        }
                        //tapBonus = GetBonusMultiply(BonusTypes.Type.Tap, tapItem.ItemId, i+1);
                        tapBonus += GetTapBonus(tapItem.ItemId, i+1);
                    }

                    phaseIdleCoins += idleCoins;
                    phaseTapCoins += tapCoins;
                    coins = idleCoins + tapCoins;
                    stepTime += coins / coinsPerSec;
                    phaseTime += stepTime;
                    dbgStr += "Step :" + i 
                        + ", idleCoins: " + NumToStr.GetNumStr(idleCoins) 
                        + ", tapCoins: " + NumToStr.GetNumStr(tapCoins) 
                        + " = " + NumToStr.GetNumStr(coins)
                        + ", stepTime: " + NumToStr.GetTimeStr(stepTime)
                        + ", coinsPerSec: " + NumToStr.GetNumStr(coinsPerSec)
                        + ", new idleBonus: " + NumToStr.GetNumStr(idleBonus)
                        + ", new tapBonus: " + NumToStr.GetNumStr(tapBonus)
                        + "\n";
                }
                double levelCoins = GetLevelCoinPrice(level);
                double levelTime = levelCoins / coinsPerSec;
                string levelName = "-";
                if (level < levelItemList.Count)
                    levelName = levelItemList[level].Id.ToString();
                dbgStr += "Level: " + level 
                    + " Name: " + levelName
                    + ", Coins: " + NumToStr.GetNumStr(levelCoins) 
                    + ", Time: " + NumToStr.GetTimeStr(levelTime)
                    + ", coinsPerSec: " + NumToStr.GetNumStr(coinsPerSec)
                    + "\n";
                level++;
                phaseTime += levelTime;
                dbgStr += "Idle Coins: " + NumToStr.GetNumStr(phaseIdleCoins);
                dbgStr += ", Tap Coins " + NumToStr.GetNumStr(phaseTapCoins);
                dbgStr += ", Level Coins: " + NumToStr.GetNumStr(levelCoins);
                dbgStr += ", Total Coins " + NumToStr.GetNumStr(phaseIdleCoins+phaseTapCoins+ levelCoins);
                dbgStr += ", Phase Time: " + NumToStr.GetTimeStr(phaseTime) + "\n";
                chapterTime += phaseTime;
            }
            if (level > 1 && (level < levelItemList.Count))
            {
                dbgStr += "\nLevel Location: " + levelItemList[level].Id.ToString();
                double levelCoins = GetLevelCoinPrice(level);
                double levelTime = levelCoins / coinsPerSec;
                dbgStr += ", Coins: " + NumToStr.GetNumStr(levelCoins)
                    + ", Time: " + NumToStr.GetTimeStr(levelTime)
                    + ", coinsPerSec: " + NumToStr.GetNumStr(coinsPerSec)
                    + "\n";
                chapterTime += levelTime;
            }
            level++;

            dbgStr += "\nChapter total time: " + NumToStr.GetTimeStr(chapterTime) + "\n";
            totalGameTime += chapterTime;
            sw.Write(dbgStr);

        }
        dbgStr = "\n------------------------------\nTotal game time: " + NumToStr.GetTimeStr(totalGameTime) + "\n------------------------------\n";
        sw.Write(dbgStr);
        sw.Close();
        //Debug.Log(dbgStr);
    }
#endif
    private UpgradeItemIdle GetIdleItem(int chapter, int phase)
    {
        for (int itemId = 0; itemId < idleItemList.Count; itemId++)
        {
            if ((int)idleItemList[itemId].chapterId == chapter && idleItemList[itemId].ActivePhase == phase)
            {
                return idleItemList[itemId];
            }
        }
        return null;
    }
    private double Analyse_GetChapterIdleCoins(int chapter, int phase)
    {
        double coins = 0;
        for (int itemId = 0; itemId < idleItemList.Count; itemId++)
        {
            if ((int)idleItemList[itemId].chapterId == chapter && idleItemList[itemId].ActivePhase == phase)
            {
                for (int i=0; i < idleItemList[itemId].maxCount; i++)
                {
                    coins += GetIdleItemCoinPrice(idleItemList[itemId].basePrice, i);
                }
            }
        }
        return coins;
    }

    private UpgradeItemTap GetTapItem(int chapter, int phase)
    {
        for (int itemId = 0; itemId < tapItemList.Count; itemId++)
        {
            if ((int)tapItemList[itemId].chapterId == chapter && tapItemList[itemId].ActivePhase == phase)
            {
                return tapItemList[itemId];
            }
        }
        return null;
    }
    private double Analyse_GetChapterTapCoins(int chapter, int phase)
    {
        double coins = 0;
        for (int itemId = 0; itemId < tapItemList.Count; itemId++)
        {
            if ((int)tapItemList[itemId].chapterId == chapter && tapItemList[itemId].ActivePhase == phase)
            {
                for (int i = 0; i < tapItemList[itemId].maxCount; i++)
                {
                    coins += GetIdleItemCoinPrice(tapItemList[itemId].basePrice, i);
                }
            }
        }
        return coins;
    }
    private int Analyse_GetMaxChapterPhase(int chapter)
    {
        int maxPhase = 0;
        for (int i=0; i < levelItemList.Count; i++)
        {
            if ((int)levelItemList[i].chapterId == chapter)
            {
                if (levelItemList[i].ActivePhase > maxPhase)
                    maxPhase = levelItemList[i].ActivePhase;
            }
        }
        return maxPhase;
    }

    public int GetChapterMaxPhase(int chapter)
    {
        return Analyse_GetMaxChapterPhase(chapter);
    }
}
