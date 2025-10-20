using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Beebyte.Obfuscator;

public class LevelResources : MonoBehaviour {
    public enum State
    {
        IDLE,
        START_LOAD,
        LOADING,
        LOAD_READY
    }
    private State m_state;

    public GameObject goMainMapButton;
    public GameObject goGoCurrentButton;
    public GameObject goLevelChange;
    public CommonDialog levelChangeDlg;

    //public Text txtTopPanelLevel;
    //public Text txtTopPanelChapterName;

    public CameraFlow camFlow;
    //public BusinessmanControl m_BusinessmanControl;

    public LevelManager currentLevelManager = null;
    public string currentLevelName = "";

#if SOFTCEN_DEBUG
    public int test_phasePos;
    public bool test_changePos = false;
    private float usedTime;
#endif


    //private GameObject goDebugLevel;
    // Use this for initialization
    void Start () {
        m_state = State.IDLE;
        GameObject goDebugLevel = GameObject.FindGameObjectWithTag("Level");
        if (goDebugLevel != null)
            goDebugLevel.SetActive(false);

        PlayerData_OnLevelChanged();
        //LoadLevelResources();
    }

    void OnDisable()
    {
        m_state = State.IDLE;
        PlayerData.OnLevelChanged -= PlayerData_OnLevelChanged;
    }
    void OnEnable()
    {
        m_state = State.IDLE;
        PlayerData.OnLevelChanged += PlayerData_OnLevelChanged;
    }

    public bool test_changeChapter = false;
    public int test_Chapter;
    void Update()
    {
        if (m_state == State.START_LOAD)
        {
            m_state = State.LOADING;
            StartCoroutine(LoadLevel());
        }
        else if (m_state == State.LOAD_READY)
        {
            m_state = State.IDLE;
            CheckChapterComplete();
            levelChangeDlg.Button_Close();
        }

#if SOFTCEN_DEBUG
        if (test_changePos)
        {
            test_changePos = false;
            // Remove chapter items
            int currentChapter = GameManager.Instance.playerData.CurrentChapter;
            GameManager.Instance.playerData.CurrentPhase = test_phasePos;
            for (int i = 0; i < BonusManager.Instance.idleItemList.Count; i++)
            {
                if ((int)BonusManager.Instance.idleItemList[i].chapterId == currentChapter)
                {
                    if (BonusManager.Instance.idleItemList[i].ActivePhase <= test_phasePos)
                    {
                        GameManager.Instance.playerData.AddIdleBonus(BonusManager.Instance.idleItemList[i].ItemId, BonusManager.Instance.idleItemList[i].maxCount);
                    }
                }
            }
            for (int i = 0; i < BonusManager.Instance.tapItemList.Count; i++)
            {
                if ((int)BonusManager.Instance.tapItemList[i].chapterId == currentChapter)
                {
                    if (BonusManager.Instance.tapItemList[i].ActivePhase <= test_phasePos)
                    {
                        GameManager.Instance.playerData.AddTapBonus(BonusManager.Instance.tapItemList[i].ItemId, BonusManager.Instance.tapItemList[i].maxCount);
                    }
                }
            }
            GameManager.Instance.playerData.CallOnLevelChanged();

            //currentLevelManager.managerPositions.SetPositionAndCamFlow(currentLevelManager.businessmanControl, camFlow, test_phasePos);
        }
        if (test_changeChapter)
        {
            test_changeChapter = false;
            GameManager.Instance.playerData.CurrentChapter = test_Chapter;
            GameManager.Instance.selectedChapter = test_Chapter;
            GameManager.Instance.playerData.CurrentPhase = 0;
            GameManager.Instance.playerData.SetLevel(BonusManager.Instance.GetCurrentChapterStartLevel());

            // Remove chapter items
            for (int i=0; i < BonusManager.Instance.idleItemList.Count; i++)
            {
                if ((int)BonusManager.Instance.idleItemList[i].chapterId == test_Chapter)
                {
                    GameManager.Instance.playerData.ResetIdleBonusCount(BonusManager.Instance.idleItemList[i].ItemId);
                }
                else if ((int)BonusManager.Instance.idleItemList[i].chapterId < test_Chapter)
                {
                    GameManager.Instance.playerData.AddIdleBonus(BonusManager.Instance.idleItemList[i].ItemId, BonusManager.Instance.idleItemList[i].maxCount);
                }
            }
            for (int i = 0; i < BonusManager.Instance.tapItemList.Count; i++)
            {
                if ((int)BonusManager.Instance.tapItemList[i].chapterId == test_Chapter)
                {
                    GameManager.Instance.playerData.ResetTapBonusCount(BonusManager.Instance.tapItemList[i].ItemId);
                }
                else if ((int)BonusManager.Instance.tapItemList[i].chapterId < test_Chapter)
                {
                    GameManager.Instance.playerData.AddTapBonus(BonusManager.Instance.tapItemList[i].ItemId, BonusManager.Instance.tapItemList[i].maxCount);
                }
            }
            GameManager.Instance.playerData.CallOnLevelChanged();
        }
#endif

    }


