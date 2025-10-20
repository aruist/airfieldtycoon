using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using VoxelBusters.EssentialKit;

public class GameManager : MonoBehaviour {
    public bool checkCloud = false;

    public static GameManager Instance = null;
    public static event Action OnSaveError;
    public static event Action OnGameLoaded;
    //public static event Action OnLoggedOut;

    public bool saveFailedShowed = false;
    public bool sessionRateAsked = false;
    //public bool loggedOut = false;
    public bool tapDisabled = false;
    private int _selectedChapter;
    private bool m_SaveFailedShowed = false;
    private bool m_QuitButtonPressed = false;

    public void QuitButtonPressed(bool status)
    {
            #if SOFTCEN_DEBUG
            Debug.Log($"GameManager QuitButtonPressed {status}");
            #endif
        m_QuitButtonPressed = status;
    }

    public bool GetQuitButtonPressed()
    {
        return m_QuitButtonPressed;
    }

    public int selectedChapter
    {
        get { return _selectedChapter; }
        set
        {
            _selectedChapter = value;
            #if SOFTCEN_DEBUG
            Debug.Log("GameManager selectedChapter new value: " + selectedChapter);
            #endif
        }
    }

    public AudioMixer mainAudioMixer;

    public PlayerData playerData;
    public PlayerData cloudPlayerData = null;

    public bool IsPhaceCompleted = false;
    public float NextChapterCoinPrice;
    public int NextChapterDiamondPrice;

#if SOFTCEN_DEBUG
    public bool dev_SkipMoney = false;
#endif

    public bool autotap_tryout = false;

    public float rateGameTime;
    private bool kaupparekisteroity = false;

