using System;
using UnityEngine;
using VoxelBusters.CoreLibrary;
#if PLAYGAMES_SDK
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
using VoxelBusters.EssentialKit;

/* Tämä luokka hoitelee Leaderboardin ja Achievementsit
 * 
 */
public class Pelikeskus : MonoBehaviour
{
    public static Pelikeskus Instance;
    public static event Action OnAuthenticated;
    public static event Action OnStateChanged;

#if PLAYGAMES_SDK
    public PlayGamesPlatform platform;
#endif

    //private bool authenticating = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameServices.OnAuthStatusChange += OnAuthStatusChange;
    }
    private void OnDisable()
    {
        GameServices.OnAuthStatusChange -= OnAuthStatusChange;
    }

    private void OnAuthStatusChange(GameServicesAuthStatusChangeResult result, Error error)
    {
        if (error == null)
        {
#if SOFTCEN_DEBUG
            Debug.Log("Pelikeskus OnAuthStatusChange Received auth status change event");
            Debug.Log("Pelikeskus OnAuthStatusChange Auth status: " + result.AuthStatus);
#endif
            if (result.AuthStatus == LocalPlayerAuthStatus.Authenticated)
            {
#if SOFTCEN_DEBUG
                Debug.Log("Pelikeskus OnAuthStatusChange Local player: " + result.LocalPlayer);
#endif
                if (OnAuthenticated != null)
                {
                    OnAuthenticated();
                }
            }            
        }
#if SOFTCEN_DEBUG
        else
        {
            Debug.LogError("Pelikeskus OnAuthStatusChange Failed login with error : " + error);
        }
#endif
        if (OnStateChanged != null)
        {
            OnStateChanged();
        }
    }

    public void Signout()
    {
        if (GameServices.IsAvailable() && GameServices.IsAuthenticated)
        {
            GameServices.Signout();
        }
    }

    public bool Authenticated()
    {
        bool loggedIn = GameServices.IsAuthenticated;
        return loggedIn;
        //return Social.Active.localUser.authenticated;
    }

    public void Authenticate()
    {
        bool isAvailable = GameServices.IsAvailable();
        bool isAuthenticated = Authenticated();
        if (!isAvailable || isAuthenticated)
        {
            #if SOFTCEN_DEBUG
            Debug.Log("Pelikeskus Authenticate skip. isAvailable: " + isAvailable + ", isAuthenticated: " + isAuthenticated);
            #endif
            return;
        }

        try
        {
            if (GameManager.Instance != null) {
                GameManager.Instance.playerData.LoggedOut = false;
            }
            GameServices.Authenticate();
        }
        catch
        {

        }

        /*if (Social.Active.localUser.authenticated || authenticating)
        {
            #if SOFTCEN_DEBUG
            Debug.Log("Pelikeskus Authenticate skipped: " + Social.Active.localUser.authenticated + ", " + authenticating);
            #endif
            return;
        }*/


#if EI_KAYTOSSA
#if PLAYGAMES_SDK
        if (platform == null)
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                    // enables saving game progress.
                    .EnableSavedGames()
                    // requests the email address of the player be available.
                    // Will bring up a prompt for consent.
                    // .RequestEmail()
                    // requests a server auth code be generated so it can be passed to an
                    //  associated back end server application and exchanged for an OAuth token.
                    // .RequestServerAuthCode(false)
                    // requests an ID token be generated.  This OAuth token can be used to
                    //  identify the player to other services such as Firebase.
                    // .RequestIdToken()
                    .Build();

            PlayGamesPlatform.InitializeInstance(config);
            // recommended for debugging:
#if SOFTCEN_DEBUG
            PlayGamesPlatform.DebugLogEnabled = true;
#endif
            // Activate the Google Play Games platform
            platform = PlayGamesPlatform.Activate();
        }
#endif
        authenticating = true;
        Social.Active.localUser.Authenticate(success =>
        {
            authenticating = false;
            if (success)
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pelikeskus Authentication successful");
                #endif
                if (OnAuthenticated != null)
                {
                    OnAuthenticated();
                }
            }
            else
            {
                #if SOFTCEN_DEBUG
                Debug.Log("Pelikeskus Authentication failed");
                #endif
            }
        });
