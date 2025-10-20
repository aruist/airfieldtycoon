
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_PlayerData : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
        PlayerData data = (PlayerData)obj;
        writer.Write(data.version);
        writer.Write(data._adfree);
        writer.Write(data._moneyearned);
        writer.Write(data._moneyused);

        writer.Write(data.IdleBonusCount.Count);
        for (int i = 0; i < data.IdleBonusCount.Count; i++)
        {
            writer.Write(data.IdleBonusCount[i].id);
            writer.Write(data.IdleBonusCount[i]._count);
        }
        writer.Write(data.TapBonusCount.Count);
        for (int i = 0; i < data.TapBonusCount.Count; i++)
        {
            writer.Write(data.TapBonusCount[i].id);
            writer.Write(data.TapBonusCount[i]._count);
        }
        writer.Write(data.UpgradeCount.Count);
        for (int i = 0; i < data.UpgradeCount.Count; i++)
        {
            writer.Write(data.UpgradeCount[i].id);
            writer.Write(data.UpgradeCount[i]._count);
        }

        writer.Write(data._diamonds);
        writer.Write(data.PowerUpList.Count);
        for (int i = 0; i < data.PowerUpList.Count; i++)
        {
            writer.Write(data.PowerUpList[i]);
        }
        writer.Write(data._gigCoins);
        writer.Write(data._famous);
        writer.Write(data._level);

        writer.Write(data._sublevel);

        writer.Write(data.askRateLater);
        writer.Write(data.likeAsked);
        writer.Write(data.playerLike);
        writer.Write(data.powerSave);
        // Version 12:
        writer.Write(data.m_autotapOwned);
        // Mission:
        writer.Write((int)data.CurrentMission.type);
        writer.Write(data.CurrentMission.TargetCount);
        writer.Write(data.CurrentMission.CurrentCount);
        writer.Write((int)data.CurrentMission.currentFishType);
        writer.Write(data.CurrentMission.BonusCoins);
        // Version 13:
        writer.Write(data.AudioVol);
        writer.Write(data.MusicVol);
        // Version 14:
        writer.Write(data.CurrentChapter);
        writer.Write(data.m_CurrentPhase);
        // Version 16:
        writer.Write(data.MissionCounter);
        // Version 17:
        writer.Write(data._diamondsPurchased);
        writer.Write(data._diamondsSpend);
        data.SetSaved();
        // Version 18:
        writer.Write(data._PrestigeCount);
        writer.Write(data._PrestigeLevel);
        // Version 19:
        writer.Write(data._Score);
        // Verson 20:
        writer.Write(data.LoggedOut);
        // Version 21:
        writer.Write(data.CloudEnabled);
        // Version 22:
        //Debug.Log ("Save IlmainenArkkuAvaamisAika: data.IlmainenArkkuAvaamisAika");
        writer.Write (data.IlmainenArkkuAvaamisAika);
        // Version 23:
        writer.Write (data.CurrentMission.Laskettu);
    }

    public override object Read(ES2Reader reader)
	{
        PlayerData data = new PlayerData();
        Read(reader, data);
        return data;
    }

    public override void Read(ES2Reader reader, object c)
	{
		PlayerData data = (PlayerData)c;
        // Add your reader.Read calls here to read the data into the object.
        int version = reader.Read<int>();
        data._adfree = reader.Read<int>();
        data._moneyearned = reader.Read<double>();
        data._moneyused = reader.Read<double>();

        if (data.IdleBonusCount == null)
            data.IdleBonusCount = new List<ItemCount>();
        else
        {
            for (int i = 0; i < data.IdleBonusCount.Count; i++)
                data.IdleBonusCount[i] = null;

            data.IdleBonusCount.Clear();
        }

        int count = reader.Read<int>();
        for (int i = 0; i < count; i++)
        {
            ItemCount ic = new ItemCount();
            ic.id = reader.Read<int>();
            ic._count = reader.Read<int>();
            data.IdleBonusCount.Add(ic);
        }

        if (data.TapBonusCount == null)
            data.TapBonusCount = new List<ItemCount>();
        else
        {
            for (int i = 0; i < data.TapBonusCount.Count; i++)
                data.TapBonusCount[i] = null;

            data.TapBonusCount.Clear();
        }

        count = reader.Read<int>();
        for (int i = 0; i < count; i++)
        {
            ItemCount ic = new ItemCount();
            ic.id = reader.Read<int>();
            ic._count = reader.Read<int>();
            data.TapBonusCount.Add(ic);
        }

        if (data.UpgradeCount == null)
            data.UpgradeCount = new List<ItemCount>();
        else
        {
            for (int i = 0; i < data.UpgradeCount.Count; i++)
                data.UpgradeCount[i] = null;

            data.UpgradeCount.Clear();
        }

        count = reader.Read<int>();
        for (int i = 0; i < count; i++)
        {
            ItemCount ic = new ItemCount();
            ic.id = reader.Read<int>();
            ic._count = reader.Read<int>();
            data.UpgradeCount.Add(ic);
        }

        data._diamonds = reader.Read<int>();
        // PowerUpList
        data.ResetPowerUps();
        count = reader.Read<int>();
        for (int i = 0; i < count; i++)
        {
            int id = reader.Read<int>();
            if (!data.PowerUpList.Contains(id))
            {
                data.PowerUpList.Add(id);
            }
        }

        data._gigCoins = reader.Read<long>();
        data._famous = reader.Read<int>();
        data._level = reader.Read<int>();
        data._sublevel = reader.Read<int>();

        data.askRateLater = reader.Read<bool>();
        data.likeAsked = reader.Read<bool>();
        data.playerLike = reader.Read<bool>();
        data.powerSave = reader.Read<bool>();

        data.m_autotapOwned = reader.Read<bool>();
        int type, targetcount, currentcount, fishtype;
        double bonus = 0;
        type = reader.Read<int>();
        targetcount = reader.Read<int>();
        currentcount = reader.Read<int>();
        fishtype = reader.Read<int>();
        bonus = reader.Read<double>();

        data.AudioVol = reader.Read<float>();
        data.MusicVol = reader.Read<float>();

        data.CurrentChapter = reader.Read<int>();
        data.m_CurrentPhase = reader.Read<int>();

        data.MissionCounter = reader.Read<int>();
        if (version >= 17)
        {
            data._diamondsPurchased = reader.Read<int>();
            data._diamondsSpend = reader.Read<int>();
        }
        else
        {
            data.DiamondsPurchased = 0;
            data.DiamondsSpend = 0;
        }

        if (version >= 18)
        {
            data._PrestigeCount = reader.Read<int>();
            data._PrestigeLevel = reader.Read<int>();
        }
        else
        {
            data.PrestigeCount = 0;
            data.PrestigeLevel = 0;
        }

        if (version >= 19)
        {
            data._Score = reader.Read<long>();
        }
        else
        {
            data.Score = 0;
        }

        if (version >= 20)
        {
            data.LoggedOut = reader.Read<bool>();
        }
        else
        {
            data.LoggedOut = false;
        }

        if (version >= 21)
        {
            data.CloudEnabled = reader.Read<bool>();
        }
        else
        {
            data.CloudEnabled = true;
        }
        data.CalculateUpdates();
        if (version >= 22) {
            data.IlmainenArkkuAvaamisAika = reader.Read<long> ();
            //Debug.Log ("Load IlmainenArkkuAvaamisAika: data.IlmainenArkkuAvaamisAika");
        } else {
            data.IlmainenArkkuAvaamisAika = ArKKuTyyPPi.uusiAika (ArKKuTyyPPi.tyyppi.ILMAINEN);
        }
        bool laskettu = false;
        if (version >= 23) {
            laskettu = reader.Read<bool> ();
        }
        if (data.CurrentMission == null)
        {
            data.CurrentMission = new MissionItem();
        }
        data.CurrentMission.SetMission((MissionTypes.type)type, targetcount, currentcount, (FishTypes.type)fishtype, bonus, laskettu);

    }

    /* ! Don't modify anything below this line ! */
    public ES2UserType_PlayerData():base(typeof(PlayerData)){}
}
