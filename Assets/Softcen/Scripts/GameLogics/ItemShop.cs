using UnityEngine;
using UnityEngine.UI;
using OneP.InfinityScrollView;
using TMPro;

public class ItemShop : MonoBehaviour {
    public bool useInfinity = false;
    public InfinityScrollView infScrollView;

    public BonusTypes.Type bonusType;
    public Transform trContent;
    public GameObject goShopItemHeader;
    public GameObject goShopItemObject;
    public TextMeshProUGUI txtSubTitle;
    public GameObject goPrestigeBounus;
    //private BonusManager bonusManager;

    // Use this for initialization
    void Start () {
        BonusManager bonusManager = BonusManager.Instance;
        if (bonusManager == null)
        {
#if SOFTCEN_DEBUG
            Debug.LogError("bonusmanager is null");
#endif
            return;
        }
        if (!useInfinity)
        {
            GameObject go;
            for (int i = 0; i < bonusManager.chapters.chapterInfo.Length; i++)
            {
                // Add Header
                go = (GameObject)Instantiate(goShopItemHeader);
                go.transform.SetParent(trContent, false);
                go.GetComponent<RectTransform>().SetAsLastSibling();
                ItemShopHeader sih = go.GetComponent<ItemShopHeader>();
                sih.txtHeader.text = bonusManager.chapters.chapterInfo[i].ChapterName;
                sih.txtSubHeader.text = bonusManager.chapters.chapterInfo[i].ChapterDescription;
                sih.chapterId = (int)bonusManager.chapters.chapterInfo[i].Id;
                // Add Items:
                AddLevelItems(go, i);
                AddIdleItems(go, i);
                AddTapItems(go, i);
            }
        }
        else
        {
            if (infScrollView == null)
                return;
            infScrollView.list_skip_Index.Clear();
            infScrollView.list_skip_Object.Clear();
            int totalItems = 0;
            //int headerIndex = 0;
            GameObject go;
            for (int i = 0; i < bonusManager.chapters.chapterInfo.Length; i++)
            {
                // Add Header
                go = (GameObject)Instantiate(goShopItemHeader);
                go.transform.SetParent(this.transform, false);
                //go.GetComponent<RectTransform>().SetAsLastSibling();
                ItemShopHeader sih = go.GetComponent<ItemShopHeader>();
                sih.txtHeader.text = bonusManager.chapters.chapterInfo[i].ChapterName;
                sih.txtSubHeader.text = bonusManager.chapters.chapterInfo[i].ChapterDescription;
                sih.chapterId = (int)bonusManager.chapters.chapterInfo[i].Id;

                infScrollView.list_skip_Index.Add(totalItems);
                infScrollView.list_skip_Object.Add(go);
                // Items:
                totalItems += ItemsCount(i);
            }
            // Add one empty
            /*go = (GameObject)Instantiate(goShopItemHeader);
            go.transform.SetParent(this.transform, false);
            go.SetActive(false);
            infScrollView.list_skip_Index.Add(totalItems);
            infScrollView.list_skip_Object.Add(go);*/

            Debug.Log("infScrollView.Setup: " + totalItems);
            infScrollView.Setup(totalItems);
        }
    }

    void OnDisable()
    {
        PlayerData.OnBonusesChanged -= PlayerData_OnBonusesChanged;
    }
    void OnEnable()
    {
        if (GameManager.Instance != null && goPrestigeBounus != null)
        {
            if (GameManager.Instance.playerData.PrestigeLevel > 0)
            {
                goPrestigeBounus.SetActive(true);
            }
            else
            {
                goPrestigeBounus.SetActive(false);
            }
        }
        UpdtateSubTitle();
        PlayerData.OnBonusesChanged += PlayerData_OnBonusesChanged;
    }

    private void PlayerData_OnBonusesChanged()
    {
        UpdtateSubTitle();
    }

    public void UpdtateSubTitle()
    {
        if (bonusType == BonusTypes.Type.Idle)
        {
            txtSubTitle.text = "Total bonus: " + NumToStr.GetNumStr(GameManager.Instance.playerData.GetIdleValue()) + " / sec";
        }
        else if (bonusType == BonusTypes.Type.Tap)
        {
            txtSubTitle.text = "Total bonus: " + NumToStr.GetNumStr(GameManager.Instance.playerData.GetTapValue()) + " / tap";
        }
        else if (bonusType == BonusTypes.Type.Level)
        {
            txtSubTitle.text = "Current level " + GameManager.Instance.playerData.Level.ToString();
        }
    }

