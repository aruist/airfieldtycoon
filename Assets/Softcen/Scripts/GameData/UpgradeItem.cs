using UnityEngine;

public class UpgradeItem : MonoBehaviour {
    public Chapters.Id chapterId;
    public Item.Identifications Id;
    public int ActivePhase = 0;
    public int maxCount = 1;
    public virtual int ItemId { get { return (int)Id; } }
    public virtual int ownedCount { get { return 0; } }
    public string titleName;
    public string description;
    public Sprite itemSprite;
    public double basePrice;
    public int baseDiamonds;
    public GameObject goPrefab;

    public Transform previewTransform;

    public virtual double GetCoinPrice()
    {
        return 0;
    }

    public virtual int GetDiamondPrice()
    {
        return baseDiamonds + ownedCount;
    }

    public virtual bool isItemActive()
    {
        return false;
    }

    public virtual int GetAvailableLevel()
    {
        int retVal = 0;
        if (BonusManager.Instance != null)
        {
            retVal = BonusManager.Instance.GetChapterStartLevel((int)chapterId);
            retVal += ActivePhase;
        }
        return retVal;
    }
}
