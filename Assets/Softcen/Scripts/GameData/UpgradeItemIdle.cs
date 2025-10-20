using UnityEngine;
using System.Collections;

public class UpgradeItemIdle : UpgradeItem
{
    public override int ownedCount
    {
        get
        {
            if (GameManager.Instance != null)
            {
                return GameManager.Instance.playerData.GetIdleBonusCount(ItemId);
            }
            return base.ownedCount;
        }
    }

    public override double GetCoinPrice()
    {
        //int adjustedId = ItemId - Item.IdleIdStart;
        return BonusManager.Instance.GetIdleItemCoinPrice(basePrice, ownedCount);
        //return BonusManager.Instance.GetIdleItemCoinPrice(adjustedId, ownedCount);
    }

    public void BuyItemWithCoins()
    {
        GameManager gm = GameManager.Instance;
        double price = GetCoinPrice();
#if SOFTCEN_DEBUG
        if (gm.dev_SkipMoney)
            gm.playerData.IncMoney(price);
#endif
        gm.playerData.DecMoney(price);
        gm.playerData.AddIdleBonus(ItemId);
        gm.Save();
    }

    public void BuyItemWithDiamonds()
    {
        GameManager gm = GameManager.Instance;
        int price = GetDiamondPrice();
        if (price <= gm.playerData.Diamonds)
        {
            gm.playerData.ChangeDiamonds(-1 * price);
            gm.playerData.AddIdleBonus(ItemId);
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Diamond Use", "Idle", 1);
        }
    }

    public override bool isItemActive()
    {
        /*if ((int)chapterId == GameManager.Instance.playerData.CurrentChapter)
        {
            if (ActivePhase < GameManager.Instance.playerData.CurrentPhase)
            {
                return true;
            }
        }
        else
        {
            if ((int)chapterId < GameManager.Instance.playerData.CurrentChapter)
                return true;
        }*/
        return false;
    }

}
