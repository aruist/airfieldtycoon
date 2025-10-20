#if USE_SCGIFT
using UnityEngine;
using System;

public class SCGift : MonoBehaviour {
    public static SCGift get;
    const string dataFileName = "sc19xf.obj";
    const string fileenc = "?encrypt=true&password=sce33sdr79khhzf2e";

    private SCGiftItem item;
    private string[] timeStr;

    void Awake()
    {
        if (get == null)
        {
            get = this;
            DontDestroyOnLoad(gameObject);
            Load();
            return;
        }
        Destroy(gameObject);
    }

	void Start () {
        timeStr = new string[61];
        for (int i=0; i <= 60; i++)
        {
            timeStr[i] = i.ToString("D2");
        }
        //DateTime dateTime = DateTime.UtcNow;
        //Debug.Log("DateTime UtcNow: " + dateTime.ToString() + ", Ticks: " + dateTime.Ticks);
        //TimeSpan timeSpan = TimeSpan.FromTicks(dateTime.Ticks);
	}

    public SCGiftItem GetCurrentGift()
    {
        return item;
    }

    //private int m_tempcounter = 0;
    public bool IsGiftReady()
    {
        long currentTicks = DateTime.UtcNow.Ticks;
        TimeSpan timeSpan = TimeSpan.FromTicks(item.duration - (currentTicks - item.startTime));
        if (timeSpan.TotalSeconds <= 0)
            return true;
        return false;
    }

    public string GetRemainTimeStr()
    {
        string retTime = "";
        if (item != null)
        {
            long currentTicks = DateTime.UtcNow.Ticks;
            TimeSpan timeSpan = TimeSpan.FromTicks(item.duration - (currentTicks - item.startTime));
            if (timeSpan.TotalSeconds <= 0)
            {
                retTime = "0 s";
            }
            else if (timeSpan.Hours > 60)
            {
                retTime = "> 60 hours";
            }
            else
            {
                if (timeSpan.TotalSeconds <= 60)
                {
                    retTime = timeStr[(int)timeSpan.TotalSeconds] + " s";
                }
                else if (timeSpan.TotalMinutes <= 60)
                {
                    retTime = timeStr[timeSpan.Minutes] + "m " + timeStr[timeSpan.Seconds] + "s";
                }
                else
                {
                    retTime = timeStr[timeSpan.Hours] + "h " + timeStr[timeSpan.Minutes] + "m";
                }
            }
        }
        return retTime;
    }

    public void CreateNewGift(int id, long durationMinutes)
    {
        item = new SCGiftItem();
        item.id = id;
        item.duration = durationMinutes * TimeSpan.TicksPerMinute;
        item.startTime = DateTime.UtcNow.Ticks;
        Save();
    }
    public void SetCurrentGift(SCGiftItem giftItem)
    {
        item = giftItem;
        giftItem.startTime = DateTime.UtcNow.Ticks;
        Save();
    }
	
    private void Load()
    {
#if SOFTCEN_DEBUG
        Debug.Log("SCGiftItem Load start");
#endif
        if (ES2.Exists(dataFileName))
        {
            try
            {
                item = ES2.Load<SCGiftItem>(dataFileName + fileenc);
#if SOFTCEN_DEBUG
                Debug.Log("SCGiftItem load id: " + item.id
                    + ", startTime: " + item.startTime
                    + ", duration: " + item.duration
                    );
#endif
            }
            catch
            {
#if SOFTCEN_DEBUG
				Debug.Log("<color=red>SCGiftItem load failed</color>");
#endif
                item = null;
            }
        }
        else
        {
#if SOFTCEN_DEBUG
			Debug.Log("No SCGift found!");
#endif
            item = null;
        }

    }

    private void Save()
    {
#if SOFTCEN_DEBUG
        Debug.Log("SCGiftItem Save start");
#endif
        if (item != null)
        {
            try
            {
                ES2.Save(item, dataFileName + fileenc);
            }
            catch
            {
#if SOFTCEN_DEBUG
                Debug.Log("<color=red>SCGiftItem save failed</color>");
#endif
            }

        }
    }

    public void ResetGift()
    {
        try
        {
            if (ES2.Exists(dataFileName))
            {
                ES2.Delete(dataFileName);
                item = null;
            }
        }
        catch
        {
        }
    }

    public void CompareCloudGiftItem(SCGiftItem cloudGift)
    {
        if (cloudGift == null)
            return;
        if (item == null)
        {
            item = new SCGiftItem();
        }
        if (cloudGift.id > item.id)
        {
            // Cloud has newer giftitem
            item.id = cloudGift.id;
            item.startTime = cloudGift.startTime;
            item.duration = cloudGift.duration;
            Save();
        }
        else if (cloudGift.id == item.id)
        {
            if (cloudGift.startTime > item.startTime)
            {
                item.startTime = cloudGift.startTime;
                item.duration = cloudGift.duration;
#if SOFTCEN_DEBUG
                Debug.Log("Cloud Gift Updated from cloud: " + item.startTime + ", dur: " + item.duration);
#endif
                Save();
            }
            else if (cloudGift.duration > item.duration)
            {
                item.duration = cloudGift.duration;
#if SOFTCEN_DEBUG
                Debug.Log("Cloud Gift Updated from cloud: " + item.startTime + ", dur: " + item.duration);
#endif
                Save();
            }
        }
    }
}

#endif
