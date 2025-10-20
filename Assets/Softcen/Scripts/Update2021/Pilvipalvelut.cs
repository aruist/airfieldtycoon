using System;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

public class Pilvipalvelut : MonoBehaviour
{
    public static Pilvipalvelut Instance;
    public static event Action OnLoggedOut;
    public static event Action OnSynchronizeReady;

    private bool initSync = false;
    public bool loggedOut = false; // Game needs to quit

    private long gameStartIndex = -1;
    public PlayerData cloudPlayerData = null;
    private GameManager gm;
    private bool syncOngoing = false;
    private bool _successRead = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gm = GameManager.Instance;
    }

    private void OnEnable()
    {
        // register for events
        Pelikeskus.OnStateChanged += Pelikeskus_OnStateChanged;
        CloudServices.OnUserChange += OnUserChange;
        CloudServices.OnSavedDataChange += OnSavedDataChange;
        CloudServices.OnSynchronizeComplete += OnSynchronizeComplete;
    }

    private void OnDisable()
    {
        // unregister from events
        Pelikeskus.OnStateChanged -= Pelikeskus_OnStateChanged;
        CloudServices.OnUserChange -= OnUserChange;
        CloudServices.OnSavedDataChange -= OnSavedDataChange;
        CloudServices.OnSynchronizeComplete -= OnSynchronizeComplete;
    }

    private void Pelikeskus_OnStateChanged()
    {
        if (!initSync)
        {
            if (Pelikeskus.Instance.Authenticated())
            {
                initSync = true;
                Syncronize();
            } else
            {
                if (OnSynchronizeReady != null)
                {
                    OnSynchronizeReady();
                }
            }
        }
    }

    public void CallSynchronizeReady()
    {
        if (OnSynchronizeReady != null)
        {
            OnSynchronizeReady();
        }
    }

    public void Syncronize()
    {
        #if SOFTCEN_DEBUG
        Debug.Log("Pilvipalvelut Syncronize() start syncOngoing: " + syncOngoing
            + ", authenticated: " + Pelikeskus.Instance.Authenticated()
            + ", CloudEnabled: " + gm.playerData.CloudEnabled
            + ", GP LoggedOut: " + gm.playerData.LoggedOut
            + ", _successRead: " + _successRead);
        #endif

        if (syncOngoing)
            return;

        if (gm.playerData.LoggedOut || !gm.playerData.CloudEnabled)
        {
            if (OnSynchronizeReady != null)
            {
                OnSynchronizeReady();
            }
            return;
        }

        #if UNITY_ANDROID
        if (!Pelikeskus.Instance.Authenticated())
        {
            return;
        }
        #endif

        syncOngoing = true;
        CloudServices.Synchronize();
    }

    #region Plugin event methods

    private void OnUserChange(CloudServicesUserChangeResult result, Error error)
    {
#if SOFTCEN_DEBUG
        var user = result.User;
        Debug.Log("Pilvipalvelut OnUserChange Received user change callback.");
        Debug.Log("Pilvipalvelut OnUserChange User id: " + user.UserId);
        Debug.Log("Pilvipalvelut OnUserChange User status: " + user.AccountStatus);
#endif
    }

    private void OnSavedDataChange(CloudServicesSavedDataChangeResult result)
    {
        bool synchronizeNeeded = false;
        string changedKey;
        var changedKeys = result.ChangedKeys;
        bool prestigeChanged = false;
        bool missionChanged = false;
        bool chapterOrPhaseChanged = false;
        bool idleBonusChanged = false;
        bool tapBonusChanged = false;
        bool lvlBonusChanged = false;
        bool levelChanged = false;
        bool tapChanged = false;
        bool idleChanged = false;

        if (cloudPlayerData == null)
            cloudPlayerData = new PlayerData();

#if SOFTCEN_DEBUG
        Debug.Log("Pilvipalvelut OnSavedDataChange Received saved data change callback.");
        Debug.Log("Pilvipalvelut OnSavedDataChange Reason: " + result.ChangeReason);
        Debug.Log("Pilvipalvelut OnSavedDataChange Total changed keys: " + changedKeys.Length);
        Debug.Log("Pilvipalvelut OnSavedDataChange Here are the changed keys:");
#endif
        for (int iter = 0; iter < changedKeys.Length; iter++)
        {
            changedKey = changedKeys[iter];
#if SOFTCEN_DEBUG
            if (result.ChangeReason != CloudSavedDataChangeReasonCode.InitialSyncChange)
            {
                Debug.Log(string.Format("Pilvipalvelut [{0}]: {1}", iter, changedKey));
            }
#endif
            if (changedKey.Equals(GameConsts.Cloud.GameStarted))
            {
                long start = CloudServices.GetLong(GameConsts.Cloud.GameStarted);
                if (result.ChangeReason == CloudSavedDataChangeReasonCode.InitialSyncChange)
                {
                    // Ei tarvi logged outtia näyttää
                    gameStartIndex = start;
                    gameStartIndex++;
                    CloudServices.SetLong(GameConsts.Cloud.GameStarted, gameStartIndex);
                    synchronizeNeeded = true;
                }
                else
                {
                    if (gameStartIndex < 0)
                        gameStartIndex = start;

                    if (start > gameStartIndex)
                    {
                        gameStartIndex = start;
                        CloudServices.SetLong(GameConsts.Cloud.GameStarted, gameStartIndex);
                        loggedOut = true;
                        if (OnLoggedOut != null)
                            OnLoggedOut();
                    }
                }
            }
            else if (changedKey.Equals(GameConsts.Cloud.PrestigeCount) || changedKey.Equals(GameConsts.Cloud.PrestigeLevel))
            {
                prestigeChanged = true;
            }
            else if (changedKey.Equals(GameConsts.Cloud.MissionCounter)
                || changedKey.Equals(GameConsts.Cloud.CurrentMissionTargetCount)
                || changedKey.Equals(GameConsts.Cloud.CurrentMissionCurrentCount)
                || changedKey.Equals(GameConsts.Cloud.CurrentMissionType)
                || changedKey.Equals(GameConsts.Cloud.CurrentMissionCurrentFishType)
                || changedKey.Equals(GameConsts.Cloud.CurrentMissionBonusCoins)
                || changedKey.Equals(GameConsts.Cloud.CurrentMissionLaskettu)
            )
            {
                missionChanged = true;
            }
            else if (changedKey.Equals(GameConsts.Cloud.IlmainenArkkuAvaamisAika))
            {
                cloudPlayerData.IlmainenArkkuAvaamisAika = CloudServices.GetLong(GameConsts.Cloud.IlmainenArkkuAvaamisAika);
                if (cloudPlayerData.IlmainenArkkuAvaamisAika > gm.playerData.IlmainenArkkuAvaamisAika)
                {
                    gm.playerData.IlmainenArkkuAvaamisAika = cloudPlayerData.IlmainenArkkuAvaamisAika;
                    if (ArKKuHaLLitSija.instance != null)
                    {
                        ArKKuHaLLitSija.instance.PaivitaAika(ArKKuTyyPPi.tyyppi.ILMAINEN, gm.playerData.IlmainenArkkuAvaamisAika);
                    }
                    CloudServices.SetLong(GameConsts.Cloud.IlmainenArkkuAvaamisAika, gm.playerData.IlmainenArkkuAvaamisAika);
                }
            }
            else if (changedKey.Equals(GameConsts.Cloud.Score))
            {
                cloudPlayerData.Score = CloudServices.GetLong(GameConsts.Cloud.Score);
                if (cloudPlayerData.Score > gm.playerData.Score)
                {
                    gm.playerData.Score = cloudPlayerData.Score;
                    CloudServices.SetLong(GameConsts.Cloud.Score, gm.playerData.Score);
                }
            }
            else if (changedKey.Equals(GameConsts.Cloud.MoneyEarned))
            {
                cloudPlayerData._moneyearned = CloudServices.GetDouble(GameConsts.Cloud.MoneyEarned);
                if (cloudPlayerData.MoneyEarned > gm.playerData.MoneyEarned)
                {
                    gm.playerData._moneyearned = cloudPlayerData._moneyearned;
                    CloudServices.SetDouble(GameConsts.Cloud.MoneyEarned, gm.playerData._moneyearned);
                }
            }
            else if (changedKey.Equals(GameConsts.Cloud.MoneyUsed))
            {
                cloudPlayerData._moneyused = CloudServices.GetDouble(GameConsts.Cloud.MoneyUsed);
                if (cloudPlayerData.MoneyUsed > gm.playerData.MoneyUsed)
                {
                    gm.playerData._moneyused = cloudPlayerData._moneyused;
                    CloudServices.SetDouble(GameConsts.Cloud.MoneyUsed, gm.playerData._moneyused);
                }
            }
            else if (changedKey.Equals(GameConsts.Cloud.Level))
            {
                cloudPlayerData._level = (int)CloudServices.GetLong(GameConsts.Cloud.Level);
                if (cloudPlayerData.Level > gm.playerData.Level)
                {
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pilvipalvelut Level Changed " + cloudPlayerData.Level + ", local was: " + gm.playerData.Level);
                    #endif
                    gm.playerData.SetLevel(cloudPlayerData.Level);
                    CloudServices.SetLong(GameConsts.Cloud.Level, (long)gm.playerData._level);
                }
            }
            else if (changedKey.Equals(GameConsts.Cloud.DiamondsPurchased))
            {
                cloudPlayerData._diamondsPurchased = (int)CloudServices.GetLong(GameConsts.Cloud.DiamondsPurchased);
                if (cloudPlayerData.DiamondsPurchased > gm.playerData.DiamondsPurchased)
                {
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pilvipalvelut DiamondsPurchased Changed " + cloudPlayerData.DiamondsPurchased + ", local was: " + gm.playerData.DiamondsPurchased);
                    #endif
                    gm.playerData.DiamondsPurchased = cloudPlayerData.DiamondsPurchased;
                    CloudServices.SetLong(GameConsts.Cloud.DiamondsPurchased, (long)gm.playerData._diamondsPurchased);
                }
            }
            else if (changedKey.Equals(GameConsts.Cloud.DiamondsSpend))
            {
                cloudPlayerData._diamondsSpend = (int)CloudServices.GetLong(GameConsts.Cloud.DiamondsSpend);
                if (cloudPlayerData.DiamondsSpend > gm.playerData.DiamondsSpend)
                {
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pilvipalvelut DiamondsSpend Changed " + cloudPlayerData.DiamondsSpend + ", local was: " + gm.playerData.DiamondsSpend);
                    #endif
                    gm.playerData.DiamondsSpend = cloudPlayerData.DiamondsSpend;
                    CloudServices.SetLong(GameConsts.Cloud.DiamondsSpend, (long)gm.playerData._diamondsSpend);
                }
            }
            else if (changedKey.Equals(GameConsts.Cloud.CurrentChapter) || changedKey.Equals(GameConsts.Cloud.CurrentPhase))
            {
                chapterOrPhaseChanged = true;
            }
            else if (changedKey.StartsWith(GameConsts.Cloud.IdleBonusBase))
            {
                idleBonusChanged = true;
            }
            else if (changedKey.StartsWith(GameConsts.Cloud.TapBonusBase))
            {
                tapBonusChanged = true;
            }
            else if (changedKey.StartsWith(GameConsts.Cloud.UpgradeBase))
            {
                lvlBonusChanged = true;
            }

            if (prestigeChanged)
            {
                cloudPlayerData.PrestigeCount = (int)CloudServices.GetLong(GameConsts.Cloud.PrestigeCount);
                cloudPlayerData.PrestigeLevel = (int)CloudServices.GetLong(GameConsts.Cloud.PrestigeLevel);
                if (cloudPlayerData.PrestigeCount > gm.playerData.PrestigeCount || cloudPlayerData.PrestigeLevel > gm.playerData._PrestigeCount)
                {
                    ReadAllFromCloud();
                    CompareLocalAndCloudData(true);
                    return;
                }
            }
            if (missionChanged)
            {
                ReadMissionFromCloud();
                if (cloudPlayerData.MissionCounter > gm.playerData.MissionCounter ||
                    (cloudPlayerData.MissionCounter == gm.playerData.MissionCounter &&
                        cloudPlayerData.CurrentMission.CurrentCount > gm.playerData.CurrentMission.CurrentCount))
                {
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pilvipalvelut Cloud key-value Update Mission"
                        + ", MissionCounter: " + cloudPlayerData.MissionCounter
                        + ", CurrentCount: " + cloudPlayerData.CurrentMission.CurrentCount
                        + ", TargetCount: " + cloudPlayerData.CurrentMission.TargetCount
                    );
                    #endif
                    gm.playerData.MissionCounter = cloudPlayerData.MissionCounter;
                    gm.playerData.CurrentMission.SetMission(cloudPlayerData.CurrentMission.type,
                        cloudPlayerData.CurrentMission.TargetCount,
                        cloudPlayerData.CurrentMission.CurrentCount,
                        cloudPlayerData.CurrentMission.currentFishType,
                        cloudPlayerData.CurrentMission.BonusCoins,
                        cloudPlayerData.CurrentMission.Laskettu
                    );
                    SaveMissionToCloud();
                    gm.playerData.CallOnMissionUpdated();
                }
            }
            if (chapterOrPhaseChanged)
            {
                cloudPlayerData.CurrentChapter = (int)CloudServices.GetLong(GameConsts.Cloud.CurrentChapter);
                cloudPlayerData.m_CurrentPhase = (int)CloudServices.GetLong(GameConsts.Cloud.CurrentPhase);
                if (cloudPlayerData.CurrentChapter > gm.playerData.CurrentChapter ||
                    (cloudPlayerData.CurrentChapter == gm.playerData.CurrentChapter && cloudPlayerData.m_CurrentPhase > gm.playerData.m_CurrentPhase))
                {
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pilvipalvelut CurrentChapter Changed " + cloudPlayerData.CurrentChapter
                        + ", phase: " + cloudPlayerData.m_CurrentPhase
                        + ", local was: " + gm.playerData.CurrentChapter
                        + ", phase: " + gm.playerData.m_CurrentPhase
                    );
                    #endif
                    levelChanged = true;
                    gm.playerData.CurrentChapter = cloudPlayerData.CurrentChapter;
                    gm.playerData.m_CurrentPhase = cloudPlayerData.m_CurrentPhase;
                    gm.selectedChapter = gm.playerData.CurrentChapter;
                    CloudServices.SetLong(GameConsts.Cloud.CurrentChapter, (long)gm.playerData.CurrentChapter);
                    CloudServices.SetLong(GameConsts.Cloud.CurrentPhase, (long)gm.playerData.m_CurrentPhase);
                }
            }
            if (idleBonusChanged)
            {
                for (int i = Item.IdleIdStart; i <= Item.IdleIdEnd; i++)
                {
                    cloudPlayerData.SetIdleBonus(i, (int)CloudServices.GetLong(GameConsts.Cloud.IdleBonusBase + i.ToString()));
                    int cloud = cloudPlayerData.GetIdleBonusCount(i);
                    int local = gm.playerData.GetIdleBonusCount(i);
                    if (cloud > local)
                    {
                        gm.playerData.SetIdleBonus(i, cloud);
                        SaveIdleBonusCountToCloud(i);
                        idleChanged = true;
                        #if SOFTCEN_DEBUG
                        Debug.Log("Pilvipalvelut IdleBonus Changed id: " + i + ", count: " + cloud);
                        #endif
                    }
                }
            }
            if (tapBonusChanged)
            {
                for (int i = Item.TapIdStart; i <= Item.TapIdEnd; i++)
                {
                    cloudPlayerData.SetTapBonus(i, (int)CloudServices.GetLong(GameConsts.Cloud.TapBonusBase + i.ToString()));
                    int cloud = cloudPlayerData.GetTapBonusCount(i);
                    int local = gm.playerData.GetTapBonusCount(i);
                    if (cloud > local)
                    {
                        tapChanged = true;
                        gm.playerData.SetTapBonus(i, cloud);
                        SaveTapBonusCountToCloud(i);
                        #if SOFTCEN_DEBUG
                        Debug.Log("Pilvipalvelut TapBonus Changed id: " + i + ", count: " + cloud);
                        #endif
                    }
                }
            }
            if (lvlBonusChanged)
            {
                for (int i = Item.LvlIdStart; i <= Item.LvlIdEnd; i++)
                {
                    cloudPlayerData.SetUpgrade(i, (int)CloudServices.GetLong(GameConsts.Cloud.UpgradeBase + i.ToString()));
                    int cloud = cloudPlayerData.GetUpgradeCount(i);
                    int local = gm.playerData.GetUpgradeCount(i);
                    if (cloud > local)
                    {
                        levelChanged = true;
                        gm.playerData.SetUpgrade(i, cloud);
                        SaveUpgradeCountToCloud(i);
                        #if SOFTCEN_DEBUG
                        Debug.Log("Pilvipalvelut LvlBonus Changed id: " + i + ", count: " + cloud);
                        #endif
                    }
                }
            }
        }

        if (synchronizeNeeded)
        {
            #if UNITY_ANDROID
            Syncronize();
            #endif
        }
        if (levelChanged)
        {
            gm.playerData.CallOnLevelChanged();
        }
        else
        {
            if (tapChanged)
            {
                gm.playerData.CalculateUpdates();
                gm.playerData.CallOnTapUpgrade();
            }
            if (idleChanged)
            {
                gm.playerData.CalculateUpdates();
                gm.playerData.CallOnIdleUpgrade();
            }
        }

    }

    private void OnSynchronizeComplete(CloudServicesSynchronizeResult result)
    {
        bool quitButtonPressed = GameManager.Instance != null ? GameManager.Instance.GetQuitButtonPressed() : false;
        #if SOFTCEN_DEBUG
        Debug.Log($"Pilvipalvelut OnSynchronizeComplete result: {result.Success}, quitButtonPressed: {quitButtonPressed}");
        #endif

        if (quitButtonPressed)
        {
            syncOngoing = false;
            Application.Quit();
            return;
        }

        if (result.Success)
        {
            _successRead = true;
            if (ReadAllFromCloud())
            {
                CompareLocalAndCloudData(true);
            }
        }
        if (OnSynchronizeReady != null)
        {
            OnSynchronizeReady();
        }
        syncOngoing = false;
    }


    public bool ReadAllFromCloud()
    {
        #if SOFTCEN_DEBUG
        Debug.Log("Pilvipalvelut ReadAllFromCloud CloudEnabled: " + gm.playerData.CloudEnabled
            + ", Authenticated: " + Pelikeskus.Instance.Authenticated()
            + ", _successRead: " + _successRead);
        #endif
        if (gm.playerData.CloudEnabled == false)
            return false;

        #if UNITY_ANDROID
        if (!Pelikeskus.Instance.Authenticated())
        {
            return false;
        }
        #endif

#pragma warning disable CS0168
        try
        {
            if (cloudPlayerData == null)
                cloudPlayerData = new PlayerData();
            // --------------------------------
            //long version = NPBinding.CloudServices.GetLong(GameConsts.Cloud.Version);

            cloudPlayerData._moneyused = CloudServices.GetDouble(GameConsts.Cloud.MoneyUsed);
            cloudPlayerData._moneyearned = CloudServices.GetDouble(GameConsts.Cloud.MoneyEarned);
            for (int i = Item.IdleIdStart; i <= Item.IdleIdEnd; i++)
            {
                cloudPlayerData.SetIdleBonus(i, (int)CloudServices.GetLong(GameConsts.Cloud.IdleBonusBase + i.ToString()));
            }
            for (int i = Item.TapIdStart; i <= Item.TapIdEnd; i++)
            {
                cloudPlayerData.SetTapBonus(i, (int)CloudServices.GetLong(GameConsts.Cloud.TapBonusBase + i.ToString()));
            }
            for (int i = Item.LvlIdStart; i <= Item.LvlIdEnd; i++)
            {
                cloudPlayerData.SetUpgrade(i, (int)CloudServices.GetLong(GameConsts.Cloud.UpgradeBase + i.ToString()));
            }

            cloudPlayerData._level = (int)CloudServices.GetLong(GameConsts.Cloud.Level);
            if (cloudPlayerData._level == 0)
                cloudPlayerData.SetLevel(0);

            cloudPlayerData._sublevel = (int)CloudServices.GetLong(GameConsts.Cloud.SubLevel);

            ReadMissionFromCloud();

            cloudPlayerData.CurrentChapter = (int)CloudServices.GetLong(GameConsts.Cloud.CurrentChapter);
            cloudPlayerData.m_CurrentPhase = (int)CloudServices.GetLong(GameConsts.Cloud.CurrentPhase);

            cloudPlayerData.MissionCounter = (int)CloudServices.GetLong(GameConsts.Cloud.MissionCounter);

            cloudPlayerData._diamondsPurchased = (int)CloudServices.GetLong(GameConsts.Cloud.DiamondsPurchased);
            if (cloudPlayerData._diamondsPurchased == 0)
                cloudPlayerData.DiamondsPurchased = 0;
            cloudPlayerData._diamondsSpend = (int)CloudServices.GetLong(GameConsts.Cloud.DiamondsSpend);
            if (cloudPlayerData._diamondsSpend == 0)
                cloudPlayerData.DiamondsSpend = 0;

            cloudPlayerData.PrestigeCount = (int)CloudServices.GetLong(GameConsts.Cloud.PrestigeCount);
            cloudPlayerData.PrestigeLevel = (int)CloudServices.GetLong(GameConsts.Cloud.PrestigeLevel);
            cloudPlayerData.Score = CloudServices.GetLong(GameConsts.Cloud.Score);

            cloudPlayerData.IlmainenArkkuAvaamisAika = CloudServices.GetLong(GameConsts.Cloud.IlmainenArkkuAvaamisAika);

            // --------------------------------

            #if SOFTCEN_DEBUG
            Debug.Log("Pilvipalvelut cloudPlayerData: coins: " + cloudPlayerData.Money
                + ", chapter: " + cloudPlayerData.CurrentChapter
                + ", phase: " + cloudPlayerData.CurrentPhase
                + ", MoneyEarned: " + cloudPlayerData.MoneyEarned
                + ", MoneyUsed: " + cloudPlayerData.MoneyUsed
            );
            string dbgStr = "GameManager IdleBonus:\n";
            for (int i = 0; i < cloudPlayerData.IdleBonusCount.Count; i++)
            {
                dbgStr += "id: " + cloudPlayerData.IdleBonusCount[i].id
                    + ", count: " + cloudPlayerData.IdleBonusCount[i].count
                    + "\n";
            }
            Debug.Log(dbgStr);
            #endif

            // Game started indication
            gameStartIndex = CloudServices.GetLong(GameConsts.Cloud.GameStarted);
            gameStartIndex++;
            CloudServices.SetLong(GameConsts.Cloud.GameStarted, gameStartIndex);
            #if SOFTCEN_DEBUG
            Debug.Log("Pilvipalvelut New gamestart index: " + gameStartIndex);
            #endif
        }
        catch (Exception e)
        {
            #if SOFTCEN_DEBUG
            Debug.Log("Pilvipalvelut Something went wrong e: " + e.Message);
            #endif
            return false;
        }
#pragma warning restore CS0168

        #if SOFTCEN_DEBUG
        Debug.Log("Pilvipalvelut ReadAllFromCloud OK!");
        #endif
        return true;
    }

    public void ReadMissionFromCloud()
    {
        if (cloudPlayerData != null)
        {
            int type = (int)CloudServices.GetLong(GameConsts.Cloud.CurrentMissionType);
            int targetcount = (int)CloudServices.GetLong(GameConsts.Cloud.CurrentMissionTargetCount);
            int currentcount = (int)CloudServices.GetLong(GameConsts.Cloud.CurrentMissionCurrentCount);
            int fishtype = (int)CloudServices.GetLong(GameConsts.Cloud.CurrentMissionCurrentFishType);
            double bonus = CloudServices.GetDouble(GameConsts.Cloud.CurrentMissionBonusCoins);
            bool laskettu = CloudServices.GetBool(GameConsts.Cloud.CurrentMissionLaskettu);
            #if SOFTCEN_DEBUG
            Debug.Log("Pilvipalvelut ReadMissionFromCloud "
                + ", type: " + type
                + ", targetcount: " + targetcount
                + ", currentcount: " + currentcount
                + ", bonus: " + bonus
                + ", laskettu: " + laskettu
            );
            #endif
            if (cloudPlayerData.CurrentMission == null)
            {
                cloudPlayerData.CurrentMission = new MissionItem();
            }
            cloudPlayerData.CurrentMission.SetMission((MissionTypes.type)type, targetcount, currentcount, (FishTypes.type)fishtype, bonus, laskettu);
        }
    }
    public void SaveMissionToCloud()
    {
#if SOFTCEN_DEBUG
        Debug.Log("SaveMissionToCloud _successRead: " + _successRead);
#endif
        CloudServices.SetLong(GameConsts.Cloud.CurrentMissionType, (long)gm.playerData.CurrentMission.type);
        CloudServices.SetLong(GameConsts.Cloud.CurrentMissionTargetCount, (long)gm.playerData.CurrentMission.TargetCount);
        CloudServices.SetLong(GameConsts.Cloud.CurrentMissionCurrentCount, (long)gm.playerData.CurrentMission.CurrentCount);
        CloudServices.SetLong(GameConsts.Cloud.CurrentMissionCurrentFishType, (long)gm.playerData.CurrentMission.currentFishType);
        CloudServices.SetDouble(GameConsts.Cloud.CurrentMissionBonusCoins, (double)gm.playerData.CurrentMission.BonusCoins);
        CloudServices.SetBool(GameConsts.Cloud.CurrentMissionLaskettu, (bool)gm.playerData.CurrentMission.Laskettu);
        CloudServices.SetLong(GameConsts.Cloud.MissionCounter, (long)gm.playerData.MissionCounter);
    }
    public void SaveUpgradeCountToCloud(int id)
    {
        CloudServices.SetLong(GameConsts.Cloud.UpgradeBase + id.ToString(), (long)gm.playerData.GetUpgradeCount(id));
    }
    public void SaveIdleBonusCountToCloud(int id)
    {
        CloudServices.SetLong(GameConsts.Cloud.IdleBonusBase + id.ToString(), (long)gm.playerData.GetIdleBonusCount(id));
    }
    public void SaveTapBonusCountToCloud(int id)
    {
        CloudServices.SetLong(GameConsts.Cloud.TapBonusBase + id.ToString(), (long)gm.playerData.GetTapBonusCount(id));
    }

    public void SaveAllDataToCloud()
    {
        #if SOFTCEN_DEBUG
        Debug.Log("Pilvipalvelut SaveAllDataToCloud, CloudEnabled: " + gm.playerData.CloudEnabled
            + ", authenticated: " + Pelikeskus.Instance.Authenticated()
            + ", _successRead: " + _successRead);
        #endif
        if (gm.playerData.CloudEnabled == false || !_successRead)
            return;

#if UNITY_ANDROID
        if (!Pelikeskus.Instance.Authenticated())
        {
#if SOFTCEN_DEBUG
            Debug.Log("Pilvipalvelut SaveAllDataToCloud not authenticated skip");
#endif
            return;
        }
#endif

#if SOFTCEN_DEBUG
        Debug.Log("Pilvipalvelut SaveAllDataToCloud"
            + ", _moneyused: " + gm.playerData._moneyused
            + ", _moneyearned: " + gm.playerData._moneyearned
            + ", Score: " + gm.playerData.Score
            + ", Level: " + gm.playerData._level
            + ", SubLevel: " + gm.playerData._sublevel
            + ", DiamondsPurchased: " + gm.playerData._diamondsPurchased
            + ", DiamondsSpend: " + gm.playerData._diamondsSpend
        );
#endif

        // --------------------------------
        CloudServices.SetLong(GameConsts.Cloud.Version, (long)gm.playerData.version);

        CloudServices.SetDouble(GameConsts.Cloud.MoneyUsed, gm.playerData._moneyused);
        CloudServices.SetDouble(GameConsts.Cloud.MoneyEarned, gm.playerData._moneyearned);

        for (int i = Item.IdleIdStart; i <= Item.IdleIdEnd; i++)
        {
            SaveIdleBonusCountToCloud(i);
        }

        for (int i = Item.TapIdStart; i <= Item.TapIdEnd; i++)
        {
            SaveTapBonusCountToCloud(i);
        }
        for (int i = Item.LvlIdStart; i <= Item.LvlIdEnd; i++)
        {
            SaveUpgradeCountToCloud(i);
        }

        CloudServices.SetLong(GameConsts.Cloud.Level, (long)gm.playerData._level);
        CloudServices.SetLong(GameConsts.Cloud.SubLevel, (long)gm.playerData._sublevel);
        SaveMissionToCloud();

        CloudServices.SetLong(GameConsts.Cloud.CurrentChapter, (long)gm.playerData.CurrentChapter);
        CloudServices.SetLong(GameConsts.Cloud.CurrentPhase, (long)gm.playerData.m_CurrentPhase);

        CloudServices.SetLong(GameConsts.Cloud.DiamondsPurchased, (long)gm.playerData._diamondsPurchased);
        CloudServices.SetLong(GameConsts.Cloud.DiamondsSpend, (long)gm.playerData._diamondsSpend);

        CloudServices.SetLong(GameConsts.Cloud.PrestigeCount, (long)gm.playerData.PrestigeCount);
        CloudServices.SetLong(GameConsts.Cloud.PrestigeLevel, (long)gm.playerData.PrestigeLevel);
        CloudServices.SetLong(GameConsts.Cloud.Score, gm.playerData.Score);

        CloudServices.SetLong(GameConsts.Cloud.IlmainenArkkuAvaamisAika, gm.playerData.IlmainenArkkuAvaamisAika);
        // --------------------------------
    }

    public void RemoveAllCloudKeys()
    {

        #if SOFTCEN_DEBUG
        Debug.Log("Pilvipalvelut RemoveAllCloudKeys");
        #endif

        CloudServices.RemoveKey("Version");
        CloudServices.RemoveKey("af");
        CloudServices.RemoveKey("money");

        for (int i = Item.IdleIdStart; i <= Item.IdleIdEnd; i++)
        {
            CloudServices.RemoveKey("IdleBonusCount" + i.ToString());
        }

        for (int i = Item.TapIdStart; i <= Item.TapIdEnd; i++)
        {
            CloudServices.RemoveKey("TapBonusCount" + i.ToString());
        }
        for (int i = Item.LvlIdStart; i <= Item.LvlIdEnd; i++)
        {
            CloudServices.RemoveKey("UpgradeCount" + i.ToString());
        }

        CloudServices.RemoveKey("level");

        CloudServices.RemoveKey("sublevel");

        CloudServices.RemoveKey("asklater");
        CloudServices.RemoveKey("likeAsked");
        CloudServices.RemoveKey("playerLike");
        CloudServices.RemoveKey("powerSave");
        CloudServices.RemoveKey("autotap");
        // Mission:
        CloudServices.RemoveKey("MisType");
        CloudServices.RemoveKey("MisTargetCount");
        CloudServices.RemoveKey("MisCurCount");
        CloudServices.RemoveKey("MisFT");
        CloudServices.RemoveKey("MisBC");
        CloudServices.RemoveKey("CurrentChapter");
        CloudServices.RemoveKey("CurrentPhase");
        CloudServices.RemoveKey("MissionCounter");

        CloudServices.RemoveKey("DPurchased");
        CloudServices.RemoveKey("DSpend");
    }

    public void CompareLocalAndCloudData(bool sentEvent)
    {
        #if SOFTCEN_DEBUG
        Debug.Log("Pilvipalvelut CompareLocalAndCloudData sentEvent: " + sentEvent + ", CloudEnabled: " + gm.playerData.CloudEnabled);
        #endif
        if (gm.playerData.CloudEnabled == false)
            return;

        if (gm.playerData != null && cloudPlayerData != null)
        {
            //bool cloudIsNewer = false;
            bool levelChanged = false;
            bool tapChanged = false;
            bool idleChanged = false;
            bool prestigeIncreased = false;
            #if SOFTCEN_DEBUG
            Debug.Log("Pilvipalvelut cloudPlayerData.PrestigeCount: " + cloudPlayerData.PrestigeCount + ", playerData.PrestigeCount: " + gm.playerData.PrestigeCount);
            #endif
            if (cloudPlayerData.PrestigeCount > gm.playerData.PrestigeCount)
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pilvipalvelut Cloud key-value Prestige new: " + cloudPlayerData.PrestigeCount + ", local was: " + gm.playerData.PrestigeCount);
                #endif
                prestigeIncreased = true;
                gm.playerData.PrestigeCount = cloudPlayerData.PrestigeCount;
                gm.playerData.PrestigeLevel = cloudPlayerData.PrestigeLevel;
            }

            if (cloudPlayerData.Score > gm.playerData.Score)
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pilvipalvelut Score updated from " + gm.playerData.Score
                    + " to " + cloudPlayerData.Score);
                #endif
                gm.playerData.Score = cloudPlayerData.Score;
            }

            if (cloudPlayerData.IlmainenArkkuAvaamisAika > gm.playerData.IlmainenArkkuAvaamisAika)
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pilvipalvelut IlmainenArkkuAvaamisAika updated: " + gm.playerData.IlmainenArkkuAvaamisAika);
                #endif
                gm.playerData.IlmainenArkkuAvaamisAika = cloudPlayerData.IlmainenArkkuAvaamisAika;
                if (ArKKuHaLLitSija.instance != null)
                {
                    ArKKuHaLLitSija.instance.PaivitaAika(ArKKuTyyPPi.tyyppi.ILMAINEN, gm.playerData.IlmainenArkkuAvaamisAika);
                }
            }

            if (cloudPlayerData.Level > gm.playerData.Level || prestigeIncreased)
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pilvipalvelut Cloud key-value Level new: " + cloudPlayerData.Level + ", local was: " + gm.playerData.Level);
                #endif
                //cloudIsNewer = true;
                gm.playerData.SetLevel(cloudPlayerData.Level);
                levelChanged = true;
            }

            if (cloudPlayerData.CurrentChapter > gm.playerData.CurrentChapter || prestigeIncreased)
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pilvipalvelut Cloud key-value Chapter new: " + cloudPlayerData.CurrentChapter + ", local was: " + gm.playerData.CurrentChapter);
                #endif
                levelChanged = true;
                //cloudIsNewer = true;
                gm.playerData.CurrentChapter = cloudPlayerData.CurrentChapter;
                gm.playerData.CurrentPhase = cloudPlayerData.CurrentPhase;
            }
            else if (cloudPlayerData.CurrentChapter == gm.playerData.CurrentChapter && cloudPlayerData.CurrentPhase > gm.playerData.CurrentPhase)
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pilvipalvelut Cloud key-value Chapter new: "
                    + cloudPlayerData.CurrentChapter + ", phase: " + cloudPlayerData.CurrentPhase
                    + ", local was: " + gm.playerData.CurrentChapter + ", phase: " + gm.playerData.CurrentPhase
                    );
                #endif
                levelChanged = true;
                //cloudIsNewer = true;
                gm.playerData.CurrentPhase = cloudPlayerData.CurrentPhase;
            }

            for (int i = Item.IdleIdStart; i <= Item.IdleIdEnd; i++)
            {
                int cloud = cloudPlayerData.GetIdleBonusCount(i);
                int local = gm.playerData.GetIdleBonusCount(i);
                if (cloud > local || prestigeIncreased)
                {
                    //cloudIsNewer = true;
                    gm.playerData.SetIdleBonus(i, cloud);
                    idleChanged = true;
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pilvipalvelut Cloud key-value IdleBonus id: " + i + ", count: " + cloud + ", local was: " + local);
                    #endif
                }
            }
            for (int i = Item.TapIdStart; i <= Item.TapIdEnd; i++)
            {
                int cloud = cloudPlayerData.GetTapBonusCount(i);
                int local = gm.playerData.GetTapBonusCount(i);
                if (cloud > local || prestigeIncreased)
                {
                    //cloudIsNewer = true;
                    gm.playerData.SetTapBonus(i, cloud);
                    tapChanged = true;
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pilvipalvelut Cloud key-value TapBonus id: " + i + ", count: " + cloud + ", local was: " + local);
                    #endif
                }
            }
            for (int i = Item.LvlIdStart; i <= Item.LvlIdEnd; i++)
            {
                int cloud = cloudPlayerData.GetUpgradeCount(i);
                int local = gm.playerData.GetUpgradeCount(i);
                if (cloud > local || prestigeIncreased)
                {
                    //cloudIsNewer = true;
                    gm.playerData.SetUpgrade(i, cloud);
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pilvipalvelut Cloud key-value LvlBonus id: " + i + ", count: " + cloud + ", local was: " + local);
                    #endif
                }
            }

            #if SOFTCEN_DEBUG
            Debug.Log("Pilvipalvelut MissionCounter: " + cloudPlayerData.MissionCounter + " vs player: " + gm.playerData.MissionCounter
                + ", CurrentCount: " + cloudPlayerData.CurrentMission.CurrentCount
                + ", TargetCount: " + cloudPlayerData.CurrentMission.TargetCount
            );
            #endif
            if (cloudPlayerData.MissionCounter > gm.playerData.MissionCounter ||
                (cloudPlayerData.MissionCounter == gm.playerData.MissionCounter &&
                cloudPlayerData.CurrentMission.CurrentCount > gm.playerData.CurrentMission.CurrentCount
                || prestigeIncreased))
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pilvipalvelut Cloud key-value Update Mission"
                    + ", MissionCounter: " + cloudPlayerData.MissionCounter
                    + ", CurrentCount: " + cloudPlayerData.CurrentMission.CurrentCount
                    + ", TargetCount: " + cloudPlayerData.CurrentMission.TargetCount
                );
                #endif
                //cloudIsNewer = true;
                gm.playerData.MissionCounter = cloudPlayerData.MissionCounter;
                gm.playerData.CurrentMission.SetMission(cloudPlayerData.CurrentMission.type,
                    cloudPlayerData.CurrentMission.TargetCount,
                    cloudPlayerData.CurrentMission.CurrentCount,
                    cloudPlayerData.CurrentMission.currentFishType,
                    cloudPlayerData.CurrentMission.BonusCoins,
                    cloudPlayerData.CurrentMission.Laskettu
                    );
                gm.playerData.CallOnMissionUpdated();
            }

            if (cloudPlayerData.DiamondsPurchased > gm.playerData.DiamondsPurchased)
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pilvipalvelut Cloud key-value DiamondsPurchased:  " + cloudPlayerData.DiamondsPurchased + ", local was: " + gm.playerData.DiamondsPurchased);
                #endif
                //cloudIsNewer = true;
                gm.playerData.DiamondsPurchased = cloudPlayerData.DiamondsPurchased;
            }
            if (cloudPlayerData.DiamondsSpend > gm.playerData.DiamondsSpend)
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pilvipalvelut Cloud key-value DiamondsSpend:  " + cloudPlayerData.DiamondsSpend + ", local was: " + gm.playerData.DiamondsSpend);
                #endif
                //cloudIsNewer = true;
                gm.playerData.DiamondsSpend = cloudPlayerData.DiamondsSpend;
            }

            if (cloudPlayerData.MoneyEarned > gm.playerData.MoneyEarned || prestigeIncreased)
            {
                gm.playerData._moneyearned = cloudPlayerData._moneyearned;
            }
            if (cloudPlayerData.MoneyUsed > gm.playerData.MoneyUsed || prestigeIncreased)
            {
                gm.playerData._moneyused = cloudPlayerData._moneyused;
            }

            gm.playerData.CalculateUpdates();
            GameManager.Instance.Save();

            if (sentEvent)
            {
                if (levelChanged)
                {
                    gm.playerData.CallOnLevelChanged();
                }
                else
                {
                    if (tapChanged)
                    {
                        gm.playerData.CalculateUpdates();
                        gm.playerData.CallOnTapUpgrade();
                    }
                    if (idleChanged)
                    {
                        gm.playerData.CalculateUpdates();
                        gm.playerData.CallOnIdleUpgrade();
                    }
                }
            }
        }
        #if SOFTCEN_DEBUG
        Debug.Log("Pilvipalvelut CompareLocalAndCloudData End");
        #endif

    }

    public void PrestigeCloudKeys()
    {
        if (gm.playerData.CloudEnabled == false)
            return;

        for (int i = Item.IdleIdStart; i <= Item.IdleIdEnd; i++)
        {
            CloudServices.RemoveKey(GameConsts.Cloud.IdleBonusBase + i.ToString());
        }

        for (int i = Item.TapIdStart; i <= Item.TapIdEnd; i++)
        {
            CloudServices.RemoveKey(GameConsts.Cloud.TapBonusBase + i.ToString());
        }
        for (int i = Item.LvlIdStart; i <= Item.LvlIdEnd; i++)
        {
            CloudServices.RemoveKey(GameConsts.Cloud.UpgradeBase + i.ToString());
        }

    }

    #endregion

}