    public void PlayerData_OnLevelChanged()
    {

        /*if (goDebugLevel != null)
        {
            currentLevelManager = goDebugLevel.GetComponent<LevelManager>();
            SetBusinessManActive(true);
            currentLevelManager.CheckItems();

            if (!goDebugLevel.activeSelf)
                goDebugLevel.SetActive(true);
            return;
        }*/
#if SOFTCEN_DEBUG
        usedTime = Time.realtimeSinceStartup;
        Debug.Log("LevelResources PlayerData_OnLevelChanged CurrentChapter: " 
            + GameManager.Instance.playerData.CurrentChapter 
            + ", Phase: " + GameManager.Instance.playerData.CurrentPhase);
#endif
        bool isGMNull = GameManager.Instance == null;
        if (isGMNull || GameManager.Instance.playerData.CurrentChapter <= 0)
        {
#if SOFTCEN_DEBUG
            Debug.Log("LevelResources SKIP isGMNull: " + isGMNull);
#endif
            return;
        }

        if (!levelChangeDlg.gameObject.activeSelf)
        {
            levelChangeDlg.Show();
        }
        SetBusinessManActive(true);
    }

    public void LoadLevelResources()
    {
        m_state = State.IDLE;
#if SOFTCEN_DEBUG
        Debug.Log("LoadLevelResources currentChapter: " + GameManager.Instance.playerData.CurrentChapter
            + ", selectedChapter: " + GameManager.Instance.selectedChapter);
#endif
        if (GameManager.Instance.selectedChapter == 0 && GameManager.Instance.playerData.CurrentChapter > 0)
        {
            GameManager.Instance.selectedChapter = GameManager.Instance.playerData.CurrentChapter;
        }
        int currentChapter = GameManager.Instance.selectedChapter;

        if (currentChapter == 0)
            return;

        string resourceName = "Airfield1";
        switch(currentChapter)
        {
            case 1:
                resourceName = "Airfield1";
                break;
            case 2:
                resourceName = "Airfield2";
                break;
            case 3:
                resourceName = "Airfield3";
                break;
            case 4:
                resourceName = "Airfield4";
                break;
            case 5:
                resourceName = "Airfield5";
                break;
            case 6:
                resourceName = "Airfield6";
                break;
            case 7:
                resourceName = "Airfield7";
                break;
            case 8:
                resourceName = "Airfield8";
                break;
            default:
                resourceName = "Airfield1";
                break;

        }
#if SOFTCEN_DEBUG
        float deltaTime = Time.realtimeSinceStartup - usedTime;
        Debug.Log("LoadLevelResources: resourcename: " + resourceName + ", time: " + deltaTime.ToString());
#endif
        if (currentLevelManager != null && currentLevelManager.gameObject.name == resourceName)
        {
            // Level already loaded
            SetBusinessManActive(true);
            currentLevelManager.CheckItems();
            return;
        }


        currentLevelName = resourceName;

        if (currentLevelManager != null && currentLevelManager.gameObject.name != currentLevelName )
        {
            Destroy(currentLevelManager.gameObject);
            currentLevelManager = null;
        }

        m_state = State.START_LOAD;
        /*GameObject goLvl = Instantiate(Resources.Load(currentLevelName, typeof(GameObject))) as GameObject;
        if (goLvl != null)
        {
            currentLevelManager = goLvl.GetComponent<LevelManager>();
            SetBusinessManActive(true);
            goLvl.name = currentLevelName;
            goLvl.SetActive(true);
        }*/
    }

