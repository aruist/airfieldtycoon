using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    /// <summary>
    /// areaList will be created on LevelManager Awake.
    /// </summary>
    public List<LevelArea> areaList;
    /// <summary>
    /// levelItemsList will be created on LevelManager Awake.
    /// </summary>
    public List<LevelItem> levelItemsList;

    public int startingLevel;
    //public static LevelManager Instance = null;
    //public CameraFlow camFlow;

    public ManagerPositions managerPositions;

    public LevelArea[] levelAreas;

    public LevelItem[] levelUpgrades;
    public LevelItem[] idleUpgrades;
    public LevelItem[] tapUpgrades;

    private bool m_TapPending = false;
    private bool m_IdlePending = false;
    private bool m_LevelPending = false;

    public BusinessmanControl businessmanControl;

    private bool m_Initialized = false;
    // Use this for initialization

    void Awake()
    {
        GenerateLists();
    }
    void Start () {
        m_Initialized = true;
        //GenerateLists();
        CheckItems();
        //UpdateBusinessman();
        //CheckLevelUpgrades();
        //CheckTapUpgrades();
        //CheckIdleUpgrades();
    }

    private void GenerateLists()
    {
        if (areaList == null)
            areaList = new List<LevelArea>();
        else
            areaList.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            LevelArea lvlArea = transform.GetChild(i).GetComponent<LevelArea>();
            if (lvlArea != null)
                areaList.Add(lvlArea);
        }

        if (levelItemsList == null)
            levelItemsList = new List<LevelItem>();
        else
            levelItemsList.Clear();

        for (int i=0; i < areaList.Count; i++)
        {
            for (int j=0; j < areaList[i].transform.childCount; j++)
            {
                LevelItem lvlItem = areaList[i].transform.GetChild(j).GetComponent<LevelItem>();
                if (lvlItem != null)
                    levelItemsList.Add(lvlItem);
            }
        }
    }

    void Update()
    {
        if (m_TapPending)
        {
            m_TapPending = false;
            CheckItems();
            //CheckTapUpgrades();
        }
        if (m_IdlePending)
        {
            m_IdlePending = false;
            CheckItems();
            //CheckIdleUpgrades();
        }
        if (m_LevelPending)
        {
            m_LevelPending = false;
            CheckItems();
            //CheckLevelUpgrades();
        }

}

void OnDisable()
    {
        PlayerData.OnLevelChanged -= PlayerData_OnLevelChanged;
        PlayerData.OnTapUpgrade -= PlayerData_OnTapUpgrade;
        PlayerData.OnIdleUpgrade -= PlayerData_OnIdleUpgrade;
#if SOFTCEN_DEBUG
        Debug.Log("LevelManager OnDisable");
        //DevOptions.OnReset -= DevOptions_OnReset;
#endif
    }


    void OnEnable()
    {
        PlayerData.OnLevelChanged += PlayerData_OnLevelChanged;
        PlayerData.OnTapUpgrade += PlayerData_OnTapUpgrade;
        PlayerData.OnIdleUpgrade += PlayerData_OnIdleUpgrade;
#if SOFTCEN_DEBUG
        Debug.Log("LevelManager OnEnable");
        //DevOptions.OnReset += DevOptions_OnReset;
#endif
        if (m_Initialized)
        {
            /*if (m_BusinessmanControl != null)
            {
                UpdateBusinessman();
                if (!m_BusinessmanControl.gameObject.activeSelf)
                {
                    m_BusinessmanControl.gameObject.SetActive(true);
                }
            }*/
            CheckItems();
        }
    }

#if SOFTCEN_DEBUG
    /*
    private void DevOptions_OnReset()
    {
        for (int i = 0; i < levelUpgrades.Length; i++)
        {
            levelUpgrades[i].levelItemEnabled = false;
        }
        for (int i = 0; i < tapUpgrades.Length; i++)
        {
            tapUpgrades[i].levelItemEnabled = false;
        }
        for (int i = 0; i < idleUpgrades.Length; i++)
        {
            idleUpgrades[i].levelItemEnabled = false;
        }
        CheckItems();
        //CheckLevelUpgrades();
        //CheckTapUpgrades();
        //CheckIdleUpgrades();
    }*/