#endif
    }

    #region LEADERBOARD
    public void RaportoiTulos(string leaderboardId, long score)
    {
        #if SOFTCEN_DEBUG
        Debug.Log("Pelikeskus RaportoiTulos " + leaderboardId + " : " + score);
        #endif
        if (GameServices.IsAvailable() && Authenticated())
        {
            try
            {
                GameServices.ReportScore(leaderboardId, score, (success, error) =>
                {
                    if (success)
                    {
                        #if SOFTCEN_DEBUG
                        Debug.Log("Request to submit score finished successfully.");
                        #endif
                    }
                    else
                    {
                        #if SOFTCEN_DEBUG
                        Debug.Log("Request to submit score failed with error: " + error.Description);
                        #endif
                    }
                });

                /*GameServices.ReportScore(leaderboardId, score, (error) =>
                {
                    if (error == null)
                    {
                        #if SOFTCEN_DEBUG
                        Debug.Log("Pelikeskus Request to submit score finished successfully.");
                        #endif
                    }
                    else
                    {
                        #if SOFTCEN_DEBUG
                        Debug.Log("Pelikeskus Request to submit score failed with error: " + error.Description);
                        #endif
                    }
                });*/

            }
            catch
            {

            }
        }
    }

    public void NaytaTulostaulukko()
    {
        bool isAvailable = GameServices.IsAvailable();
        bool isAuthenticated = Authenticated();
        #if SOFTCEN_DEBUG
        Debug.Log("Pelikeskus NaytaTulostaulukko isAvail: " + isAvailable + ", " + " auth: " + isAuthenticated);
        #endif
        if (isAvailable && isAuthenticated)
        {
            try
            {
                GameServices.ShowLeaderboards(callback: (result, error) =>
                {
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pelikeskus NaytaTulostaulukko Leaderboards UI closed result " + result.ToString() + ", error: " + error.Description);
                    #endif
                });
            }
            catch
            {

            }
        }
    }
    #endregion

    #region ACHIEVEMENTS
    public void NaytaSaavutukset()
    {
        bool isAvailable = GameServices.IsAvailable();
        bool isAuthenticated = Authenticated();
        #if SOFTCEN_DEBUG
        Debug.Log("Pelikeskus NaytaSaavutukset isAvail: " + isAvailable + ", " + " auth: " + isAuthenticated);
        #endif
        if (isAvailable && Authenticated())
        {
            try
            {
                GameServices.ShowAchievements((result, error) =>
                {
                    #if SOFTCEN_DEBUG
                    Debug.Log("Pelikeskus NaytaSaavutukset Achievements view closed " + result.ToString() + ", error: " + error.Description);
                    #endif
                });
            }
            catch
            {

            }
        }
    }
    public void RaportoiSaavutus(string id, double progress)
    {
        bool isAvailable = GameServices.IsAvailable();
        bool isAuthenticated = Authenticated();
        #if SOFTCEN_DEBUG
        Debug.Log("Pelikeskus RaportoiSaavutus id: " + id + " : " + progress + ", isAvail: " + isAvailable + ", " + " auth: " + isAuthenticated);
        #endif
        if (isAvailable && isAuthenticated)
        {
            try
            {
                GameServices.ReportAchievementProgress(id, progress, (success, error) =>
                {
                    if (success)
                    {
                        #if SOFTCEN_DEBUG
                        Debug.Log("Pelikeskus RaportoiSaavutus Request to submit progress finished successfully.");
                        #endif
                    }
                    else
                    {
                        #if SOFTCEN_DEBUG
                        Debug.Log("Pelikeskus RaportoiSaavutus Request to submit progress failed with error. Error: " + error);
                        #endif
                    }
                });

                /*GameServices.ReportAchievementProgress(id, progress, (error) =>
                {
                    if (error == null)
                    {
                        #if SOFTCEN_DEBUG
                        Debug.Log("Pelikeskus RaportoiSaavutus Request to submit progress finished successfully.");
                        #endif
                    }
                    else
                    {
                        #if SOFTCEN_DEBUG
                        Debug.Log("Pelikeskus RaportoiSaavutus Request to submit progress failed with error. Error: " + error);
                        #endif
                    }
                });*/
            }
            catch
            {

            }
        }
    }
    #endregion

    #region SAVED_GAMES
#if PLAYGAMES_SDK
    void ShowSelectUI()
    {
#if SOFTCEN_DEBUG
        Debug.Log("Pelikeskus ShowSelectUI");
#endif
        uint maxNumToDisplay = 5;
        bool allowCreateNew = false;
        bool allowDelete = true;

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ShowSelectSavedGameUI("Select saved game",
            maxNumToDisplay,
            allowCreateNew,
            allowDelete,
            OnSavedGameSelected);
}

public void OnSavedGameSelected(SelectUIStatus status, ISavedGameMetadata game)
    {
        if (status == SelectUIStatus.SavedGameSelected)
        {
            // handle selected game save
#if SOFTCEN_DEBUG
            Debug.Log("Pelikeskus OnSavedGameSelected Ok");
#endif
        }
        else
        {
            // handle cancel or error
#if SOFTCEN_DEBUG
            Debug.Log("Pelikeskus OnSavedGameSelected Error");
#endif
        }
    }
#endif
    #endregion
}