    private int ItemsCount(int chapterId)
    {
        int count = 0;
        BonusManager bonusManager = BonusManager.Instance;
        if (bonusType == BonusTypes.Type.Level)
        {
            for (int j = 0; j < bonusManager.levelItemList.Count; j++)
            {
                if (bonusManager.chapters.chapterInfo[chapterId].Id == bonusManager.levelItemList[j].chapterId && bonusManager.levelItemList[j].ActivePhase >= 0)
                {
                    count++;
                }
            }
        }
        else if (bonusType == BonusTypes.Type.Idle)
        {
            for (int j = 0; j < bonusManager.idleItemList.Count; j++)
            {
                if (bonusManager.chapters.chapterInfo[chapterId].Id == bonusManager.idleItemList[j].chapterId)
                {
                    count++;
                }
            }
        }
        else if (bonusType == BonusTypes.Type.Tap)
        {
            for (int j = 0; j < bonusManager.tapItemList.Count; j++)
            {
                if (bonusManager.chapters.chapterInfo[chapterId].Id == bonusManager.tapItemList[j].chapterId)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void AddLevelItems(GameObject go, int i)
    {
        BonusManager bonusManager = BonusManager.Instance;
        if (bonusType == BonusTypes.Type.Level)
        {
            for (int j = 0; j < bonusManager.levelItemList.Count; j++)
            {
                if (bonusManager.chapters.chapterInfo[i].Id == bonusManager.levelItemList[j].chapterId && bonusManager.levelItemList[j].ActivePhase >= 0)
                {
                    go = (GameObject)Instantiate(goShopItemObject);
                    go.transform.SetParent(trContent, false);
                    go.GetComponent<RectTransform>().SetAsLastSibling();
                    ItemShopObject iso = go.GetComponent<ItemShopObject>();
                    iso.txtTitle.text = bonusManager.levelItemList[j].titleName;
                    iso.itemImage.sprite = bonusManager.levelItemList[j].itemSprite;
                    iso.itemObj = bonusManager.levelItemList[j];
                }
            }
        }
    }

    private void AddIdleItems(GameObject go, int i)
    {
        BonusManager bonusManager = BonusManager.Instance;
        if (bonusType == BonusTypes.Type.Idle)
        {
            for (int j = 0; j < bonusManager.idleItemList.Count; j++)
            {
                if (bonusManager.chapters.chapterInfo[i].Id == bonusManager.idleItemList[j].chapterId)
                {
                    go = (GameObject)Instantiate(goShopItemObject);
                    go.transform.SetParent(trContent, false);
                    go.GetComponent<RectTransform>().SetAsLastSibling();
                    ItemShopObject iso = go.GetComponent<ItemShopObject>();
                    iso.txtTitle.text = bonusManager.idleItemList[j].titleName;
                    iso.itemImage.sprite = bonusManager.idleItemList[j].itemSprite;
                    iso.itemObj = bonusManager.idleItemList[j];
                }
            }
        }
    }

    private void AddTapItems(GameObject go, int i)
    {
        BonusManager bonusManager = BonusManager.Instance;
        if (bonusType == BonusTypes.Type.Tap)
        {
            for (int j = 0; j < bonusManager.tapItemList.Count; j++)
            {
                if (bonusManager.chapters.chapterInfo[i].Id == bonusManager.tapItemList[j].chapterId)
                {
                    go = (GameObject)Instantiate(goShopItemObject);
                    go.transform.SetParent(trContent, false);
                    go.GetComponent<RectTransform>().SetAsLastSibling();
                    ItemShopObject iso = go.GetComponent<ItemShopObject>();
                    iso.txtTitle.text = bonusManager.tapItemList[j].titleName;
                    iso.itemImage.sprite = bonusManager.tapItemList[j].itemSprite;
                    iso.itemObj = bonusManager.tapItemList[j];
                }
            }
        }
    }


}
