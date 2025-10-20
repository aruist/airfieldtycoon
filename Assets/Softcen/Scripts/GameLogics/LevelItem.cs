using UnityEngine;
using System.Collections;

public class LevelItem : MonoBehaviour {
    public Item.Identifications Id;
    public int activeCount = 1;

    public float enableY = 0f;
    public float disableY = -50f;
    //public bool levelItemEnabled = false;

    public Object itemObj;
    public GameObject goCamPath = null;

    private Vector3 m_pos;
    // Use this for initialization
    //public int activePhase;

    void OnDisable()
    {
        if (goCamPath != null)
        {
            goCamPath.SetActive(false);
        }
    }

    public void CheckItem()
    {
        bool isActive = false;
        if (itemObj == null)
        {
            itemObj = BonusManager.Instance.GetItemObject(Id);
        }
        if (itemObj is UpgradeItemLevel)
        {
            UpgradeItemLevel uil = (UpgradeItemLevel)itemObj;
            //activePhase = uil.ActivePhase;
            isActive = uil.isItemActive();
            //int currentChapter = GameManager.Instance.playerData.CurrentChapter;
        }
        else if (itemObj is UpgradeItemIdle)
        {
            UpgradeItemIdle obj = (UpgradeItemIdle)itemObj;
            if (obj.ownedCount >= activeCount)
                isActive = true;
        }
        else if (itemObj is UpgradeItemTap)
        {
            UpgradeItemTap obj = (UpgradeItemTap)itemObj;
            if (obj.ownedCount >= activeCount)
                isActive = true;
        }
        SetState(isActive);
    }

    public bool IsLevelItemInActive()
    {
        if (itemObj == null)
        {
            itemObj = BonusManager.Instance.GetItemObject(Id);
        }
        if (itemObj  != null)
        {
            if (itemObj is UpgradeItemLevel)
            {
                UpgradeItemLevel uil = (UpgradeItemLevel)itemObj;
                return !uil.isItemActive();
            }
        }
        return false;
    }

    public void CheckIdleItem()
    { /*
        if (levelItemEnabled)
            return;
        if (bonusType == BonusTypes.Type.Idle)
        {
            int count = GameManager.Instance.playerData.GetIdleBonusCount(itemId);
            if (count >= enableCountOrLvl)
            {
                SetState(true);
            }
            else
            {
                SetState(false);
            }
        }
        */
    }
    public void CheckTapItem()
    {
        /*
        if (levelItemEnabled)
            return;
        if (bonusType == BonusTypes.Type.Tap)
        {
            int count = GameManager.Instance.playerData.GetTapBonusCount(itemId);
            if (count >= enableCountOrLvl)
            {
                SetState(true);
            }
            else
            {
                SetState(false);
            }
        }*/
    }

    public bool CheckLevelItem(int count)
    {
        /*
        if (levelItemEnabled)
            return true;

        if (bonusType == BonusTypes.Type.Level)
        {
            if (count >= enableCountOrLvl)
            {
                SetState(true);
                return true;
            }
            else
            {
                SetState(false);
            }
        }*/
        return false;
    }

    private void SetState(bool state)
    {
        //levelItemEnabled = state;
        m_pos = transform.position;
        if (state)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            m_pos.y = enableY;
            if (goCamPath != null)
                goCamPath.SetActive(true);
        }
        else
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);

            m_pos.y = disableY;
            if (goCamPath != null)
                goCamPath.SetActive(false);
        }
        transform.position = m_pos;
    }
}