#endif

    private void PlayerData_OnIdleUpgrade()
    {
        m_IdlePending = true;
    }

    private void PlayerData_OnTapUpgrade()
    {
        m_TapPending = true;
    }

    public void PlayerData_OnLevelChanged()
    {
        //UpdateBusinessman();
    }

    /*private void UpdateBusinessman()
    {
        if (m_BusinessmanControl != null)
        {
            if (!m_BusinessmanControl.gameObject.activeSelf)
                m_BusinessmanControl.gameObject.SetActive(true);
            managerPositions.SetPositionAndCamFlow(m_BusinessmanControl, camFlow);
        }
    }*/

    private int GetSubLevel()
    {
        int level = GameManager.Instance.playerData.Level;
        int sublevel = level - startingLevel;
        if (sublevel < 0)
        {
#if SOFTCEN_DEBUG
            Debug.LogWarning("LEVEL Sublevel < 0");
#endif
            sublevel = 0;
        }
        return sublevel;
    }

    //public CameraFlowData testCamFlowData;
    /*
private void CheckLevelUpgrades()
{
    int level = GetSubLevel(); // GameManager.Instance.playerData.Level;
    CameraFlowData camFlowData = null;
    int camflowLevel = -1;
    for (int i=0; i < levelUpgrades.Length; i++)
    {
        if (!levelUpgrades[i].levelItemEnabled)
        {
            levelUpgrades[i].CheckLevelItem(level);
        }

        if (levelUpgrades[i].enableCountOrLvl <= level)
        {
            if (levelUpgrades[i].enableCountOrLvl > camflowLevel && levelUpgrades[i].camFlowData != null)
            {
                camFlowData = levelUpgrades[i].camFlowData;
                camflowLevel = levelUpgrades[i].enableCountOrLvl;
            }
        }
    }
    camFlow.TargetMain = m_BusinessmanControl.transform;
    testCamFlowData = camFlowData;
    if (camFlowData != null)
    {
        camFlow.SetParameters(camFlowData.camFlowParams);
    }
    else
    {
        camFlow.SetParameters(startCamFlowParams);
    }
    UpdateBusinessman();
}
    */

    /*
        private void CheckTapUpgrades()
        {
            //int level = GameManager.Instance.playerData.Level;
            for (int i = 0; i < tapUpgrades.Length; i++)
            {
                if (!tapUpgrades[i].levelItemEnabled)
                    tapUpgrades[i].CheckTapItem();
            }
        }
        private void CheckIdleUpgrades()
        {
            //int level = GameManager.Instance.playerData.Level;
            for (int i = 0; i < idleUpgrades.Length; i++)
            {
                if (!idleUpgrades[i].levelItemEnabled)
                    idleUpgrades[i].CheckIdleItem();
            }
        }
    */
    private void SetAreas(int level)
    {
        for (int i=0; i < levelAreas.Length; i++)
        {
            if (level >= levelAreas[i].activateOnLevel)
            {
                if (!levelAreas[i].gameObject.activeSelf)
                {
                    levelAreas[i].gameObject.SetActive(true);
                }
            }
            else
            {
                if (levelAreas[i].gameObject.activeSelf)
                {
                    levelAreas[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void CheckItems()
    {
        for (int i=0; i < levelItemsList.Count; i++)
        {
            levelItemsList[i].CheckItem();
        }
    }

    /*public bool IsPhasesCompleted()
    {
        for (int i = 0; i < levelItemsList.Count; i++)
        {
            if (levelItemsList[i].IsLevelItemInActive() == true)
            {
#if SOFTCEN_DEBUG
                Debug.Log("IsPhasesCompleted FALSE " + levelItemsList[i].gameObject.name);
#endif
                return false;
            }
        }
        return true;
    }*/

}
