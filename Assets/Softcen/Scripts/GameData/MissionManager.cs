using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Beebyte.Obfuscator;

public class MissionManager : MonoBehaviour {
    public static MissionManager Instance;
    public TextMeshProUGUI txtMissionTitle;
    //public Text txtMissionCount;
    private List<FishTypes.type> levelFishTypes;
    public TextMeshProUGUI txtMissionDlgDescription;
    public GameObject goMissionDlgDoubleBtn;
    public GameObject goMissionDlgAcceptBtn;
    public GameObject goMissionDlgOkBtn;

    public AudioClip acMissionReady;

    public GameObject goMissionReady;

    private string strMission = "";

    private bool m_MissionReady;
    private bool m_UpdateCountTextPending = false;
    private bool m_ActivateMissionNotification = false;

    private int[] tapCounts = new int[]
    {
        100,
        300,
        400,
        500,
        600,
        700,
        800,
        900,
        1100,
        1300,
        1500,
        2000,
        3000,
    };

	// Use this for initialization
	void Awake () {
        Instance = this;
        levelFishTypes = new List<FishTypes.type>();
        m_MissionReady = false;
        goMissionReady.SetActive(false);
    }

    void OnEnable() {
        PlayerData.OnMissionUpdated += PlayerData_OnMissionUpdated;
    }

    void PlayerData_OnMissionUpdated ()
    {
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        m_MissionReady = mi.IsMissionFinished ();
        m_UpdateCountTextPending = true;
    }

    void OnDisable() {
        PlayerData.OnMissionUpdated -= PlayerData_OnMissionUpdated;
    }
	