	void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this);
            kaupparekisteroity = true;

        }
		else {
			Destroy(this.gameObject);
			return;
		}
        rateGameTime = Time.time;
	}

    private void OnDestroy()
    {
        if (kaupparekisteroity)
        {
            kaupparekisteroity = false;
        }
    }

    void OnEnable()
    {
        PlayerData.OnLevelChanged += CheckAchievements;
		PlayerData.OnTapUpgrade += CheckAchievements;
		PlayerData.OnIdleUpgrade += CheckAchievements;
        Pelikeskus.OnAuthenticated += Pelikeskus_OnAuthenticated;
    }

    void OnDisable()
    {
        PlayerData.OnLevelChanged -= CheckAchievements;
		PlayerData.OnTapUpgrade -= CheckAchievements;
		PlayerData.OnIdleUpgrade -= CheckAchievements;
        Pelikeskus.OnAuthenticated -= Pelikeskus_OnAuthenticated;
    }

    // Gets called when the player opens the notification.
    void Start()
    {
        Load();
        // Setup audio:
        SetSFXVolume(playerData.AudioVol);
        SetMusicVolume(playerData.MusicVol);

        #if SOFTCEN_DEBUG
        Debug.Log("GameManager Start() playerData.LoggedOut: " + playerData.LoggedOut + ", Authenticated: " + Pelikeskus.Instance.Authenticated() +
            ", pilvipalvelut: " + Pilvipalvelut.Instance != null);
        #endif
        #if UNITY_ANDROID
        if (!playerData.LoggedOut)
        {
            if (!Pelikeskus.Instance.Authenticated()) {
                Pelikeskus.Instance.Authenticate();
            }
            else {
                Pilvipalvelut.Instance.Syncronize();
            }
        } else
        {
            Pilvipalvelut.Instance.CallSynchronizeReady();
        }
        #else
        Pelikeskus.Instance.Authenticate();
        Pilvipalvelut.Instance.Syncronize();
        #endif

        /*OneSignal.StartInit("810440c1-5aaf-4215-9524-1c54760a8d6d", "127148250235")
        #if UNITY_IOS
			.Settings(new Dictionary<string, bool>() {
				{ OneSignal.kOSSettingsAutoPrompt, true },
				{ OneSignal.kOSSettingsInAppLaunchURL, false } })
        #endif
			.EndInit();*/

        Tienistit.Instance.Init();
        if(!IsAdFree())
        {
            Tienistit.Instance.ShowBanner();
        }
    }

    public void ResetGame() {
#if SOFTCEN_DEBUG
		Debug.Log("GameManager ResetGame");
        //StartUp.Reset();
		playerData = new PlayerData();
		playerData.Init();
		Save();

        _selectedChapter = 0;
        IsPhaceCompleted = false;
        NextChapterCoinPrice = 0;
        NextChapterDiamondPrice = 0;

#endif
    }

    private void Load() {
        string filename = GameConsts.DataFile + GameConsts.FilenamePrefix + GameConsts.FileKey;
        string filename2 = GameConsts.DataFile + GameConsts.FilenamePrefix + GameConsts.FileKey + "&saveLocation=playerprefs&tag=myTag";
        #if SOFTCEN_DEBUG
        Debug.Log("GameManager LOAD " + filename);
        #endif
        //ES2Settings settings = new ES2Settings();
        //settings.saveLocation = ES2Settings.SaveLocation.PlayerPrefs;
        if (ES2.Exists(filename2))
        {
            Debug.Log("GameManager Load with settings");
            try
            {
                playerData = ES2.Load<PlayerData>(filename2);
            }
            catch
            {
                #if SOFTCEN_DEBUG
				Debug.Log("GameManager <color=red>Playerdata load failed</color>");
                #endif
                playerData = new PlayerData();
                playerData.Init();
            }
            selectedChapter = playerData.CurrentChapter;
        }
        else
        {
            // Using old data file system
            if (ES2.Exists(GameConsts.DataFile))
            {
                try
                {
                    playerData = ES2.Load<PlayerData>(filename);
                }
                catch
                {
                #if SOFTCEN_DEBUG
				Debug.Log("GameManager <color=red>Playerdata load failed</color>");
                #endif
                    playerData = new PlayerData();
                    playerData.Init();
                }
            }
            else
            {
                #if SOFTCEN_DEBUG
			    Debug.Log("GameManager No Playerdata found. Create new one!");
                #endif
                playerData = new PlayerData();
                playerData.Init();
            }
            selectedChapter = playerData.CurrentChapter;
            // Save to playerprefs
            playerData.SetDirty();
            //Save();
        }
        #if SOFTCEN_DEBUG
        Debug.Log("GameManager Load done!");
        #endif
        if (OnGameLoaded != null)
        {
            OnGameLoaded();
        }
    }


    public void Save() {
        string filename = GameConsts.DataFile + GameConsts.FilenamePrefix + GameConsts.FileKey + "&saveLocation=playerprefs&tag=myTag";
        #if SOFTCEN_DEBUG
        Debug.Log($"GameManager SAVE changed {playerData.Changed}, m_QuitButtonPressed: {m_QuitButtonPressed}, filename: {filename}");
        #endif

        if (m_QuitButtonPressed == false) ReportScore(GameConsts.LeaderBoards.TopScores, playerData.Score);

        // #if SOFTCEN_DEBUG
        // Debug.Log("GameManager SAVE playerData Changed: " + playerData.Changed + ", filename: " + filename);
        // #endif
        if (playerData.Changed) {
            try
            {
                ES2.Save(playerData, filename);
                playerData.SetSaved();
            }
            catch
            {
                if (!m_SaveFailedShowed && HomeManager.Instance != null)
                {
                    m_SaveFailedShowed = true;
                    HomeManager.Instance.OpenSaveFailedDlg();
                }
            }

            if (Pelikeskus.Instance.Authenticated())
            {
                Pilvipalvelut.Instance.SaveAllDataToCloud();
                Pilvipalvelut.Instance.Syncronize();
            }
            else if (m_QuitButtonPressed)
            {
                Application.Quit();
            }
        }
        else {
            #if SOFTCEN_DEBUG
            Debug.Log("GameManager No data change SAVE skipped");
            #endif
        }
        #if SOFTCEN_DEBUG
        Debug.Log("GameManager Save END");
        #endif
	}


	public bool IsAdFree() {
        /*Debug.Log("GameManager IsAdFree: " + playerData.AdFree
                  + ", gm.instance._adfree: " + GameManager.Instance.playerData._adfree
                  + ", playerdata._adfree: " + playerData._adfree
                  );*/
		return playerData.AdFree;
	}

	public void SetAdFree() {
        #if SOFTCEN_DEBUG
        Debug.Log("GameManager SetAdFree()");
        #endif
        playerData.AdFree = true;
        Save ();
        if (Tienistit.Instance != null)
            Tienistit.Instance.SetAdFree(true); // hillo.Instance.PermanentlyDisableAds();
    }

    public bool hasRewardVideo()
    {
        if (Tienistit.Instance != null)
            return Tienistit.Instance.HasRewarded();// hasRewardVideo();
        return false;
    }
    public void ShowInterstitial(bool useTime)
    {
        if (Tienistit.Instance != null)
            Tienistit.Instance.ShowInterstitial(useTime);
    }
    public bool CanShowInterstitialAd(bool useTime)
    {
        if (Tienistit.Instance != null)
            return Tienistit.Instance.CanShowInterstitial(); // .CanShowInterstitialAd(useTime);
        return false;
    }
    public bool HasInterstitial()
    {
        if (Tienistit.Instance != null)
            return Tienistit.Instance.HasInterstitial();
        return false;
    }
    public void showRewardVideo(int id)
    {
        if (Tienistit.Instance != null)
            Tienistit.Instance.ShowRewarded(id);
    }

    void OnApplicationQuit() {
        #if SOFTCEN_DEBUG
        Debug.Log("Gamemanager OnApplicationQuit");
        #endif
        if (m_QuitButtonPressed == false) Save ();
    }


    void OnApplicationPause(bool pauseStatus)
    {
        #if SOFTCEN_DEBUG
        Debug.Log("Gamemanager OnApplicationPause: " + pauseStatus);
        #endif
        if (pauseStatus)
        {
			if (!Pilvipalvelut.Instance.loggedOut)
            	Save();
        }
        else
        {
			if (!Pilvipalvelut.Instance.loggedOut && playerData != null )
            {
                Pilvipalvelut.Instance.ReadAllFromCloud();
                Pilvipalvelut.Instance.CompareLocalAndCloudData(true);
            }
        }
    }

    public void SetSFXVolume(float volume)
    {
        mainAudioMixer.SetFloat("SFXVol", volume);
        playerData.AudioVol = volume;

    }
    public void SetMusicVolume(float volume)
    {
        mainAudioMixer.SetFloat("MusicVol", volume);
        GameManager.Instance.playerData.MusicVol = volume;
    }

    public string Key()
    {
        string key = "";
        int index = 0;
        for (int i = 0; i < GameConsts.Description1.Length; i++)
        {
            int c = GameConsts.Level.data[index] + GameConsts.Description1[i];
            key += (char)c;
            index++;
        }
        for (int i = 0; i < GameConsts.Description2.Length; i++)
        {
            int c = GameConsts.Level.data[index] + GameConsts.Description2[i];
            key += (char)c;
            index++;
        }
        for (int i = 0; i < GameConsts.Description3.Length; i++)
        {
            int c = GameConsts.Level.data[index] + GameConsts.Description3[i];
            key += (char)c;
            index++;
            if (index >= GameConsts.Level.data.Length) break;
        }
        return key;
    }

    public bool IsPrestige
    {
        get { return (playerData.PrestigeLevel > playerData.Level); }
    }

    public void Prestige()
    {
        int currentChapter = GameManager.Instance.playerData.CurrentChapter;
        int maxPhase = BonusManager.Instance.GetChapterMaxPhase(currentChapter);
        if (currentChapter == Chapters.MaxChapter && GameManager.Instance.playerData.CurrentPhase == maxPhase)
        {
            CompleteAchievement("GID_FullPrestige");
            GameManager.Instance.playerData.Score += 1000;
        }


#if SOFTCEN_DEBUG
        Debug.Log("GameManager Old Prestige count: " + playerData.PrestigeCount
            + ", level: " + playerData.PrestigeLevel
            );
#endif
        playerData.PrestigeCount++;
        if (playerData.PrestigeLevel < playerData.Level-1)
            playerData.PrestigeLevel = playerData.Level-1;
        playerData.Prestige();
        _selectedChapter = 0;
        IsPhaceCompleted = false;
        NextChapterCoinPrice = 0;
        NextChapterDiamondPrice = 0;
		Pilvipalvelut.Instance.PrestigeCloudKeys();
        Save();
        SynchronizeCloud ();
#if SOFTCEN_DEBUG
        Debug.Log("GameManager New Prestige count: " + playerData.PrestigeCount
            + ", level: " + playerData.PrestigeLevel
            );
#endif
    }

    private void Pelikeskus_OnAuthenticated()
    {
#if SOFTCEN_DEBUG
        Debug.Log("GameManager Pelikeskus_OnAuthenticated");
#endif
        Pilvipalvelut.Instance.Syncronize();
    }

    public void ShowLeaderboard()
    {
        ReportScore(GameConsts.LeaderBoards.TopScores, playerData.Score);
        Pelikeskus.Instance.NaytaTulostaulukko();
    }

    public void ShowAchievements()
    {
        ReportScore(GameConsts.LeaderBoards.TopScores, playerData.Score);
        CheckAchievements();
        Pelikeskus.Instance.NaytaSaavutukset();
    }

    public void GameServicesLogOut()
    {
#if USES_GAME_SERVICES
        if (NPBinding.GameServices.LocalUser.IsAuthenticated)
        {
            playerData.LoggedOut = true;
            NPBinding.GameServices.LocalUser.SignOut((bool _success, string _error) => {

                if (_success)
                {
#if SOFTCEN_DEBUG
                    Debug.Log("GameManager Local user is signed out successfully!");
#endif
                }
                else
                {
#if SOFTCEN_DEBUG
                    Debug.Log("GameManager Request to signout local user failed.");
                    Debug.Log(string.Format("GameManager Error= {0}.", _error));
#endif
                }
                if (OnGameServiceChanged != null)
                    OnGameServiceChanged();

            });
        }
#endif

    }

    public void CompleteAchievement(string globalAchId)
    {
        Pelikeskus.Instance.RaportoiSaavutus(globalAchId, 100);
    }

    public void ReportScore(string _leaderboardGID, long score)
    {
        Pelikeskus.Instance.RaportoiTulos(_leaderboardGID, score);
    }

    public void CheckAchievements()
    {
        // Buy helicopter
        if (playerData.GetTapBonusCount((int)Item.Identifications.Tap_C2_SmallHelicopter1) > 0)
        {
            CompleteAchievement("GID_BuyHelicopter");
        }
        // Buy privete jet
        if (playerData.GetTapBonusCount((int)Item.Identifications.Tap_C3_Jet06) > 0)
        {
            CompleteAchievement("GID_BuyPrivateJet");
        }
        // Buy Fighter Z-3
        if (playerData.GetTapBonusCount((int)Item.Identifications.Tap_C6_Military_SmallFighter) > 0)
        {
            CompleteAchievement("GID_BuyFighterZ3");
        }
        // Hire air hostess Ulla
        if (playerData.GetIdleBonusCount((int)Item.Identifications.Idle_C5_AirHostessRita) > 0)
        {
            CompleteAchievement("GID_HireUlla");
        }
        // Hire air hostess Mary
        if (playerData.GetIdleBonusCount((int)Item.Identifications.Idle_C7_AirHostessMary) > 0)
        {
            CompleteAchievement("GID_HireMary");
        }
        // Make full prestige -> this is checked in prestige button

        // Build three air control towers
        if (playerData.GetUpgradeCount((int)Item.Identifications.Lvl_C3_Tower) > 0
            && playerData.GetUpgradeCount((int)Item.Identifications.Lvl_C4_Tower) > 0
            && playerData.GetUpgradeCount((int)Item.Identifications.Lvl_C5_Tower) > 0
            )
        {
            CompleteAchievement("GID_ThreeTowers");
        }

    }

    public void SynchronizeCloud() {
#if USES_CLOUD_SERVICES && UNITY_ANDROID
        if (m_CloudServicesEnabled) {
            NPBinding.CloudServices.Synchronise();
        }
#endif
    }
}
