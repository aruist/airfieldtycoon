using UnityEngine;
using Beebyte.Obfuscator;

public class GameConsts {
    public const string AnalyticsName = "ATC";
    public const int GooglePlayAutoSignDefault = 0;
#if SOFTCEN_DEBUG
    public const float InterstitialShowTime = 360.0f;
#else
    public const float InterstitialShowTime = 420.0f;
#endif
    public const string GameName = "Airfield Tycoon Clicker";
    public static string FilenamePrefix
    {
        [ObfuscateLiterals]
        get
        {
            return "?encrypt=true&password=";
        }
    }
    public static string DataFile
    {
        [ObfuscateLiterals]
        get
        {
            return "atc8hwr.obj";
        }
    }
    public static string FileKey 
    {
        [ObfuscateLiterals]
        get
        {
            return "bge76sdg28khz7tW5g";
        }
    }

    //public const string UnityProjectId = "bae56aa1-142a-4292-a720-4a3ccee391da";
    public const int maxLevel = 38;
	public const double StartupMoney = 10f;
	public const int StartupDiamonds = 10;
    public const int MaxUpgradeCount = 10;

    public class AutoTap
    {
        public const float TryTime = 90;
        public const float MinTime = 60f;
        public const float MaxTime = 180f;
    }

    public const float RateGameTimeout = 600f;
    public const int RateGameMinLevel = 4;

	public class Skenes {
		public const string Home = "AirfieldMain";
		public const string Loader = "Loader";
        public const string Map = "Map";
	}
	public const string gpke3 = "notset";

	public class Reward {
        public const float RewardEventMinTime = 60f;
		public const float RewardEventMaxTime = 180f;
        public const int AddMoreTime = 1;
        public const int PenaltyReward = 2;
    }

    // Password hash
    public const string pGq2Vlmr = "LNl5Gc0isNLg56vHadVA";
    // Salt 16 char
    public const string sZvWe9sJ = "3f1IrBQkM5ujp1lc";
    // VI 16 char
    public const string vs43xKOY = "ccpYI30xb1fdVS73";

    /*public static string AdMob
    {
        get { return M4hVva1c.ZTGjqBkg(xh3lw.L_23sd.lvl2); }
    }*/

#if UNITY_ANDROID
    // GP AdMob Banner:
    public static string amhix2wb
    {
        get { return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl1); }
    }
    // GP Admob Interstitial
    public static string amhix2wi
    {
        get { return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl2); }
    }
    // GP AdMob Reward
    public static string ams3fgsfs
    {
        get { return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.aterg); }
    }
    // Admob GP ID
    public static string addf33gfal
    {
        get { return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl6); }    
    }
#endif
#if UNITY_IOS
    // Admob IOS ID
    public static string addf33gfal
    {
        get { return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl7); }
    }
    // IOS AdMob Banner:
    public static string amhix2wb
    {
        get { return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl3); }
    }
    // IOS Admob Interstitial
    public static string amhix2wi
    {
        get { return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl4); }
    }
    // IOS AdMob Reward
    public static string ams3fgsfs
    {
        get { return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.ateri); }
    }