    void Start()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.playerData.CurrentMission.type == MissionTypes.type.None)
        {
            CreateNewMission();
        }
        else
        {
            UpdateAllTexts();
        }
    }

    #if UNITY_EDITOR
    public bool _paata_tehtava = false;
    #endif
	// Update is called once per frame
	void Update () {
	    if (m_UpdateCountTextPending)
        {
            m_UpdateCountTextPending = false;
            UpdateCountText();
        }
        if (m_ActivateMissionNotification)
        {
            m_ActivateMissionNotification = false;
        }
        #if UNITY_EDITOR
        if (_paata_tehtava) {
            _paata_tehtava = false;
            MissionItem mi = GameManager.Instance.playerData.CurrentMission;
            mi.paataTehtava ();
            paataTehtava();
        }
        #endif
    }

    public void SetupMissionDlg()
    {
        string txt = "";
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        switch (mi.type)
        {
            /*case MissionTypes.type.CatchFish:
                txt += "Catch " + mi.TargetCount.ToString() + " " + mi.currentFishType.ToString();
                break;*/
            case MissionTypes.type.TapXTimes:
                txt += "Tap display " + mi.TargetCount.ToString() + " times";
                break;
            case MissionTypes.type.UpgradeIdle:
                txt += "Buy " + mi.TargetCount.ToString()  + " Idle upgrades";
                break;
            case MissionTypes.type.UpgradeTap:
                txt += "Buy " + mi.TargetCount.ToString() + " Tap upgrades";
                break;

        }

        if (mi.IsMissionFinished())
        {
            goMissionDlgOkBtn.SetActive(false);
            goMissionDlgAcceptBtn.SetActive(true);
            if (GameManager.Instance.hasRewardVideo())
                goMissionDlgDoubleBtn.SetActive(true);
            else
                goMissionDlgDoubleBtn.SetActive(false);
            
            txt += "\nREWARD: " + NumToStr.GetNumStr(mi.GetBonus()) + " coins";
            txtMissionDlgDescription.SetText (txt);
        }
        else
        {
            txt += "\nREWARD: coins";
            txtMissionDlgDescription.SetText (txt);
            goMissionDlgOkBtn.SetActive(true);
            goMissionDlgAcceptBtn.SetActive(false);
            goMissionDlgDoubleBtn.SetActive(false);
        }
        
    }

    public void IncreaseTap()
    {
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        if (mi.type == MissionTypes.type.TapXTimes)
        {
            if (mi.AddCount())
            {
                m_ActivateMissionNotification = true;
            }
            m_UpdateCountTextPending = true;
        }
    }

    public void IncreaseIdleUpgrade()
    {
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        if (mi.type == MissionTypes.type.UpgradeIdle)
        {
            if (mi.AddCount())
            {
                m_ActivateMissionNotification = true;
            }
            m_UpdateCountTextPending = true;
        }

    }
    public void IncreaseTapUpgrade()
    {
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        if (mi.type == MissionTypes.type.UpgradeTap)
        {
            if (mi.AddCount())
            {
                m_ActivateMissionNotification = true;
            }
            m_UpdateCountTextPending = true;
        }
    }

    public void CatchFish(FishTypes.type fishType)
    {
        /*
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        if (mi.type == MissionTypes.type.CatchFish && mi.currentFishType == fishType)
        {
            if (mi.AddCount())
            {
            }
            UpdateCountText();
        }*/
    }

    public void ClearFishTypes()
    {
        levelFishTypes.Clear();
    }
    public void AddFishType(FishTypes.type fishType)
    {
        levelFishTypes.Add(fishType);
    }

    private void UpdateCountText()
    {
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        if (!m_MissionReady) {
            txtMissionTitle.SetText(string.Format(strMission, mi.CountLeft));
        } else {
            return;
        }

        if (mi.IsMissionFinished())
        {
            if (m_MissionReady == false)
            {
                paataTehtava ();
            }
        }
        else
        {
            //txtMissionCount.text = mi.CountLeft().ToString();
            if (goMissionReady.activeSelf)
                goMissionReady.SetActive(false);

        }

    }

    private void paataTehtava() {
        m_MissionReady = true;
        txtMissionTitle.SetText ("Mission ready!");
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        mi.LaskePalkinto ();
        GameManager.Instance.playerData.SetDirty ();
        AudioManager.Instance.PlayClip(acMissionReady);
        goMissionReady.SetActive(true);
    }


    public void UpdateAllTexts()
    {
        //string txt = "MISSION: ";
        if (GameManager.Instance == null || GameManager.Instance.playerData == null )
            return;

        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        if (mi == null)
            return;

        switch(mi.type)
        {
            /*case MissionTypes.type.CatchFish:
                //txt += "Catch " + mi.currentFishType.ToString() + " fish";
                strMission = "Catch {0} " + mi.currentFishType.ToString();
                break;*/
            case MissionTypes.type.TapXTimes:
                //txt += "Tap display x times";
                strMission = "Tap display {0} times ";
                break;
            case MissionTypes.type.UpgradeIdle:
                //txt += "Buy Idle upgrades";
                strMission = "Buy {0} Idle upgrades";
                break;
            case MissionTypes.type.UpgradeTap:
                //txt += "Buy Tap upgrades";
                strMission = "Buy {0} Tap upgrades";
                break;

        }
        //txtMissionCount.text = "";
        UpdateCountText();
    }

    private int GetTargetCount(MissionTypes.type missionType)
    {
        int currentLevel = GameManager.Instance.playerData.Level;
        int targetCount = 0;
        switch (missionType)
        {
            /*case MissionTypes.type.CatchFish:
                fishType = levelFishTypes[UnityEngine.Random.Range(0, levelFishTypes.Count)];
                if (currentLevel < 5)
                    targetCount = UnityEngine.Random.Range(4, 6);
                else
                    targetCount = UnityEngine.Random.Range(5, 10);
                break;*/
            case MissionTypes.type.TapXTimes:
                targetCount = GetTapXTimesTargetCount();
                break;
            case MissionTypes.type.UpgradeIdle:
                if (currentLevel < 5)
                {
                    targetCount = 1;
                }
                else
                {
                    targetCount = UnityEngine.Random.Range(1, 3);
                }
                break;
            case MissionTypes.type.UpgradeTap:
                if (currentLevel < 5)
                {
                    targetCount = 1;
                }
                else
                {
                    targetCount = UnityEngine.Random.Range(1, 3);
                }
                break;
        }
        return targetCount;

    }

    public void CreateNewMission()
    {
        double bonus = 0;
        int targetCount = 0;
        m_MissionReady = false;
        goMissionReady.SetActive(false);

        FishTypes.type fishType = FishTypes.type.None;
        System.Array A = System.Enum.GetValues(typeof(MissionTypes.type));
        MissionTypes.type missionType = MissionTypes.type.None;
        switch (GameManager.Instance.playerData.MissionCounter)
        {
            case 0:
                missionType = MissionTypes.type.TapXTimes;
                bonus = 50d;
                targetCount = 20;
                break;
            case 1:
                missionType = MissionTypes.type.UpgradeIdle;
                bonus = 80d;
                targetCount = 1;
                break;
            case 2:
                missionType = MissionTypes.type.UpgradeTap;
                targetCount = 1;
                bonus = 100d;
                break;
            default:
                missionType = (MissionTypes.type)A.GetValue(UnityEngine.Random.Range(1, A.Length));
                bonus = GameManager.Instance.playerData.GetCurrentTapValue() * Random.Range(30, 60);
                targetCount = GetTargetCount(missionType);
                break;
        }
#if SOFTCEN_DEBUG
        int currentLevel = GameManager.Instance.playerData.Level;
        Debug.Log("<color=aqua>CreateNewMission level: " + currentLevel
            + ", counter: " + GameManager.Instance.playerData.MissionCounter
            + ", new mission: " + missionType.ToString()
            + "</color>");
#endif

        GameManager.Instance.playerData.MissionCounter++;
        // Bonus coins:        
        GameManager.Instance.playerData.CurrentMission.ResetMission(missionType, targetCount, fishType, bonus);
        GameManager.Instance.Save();
        UpdateAllTexts();
    }

    private int GetTapXTimesTargetCount()
    {
        int currentLevel = GameManager.Instance.playerData.Level;
        int targetCount = 0;
        if (currentLevel < 5)
            targetCount = tapCounts[UnityEngine.Random.Range(0, 3)];
        else if (currentLevel < 10)
            targetCount = tapCounts[UnityEngine.Random.Range(1, 4)];
        else if (currentLevel < 15)
            targetCount = tapCounts[UnityEngine.Random.Range(2, 5)];
        else if (currentLevel < 20)
            targetCount = tapCounts[UnityEngine.Random.Range(3, 6)];
        else
            targetCount = tapCounts[UnityEngine.Random.Range(4, tapCounts.Length)];

        return targetCount;
    }

    [SkipRename]
    public void MissionDlgDoubleBonus()
    {
        AudioManager.Instance.PlayButtonClick();
        HomeManager.Instance.CloseAllDialogs();
        HomeManager.Instance.DoubleBonusRewardVideo();
    }

    [SkipRename]
    public void MissionDlgAccept()
    {
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.playerData.Score += GameConsts.Score.MissionDone;
        MissionItem mi = GameManager.Instance.playerData.CurrentMission;
        GameManager.Instance.playerData.IncMoney(mi.GetBonus());
        HomeManager.Instance.CloseAllDialogs();
        CreateNewMission();
    }


}
