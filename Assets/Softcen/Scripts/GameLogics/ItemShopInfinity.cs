using UnityEngine;
using System.Collections;
using OneP.InfinityScrollView;

public class ItemShopInfinity : InfinityBaseItem
{
    public ItemShopObject itemShopObject;
    public BonusTypes.Type bonusType;
    private BonusManager bonusManager;

    void Awake()
    {
        bonusManager = BonusManager.Instance;
    }

    public override void Reload(InfinityScrollView _infinity, int _index)
    {
        base.Reload(_infinity, _index);
        if (itemShopObject != null && bonusManager != null)
        {
            if (bonusType == BonusTypes.Type.Idle)
            {
                int j = Mathf.Min(bonusManager.idleItemList.Count - 1, _index);
                itemShopObject.txtTitle.text = bonusManager.idleItemList[j].titleName;
                itemShopObject.itemImage.sprite = bonusManager.idleItemList[j].itemSprite;
                itemShopObject.itemObj = bonusManager.idleItemList[j];
                if (_index == 0)
                    Debug.Log("index is 0");
                itemShopObject.CheckActiveStatus();
            }
            else if (bonusType == BonusTypes.Type.Tap)
            {
                int j = Mathf.Min(bonusManager.tapItemList.Count - 1, _index);
                itemShopObject.txtTitle.text = bonusManager.tapItemList[j].titleName;
                itemShopObject.itemImage.sprite = bonusManager.tapItemList[j].itemSprite;
                itemShopObject.itemObj = bonusManager.tapItemList[j];
                itemShopObject.CheckActiveStatus();
            }
            else if (bonusType == BonusTypes.Type.Level)
            {
                int j = Mathf.Min(bonusManager.levelItemList.Count - 1, _index);
                int index = 0;
                for (int i=0; i < j; i++)
                {
                    if (bonusManager.levelItemList[index].ActivePhase < 0)
                    {
                        index += 2;
                    }
                    else
                    {
                        index++;
                    }
                }
                if (bonusManager.levelItemList[index].ActivePhase < 0)
                {
                    index ++;
                }

                itemShopObject.txtTitle.text = bonusManager.levelItemList[index].titleName;
                itemShopObject.itemImage.sprite = bonusManager.levelItemList[index].itemSprite;
                itemShopObject.itemObj = bonusManager.levelItemList[index];
                itemShopObject.CheckActiveStatus();
            }
        }
    }

    public void OnClick()
    {
        //Sample1.Instance.OnClickItem(Index + 1);
    }

}
