using UnityEngine;
using System.Collections;

public class UpgradeItemLevel : UpgradeItem {
    public override int ownedCount
    {
        get
        {
            if (GameManager.Instance != null)
            {
                return GameManager.Instance.playerData.GetUpgradeCount(ItemId);
            }
            return base.ownedCount;
        }
    }

    public override double GetCoinPrice()
    {
        return BonusManager.Instance.GetLevelCoinPrice(ItemId);
    }
    public override int GetDiamondPrice()
    {
        return BonusManager.Instance.GetLevelDiamondPrice(ItemId);
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
        gm.playerData.IncLevel();
        gm.playerData.CurrentPhase++;
    }

    public void BuyItemWithDiamonds()
    {
        GameManager gm = GameManager.Instance;
        int price = GetDiamondPrice();
        if (price <= gm.playerData.Diamonds)
        {
            gm.playerData.ChangeDiamonds(-1 * price);
            gm.playerData.IncLevel();
            gm.playerData.CurrentPhase++;
            gm.Save();
            SCAnalytics.LogEvent(GameConsts.AnalyticsName, "Diamond Use", "Level", 1);
        }
    }

    public override bool isItemActive()
    {
        bool retVal = false;
        if ((int)chapterId == GameManager.Instance.playerData.CurrentChapter)
        {
            if (ActivePhase <= GameManager.Instance.playerData.CurrentPhase)
            {
                retVal = true;
            }
        }
        else
        {
            if ((int)chapterId < GameManager.Instance.playerData.CurrentChapter)
                retVal = true;
        }
        return retVal;
    }

}