    [SkipRename]
    public void LevelChangeStep2()
    {
#if SOFTCEN_DEBUG
        float deltaTime = Time.realtimeSinceStartup - usedTime;
        Debug.Log("LevelChangeStep2 start deltaTime: " + deltaTime.ToString());
#endif
        HomeManager.Instance.CloseAllDialogs();
        //txtTopPanelLevel.text = "LEVEL: " + GameManager.Instance.playerData.Level.ToString();
        //txtTopPanelChapterName.text = BonusManager.Instance.GetCurrentChapterName();
        LoadLevelResources();

        if (m_state == State.IDLE)
        {
            CheckChapterComplete();
            levelChangeDlg.Button_Close();
        }

#if SOFTCEN_DEBUG
        deltaTime = Time.realtimeSinceStartup - usedTime;
        Debug.Log("LevelChangeStep2 end deltaTime: " + deltaTime.ToString());
#endif
    }

    [SkipRename]
    public void LevelChangeDone()
    {
#if SOFTCEN_DEBUG
        Debug.Log("LevelChangeDone");
#endif
        //goLevelChange.SetActive(false);
    }

    /*
public bool IsPhaseCompleted()
{
    if (currentLevelManager != null)
    {
        int currentChapter = GameManager.Instance.playerData.CurrentChapter;

        if (currentChapter < Chapters.MaxChapter && currentLevelManager.IsPhasesCompleted())
        {
            return true;
        }
    }
    return false;
}*/
    private void CheckChapterComplete()
    {
        if (GameManager.Instance.selectedChapter < GameManager.Instance.playerData.CurrentChapter)
        {
            goGoCurrentButton.SetActive(true);
        }
        else
        {
            goGoCurrentButton.SetActive(false);
        }

        if (BonusManager.Instance.IsChapterCompleted(GameManager.Instance.selectedChapter))
        {
            goMainMapButton.SetActive(true);
        }
        else
        {
            goMainMapButton.SetActive(false);
        }
    }

    public void SetBusinessManActive(bool state)
    {
#if SOFTCEN_DEBUG
        Debug.Log("SetBusinessManActive state: " + state);
#endif
        if (currentLevelManager == null || currentLevelManager.businessmanControl == null)
            return;

        if (state)
        {
            if (!currentLevelManager.businessmanControl.gameObject.activeSelf)
                currentLevelManager.businessmanControl.gameObject.SetActive(true);
            if (GameManager.Instance.selectedChapter < GameManager.Instance.playerData.CurrentChapter)
            {
                // With max phase
                currentLevelManager.managerPositions.SetPositionAndCamFlow(currentLevelManager.businessmanControl, camFlow, 1000);
            }
            else
            {
                currentLevelManager.managerPositions.SetPositionAndCamFlow(currentLevelManager.businessmanControl, camFlow, GameManager.Instance.playerData.CurrentPhase);
            }
        }
        else
        {
            if (currentLevelManager.businessmanControl.gameObject.activeSelf)
                currentLevelManager.businessmanControl.gameObject.SetActive(false);
        }
    }

    [SkipRename]
    public void GoCurrentChapter()
    {
        GameManager.Instance.selectedChapter = GameManager.Instance.playerData.CurrentChapter;
        GameManager.Instance.playerData.CallOnLevelChanged();
    }

    IEnumerator LoadLevel()
    {
        //Debug.Log("Start LoadLevel");
        ResourceRequest request = Resources.LoadAsync(currentLevelName, typeof(GameObject));
        yield return request;
        //Debug.Log("Finishing LoadLevel");
        GameObject goLvl = Instantiate(request.asset as GameObject) as GameObject;
        if (goLvl != null)
        {
            currentLevelManager = goLvl.GetComponent<LevelManager>();
            SetBusinessManActive(true);
            goLvl.name = currentLevelName;
            goLvl.SetActive(true);
        }
        //Debug.Log("Done LoadLevel");
        m_state = State.LOAD_READY;
    }
}
