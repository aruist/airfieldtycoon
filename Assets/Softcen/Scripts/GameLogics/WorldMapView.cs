using UnityEngine;
using UnityEngine.UI;

public class WorldMapView : MonoBehaviour {
    public CameraFlow camFlow;
    public CameraFlowMap camFlowMap;
    public GameObject goBuyPanel;
    public Text txtDiamondPrice;
    public Text txtCoinPrice;

    public ChapterMap[] chapters;
    public Color colorActive;
    public Color colorInactive;
    public Color colorDone;

    public float deactivePosZ = -13f;
    public float sphereActivePosZ = 3.3f;
    public float lighthouseActivePosZ = 0f;

    public double currentCoinPrice;
    public int currentDiamondPrice;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            gameObject.SetActive(false);
            return;
        }
        SetActiveChapter();
    }

    public void SetActiveChapter()
    {
        int currentId = GameManager.Instance.playerData.CurrentChapter;

        if (currentId == 0)
            currentId = 1;

#if SOFTCEN_DEBUG
        Debug.Log("SetActive chapter: " + currentId);
#endif

        for (int i=0; i < chapters.Length; i++)
        {
            if (chapters[i] != null)
            {
                if ((int)chapters[i].Id == currentId)
                {
                    camFlowMap.trTarget = chapters[i].transform;
                    chapters[i].SetSelected(ChapterMap.Mode.Selected, colorActive);
                    currentCoinPrice = BonusManager.Instance.GetLevelCoinPrice((int)chapters[i].itemId);
                    currentDiamondPrice = BonusManager.Instance.GetLevelDiamondPrice((int)chapters[i].itemId);
                    txtDiamondPrice.text = currentDiamondPrice.ToString();
                    txtCoinPrice.text = currentCoinPrice.ToString();
                }
                else
                {
                    if (currentId > (int)chapters[i].Id)
                    {
                        chapters[i].SetSelected(ChapterMap.Mode.Done, colorDone);
                    }
                    else
                    {
                        chapters[i].SetSelected(ChapterMap.Mode.Inactive, colorInactive);
                    }
                }

            }
        }


        if (GameManager.Instance.IsPhaceCompleted)
        {
            goBuyPanel.SetActive(true);
        }
        else
        {
            goBuyPanel.SetActive(false);
        }

    }
}