#endif

    public class Ads {

        public const bool UseChartboost = false;
        public const bool UseAdMob = false;
        public const float AmazonAdsRefresh = 60.0f;
        public const string AmazonAdsAppKey = "";


        // Minimum screen height to show ads
        public const int MinScreenHeight = 320;
        // AdMob Airfiled Tycoon Clicker
#if UNITY_ANDROID

        public static string AdMobInterstitial
        {
            [ObfuscateLiterals]
            get
            {
                string str = "";
                return str;
            }
        }
        public static string AdMobId
        {
            [ObfuscateLiterals]
            get
            {
                string str = "";
                return str;
            }
        }
#elif UNITY_IOS || UNITY_IPHONE

        public const string AdMobInterstitial = "";
        public const string AdMobId = "";
#else
        public const string AdMobInterstitial = "";
        public const string AdMobId = "";
#endif

    }
    public const string gpke2 = "notset";

    public class Analytics {
        // Harbor Tycoon Clicker:
        public const string GPAndroidAnalytics = "^UA-52557140-26^";
        public const string GPIOSAnalytics = "^UA-52557140-27^";
    }
	public class Links
	{

#if UNITY_IOS || UNITY_IPHONE
		public const string MoreGames = "http://appstore.com/softcen";
        public const string RateGame = "itms-apps://itunes.apple.com/app/id1069619421"; // Harbor Tycoon
#endif
#if UNITY_ANDROID && SOFTCEN_AMAZON
		public const string MoreGames = "http://www.amazon.com/gp/mas/dl/android?s=softcen&showAll=1";
        public const string RateGame = "http://www.amazon.com/gp/mas/dl/android?p=com.softcen.harbor.tycoon";
#endif
#if UNITY_ANDROID && !SOFTCEN_AMAZON
        public const string MoreGames = "market://search?q=softcen";
        public const string RateGame = "market://details?id=com.softcen.harbor.tycoon";
#endif
#if UNITY_WP8 || UNITY_STANDALONE
		public const string MoreGames = "";
		public const string RateGame = "";
#endif
	}

	public const string gpke1 = "notset";

	public class LeaderBoards {
        public const string TopScores = "LeaderboardScores";
#if UNITY_IOS
        public const string GID_TopScores = "com.softcen.airfield.tycoon.clicker.lb.top.scores";
#else
        public const string GID_TopScores = "CgkI-6iB1dkDEAIQAQ";
        //public const string GID_TopScores = "LeaderboardScores"; // Voxel buster
#endif
    }

    public class Achievements {
#if UNITY_IOS
        public const string GID_BuyHelicopter = "ach.buy.helicopter";
        public const string GID_BuyPrivateJet = "ach.buy.private.jet";
        public const string GID_BuyFighterZ3 = "com.softcen.airfield.tycoon.clicker.ach.buy.fighter.z3";
        public const string GID_HireUlla = "com.softcen.airfield.tycoon.clicker.ach.hire.ulla";
        public const string GID_HireMary = "com.softcen.airfield.tycoon.clicker.ach.hire.mary";
        public const string GID_FullPrestige = "com.softcen.airfield.tycoon.clicker.ach.full.prestige";
        public const string GID_ThreeTowers = "com.softcen.airfield.tycoon.clicker.ach.three.towers";
#elif UNITY_ANDROID
        public const string GID_BuyHelicopter = "CgkI-6iB1dkDEAIQAg";
        public const string GID_BuyPrivateJet = "CgkI-6iB1dkDEAIQAw";
        public const string GID_BuyFighterZ3 = "CgkI-6iB1dkDEAIQBA";
        public const string GID_HireUlla = "CgkI-6iB1dkDEAIQBQ";
        public const string GID_HireMary = "CgkI-6iB1dkDEAIQBg";
        public const string GID_FullPrestige = "CgkI-6iB1dkDEAIQBw";
        public const string GID_ThreeTowers = "CgkI-6iB1dkDEAIQCA";
#else
        public const string GID_BuyHelicopter = "Buy Helicopter";
        public const string GID_BuyPrivateJet = "Buy Private Jet";
        public const string GID_BuyFighterZ3 = "Buy Fighter Z-3";
        public const string GID_HireUlla = "Hire Air Hostess Ulla";
        public const string GID_HireMary = "Hire Air Hostess Mary";
        public const string GID_FullPrestige = "Make full prestige";
        public const string GID_ThreeTowers = "Build Three Towers";
#endif
    }

    public const string Description1 = "Have you ever dream about starting your own company and becoming an enterpreneur? In Harbor Tycoon Clicker game you will be given the opportunity to realize your dreams.";
    public const string Description2 = "Start from a scratch with a empty harbor, and reach the most impressive harbor which has never been seen. Hire and level up staff, buy ships and environments to your harbor.";
    public const string Description3 = "Tap to create money and earn enough money to invest in your harbors. Open gift case to get impressive clicker bonuses.";

    public class Score
    {
        public const int LevelChange = 2;
        public const int IdleUpgrade = 2;
        public const int TapUpgrade = 2;
        public const int MissionDone = 5;
        public const int FullPrestige = 2000;
    }

    public class Cloud
    {
        public const string Version = "ver";
        //writer.Write(data.version);
        //writer.Write(data._adfree);
        //public const string AdFree = "caf";
        //writer.Write(data._moneyearned);
        public const string MoneyEarned = "cme";
        //writer.Write(data._moneyused);
        public const string MoneyUsed = "cmu";

        //(writer.Write(data.IdleBonusCount.Count);
        public const string IdleBonusCount = "cibc";
        public const string IdleBonusBase = "cibb_";
        //for (int i = 0; i<data.IdleBonusCount.Count; i++)
        //{
        //   writer.Write(data.IdleBonusCount[i].id);
        //    writer.Write(data.IdleBonusCount[i]._count);
        //}
        //writer.Write(data.TapBonusCount.Count);
        public const string TapBonusCount = "ctbc";
        public const string TapBonusBase = "ctbb_";
        //for (int i = 0; i<data.TapBonusCount.Count; i++)
        //{
        //    writer.Write(data.TapBonusCount[i].id);
        //    writer.Write(data.TapBonusCount[i]._count);
        //}
        //writer.Write(data.UpgradeCount.Count);
        public const string UpgradeCount = "cubc";
        public const string UpgradeBase = "cubb_";
        //for (int i = 0; i<data.UpgradeCount.Count; i++)
        //{
        //    writer.Write(data.UpgradeCount[i].id);
        //    writer.Write(data.UpgradeCount[i]._count);
        //}

        //writer.Write(data._diamonds);
        //writer.Write(data.PowerUpList.Count);
        //for (int i = 0; i<data.PowerUpList.Count; i++)
        //{
        //    writer.Write(data.PowerUpList[i]);
        //}
        //writer.Write(data._gigCoins);
        //writer.Write(data._famous);
        //writer.Write(data._level);
        public const string Level = "clvl";
        //writer.Write(data._sublevel);
        public const string SubLevel = "cslvl";

        //writer.Write(data.askRateLater);
        //writer.Write(data.likeAsked);
        //writer.Write(data.playerLike);
        //writer.Write(data.powerSave);
        // Version 12:
        //writer.Write(data.m_autotapOwned);
        // Mission:
        //writer.Write((int)data.CurrentMission.type);
        public const string CurrentMissionType = "cmt";
        //writer.Write(data.CurrentMission.TargetCount);
        public const string CurrentMissionTargetCount = "cmtc";
        //writer.Write(data.CurrentMission.CurrentCount);
        public const string CurrentMissionCurrentCount = "cmcc";
        //writer.Write((int)data.CurrentMission.currentFishType);
        public const string CurrentMissionCurrentFishType = "cmft";
        //writer.Write(data.CurrentMission.BonusCoins);
        public const string CurrentMissionBonusCoins = "cmbc";
        public const string CurrentMissionLaskettu = "cmla";
        // Version 13:
        //writer.Write(data.AudioVol);
        //writer.Write(data.MusicVol);
        // Version 14:
        //writer.Write(data.CurrentChapter);
        public const string CurrentChapter = "ccchap";
        //writer.Write(data.m_CurrentPhase);
        public const string CurrentPhase = "ccphase";
        // Version 16:
        //writer.Write(data.MissionCounter);
        public const string MissionCounter = "cmisc";
        // Version 17:
        //writer.Write(data._diamondsPurchased);
        public const string DiamondsPurchased = "cdp";
        //writer.Write(data._diamondsSpend);
        public const string DiamondsSpend = "cds";
        // Version 18:
        //writer.Write(data._PrestigeCount);
        public const string PrestigeCount = "cpc";
        //writer.Write(data._PrestigeLevel);
        public const string PrestigeLevel = "cpl";
        // Version 19:
        //writer.Write(data._Score);
        public const string Score = "cscr";
        // Verson 20:
        //writer.Write(data.LoggedOut);
        public const string GameStarted = "cgs";

        public const string IlmainenArkkuAvaamisAika = "iaaa";
    }

    public class Level
    {
        // Do not change level data
        public static int[] data = {
            5,-24,-45,-35,41,-15,-46,-39,34,2,-11,12,-10,75,5,-43,-44,22,-61,34,-32,-17,-42,-47,-51,33,-36,-49,-32,-33,-60,-40,-33,-30,41,-55,-44,-14,-39,35,
            -46,-38,-41,33,14,-33,-58,-42,-29,-4,-43,15,11,-32,-23,44,4,-24,17,-3,-39,16,-31,-47,87,22,-33,87,1,-57,1,-20,-41,-30,-6,-47,0,-35,-61,-38,
            -7,40,-8,-61,45,-4,9,-35,18,10,-60,75,2,-73,19,-27,-1,11,48,16,-4,-57,2,-24,-15,-7,25,-35,10,-26,19,50,-34,-23,-61,77,-53,-27,-58,-24,
            55,17,0,39,-20,-53,1,-52,-34,57,-60,3,-48,23,-28,-8,-15,-13,8,-5,-5,-59,-40,1,-73,90,-10,-43,11,-63,-18,20,-41,-57,-51,6,56,-14,-37,-69,
            3,40,18,-62,-20,-30,-42,-29,64,
            -15,-59,-41,-36,-32,47,-20,3,-26,9,56,-28,38,-58,17,-65,-30,-1,19,0,20,-45,-19,-2,4,90,-31,25,-18,-35,-24,4,-66,67,8,-46,-14,-13,-40,-42,
            9,81,-22,-12,-50,36,-14,-22,-7,-46,-29,47,-45,-20,-4,23,7,-10,-5,-4,46,-30,-40,-28,7,-35,-11,-47,-40,-15,-22,40,2,-49,-30,-10,-60,3,23,-38,
            -27,-34,-47,-1,65,-24,-28,-7,56,-42,-50,1,-33,6,52,-11,-31,3,-44,65,-41,-26,-52,-58,4,79,15,-62,-61,13,82,2,-53,10,51,-31,-4,-13,-30,-10,
            72,-33,-45,76,-18,-63,-8,-49,-13,68,73,23,-42,-24,79,-36,-21,9,-37,-49,17,-21,-1,-46,46,10,-55,-39,-57,-31,-33,-38,-10,-17,-39,-1,-2,53,-8,11,
            84,-66,-28,-11,-24,58,11,4,-40,-26,-30,-40,35,
            -13,-28,-63,25,-6,-33,50,15,-48,4,-14,-19,16,42,-27,10,-22,11,-51,56,18,-24,-23,18,-28,-20,-43,10,20,3,1,-2,-48,-26,-15,79,-22,-31,-32,20,
            -6,57,-66,8,41,-37,-45,-37,-36,-49
        };
    }

}
