using UnityEngine;
using System.Collections;
#if USES_GAME_SERVICES
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using VoxelBusters.NativePlugins.Internal;
#endif
using System;

namespace SoftcenSDK
{
    public class SCGameServices : MonoBehaviour
    {
#if USES_GAME_SERVICES
        public static SCGameServices Instance = null;
        public static event Action OnGameServiceChange;

        private bool debuglog;
#if !SOFTCEN_AMAZON
        private eLeaderboardTimeScope m_timeScope;
#endif
        private string[] m_leaderboardGIDList = new string[0];
        private string[] m_achievementGIDList = new string[0];

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        // Use this for initialization
        void Start()
        {
#if SOFTCEN_DEBUG
            debuglog = true;
#else
            debuglog = false;
#endif
#if !SOFTCEN_AMAZON
            m_timeScope = eLeaderboardTimeScope.ALL_TIME;
#endif
            // Extract gid information
            ExtractGID();

            // Leaderboard
            if (m_leaderboardGIDList.Length == 0)
                if (debuglog) Debug.LogWarning("Could not find leaderboard id information. Please configure it.");

            // Achievement
            if (m_achievementGIDList.Length == 0)
                if (debuglog) Debug.LogWarning("Could not find achievement id information. Please configure it.");

#if SOFTCEN_AMAZON
            InitAmazon();
#endif
#if UNITY_IOS
            Authenticate();
#endif

#if UNITY_ANDROID && !SOFTCEN_AMAZON
            int m_AutoSignIn = PlayerPrefs.GetInt("AutoSignIn", GameConsts.GooglePlayAutoSignDefault);
            if (m_AutoSignIn == 1)
            {
                Authenticate();
            }
#endif

        }

#if !SOFTCEN_AMAZON
        private bool _AchievementsLoadPending = false;
        void Update()
        {
            if (_AchievementsLoadPending)
            {
                _AchievementsLoadPending = false;
                LoadAchievements();
            }
        }
#endif

#region Public API Methods
        public bool IsGameServiceAvailable()
        {
#if SOFTCEN_AMAZON
            return AGSClient.IsServiceReady();
#else
            return NPBinding.GameServices.IsAvailable();
#endif
        }
        public bool Authenticating
        {
            get { return mAuthenticating; }
        }
        public void Authenticate()
        {
            if (Authenticated || mAuthenticating)
                return;

#if !SOFTCEN_AMAZON
            if (!NPBinding.GameServices.LocalUser.IsAuthenticated && NPBinding.GameServices.IsAvailable())
            {
                AuthenticateUser();
            }
#endif
        }
        public void LogOut()
        {
            SignOut();
        }
#endregion
#region API Methods

        private void IsAvailable()
        {
#if !SOFTCEN_AMAZON
            if (NPBinding.GameServices.IsAvailable())
                if (debuglog) Debug.Log("Game-Service feature is available.");
            else
                if (debuglog) Debug.Log("Game-Service feature is not available.");
#endif
        }

#endregion

#region Local User API Methods

        public bool Authenticated
        {
#if SOFTCEN_AMAZON
            get { return AGSClient.IsServiceReady();  }
#else
            get { return NPBinding.GameServices.LocalUser.IsAuthenticated; }
#endif
        }

        public bool IsAuthenticated()
        {
#if SOFTCEN_AMAZON
            return AGSClient.IsServiceReady();
#else
            return NPBinding.GameServices.LocalUser.IsAuthenticated;
#endif
        }

        private bool mAuthenticating = false; 
        private void AuthenticateUser()
        {
#if !SOFTCEN_AMAZON
            if (Authenticated || mAuthenticating)
                return;

            mAuthenticating = true;
            if (OnGameServiceChange != null)
            {
                OnGameServiceChange();
            }

            NPBinding.GameServices.LocalUser.Authenticate((bool _success, string _error) => {
                mAuthenticating = false;
                if (debuglog) Debug.Log("Local user authentication finished.");
                if (debuglog) Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));

                if (_success)
                {
                    _AchievementsLoadPending = true;
#if UNITY_ANDROID
                    PlayerPrefs.SetInt("AutoSignIn", 1);
                    PlayerPrefs.Save();
#endif
                    if (debuglog) Debug.Log(string.Format("Local user details= {0}.", NPBinding.GameServices.LocalUser));
                }
                if (OnGameServiceChange != null)
                {
                    OnGameServiceChange();
                }
            });
#endif
        }

        private void LoadAchievementDescriptions()
        {
#if !SOFTCEN_AMAZON
            if (Authenticated)
            {
                NPBinding.GameServices.LoadAchievementDescriptions((AchievementDescription[] _descriptions, string _error) => {

                    if (debuglog)
                    {
                        Debug.Log("Request to load achievement descriptions finished.");
                        Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));

                        if (_descriptions != null)
                        {
                            int _descriptionCount = _descriptions.Length;
                            Debug.Log(string.Format("Total loaded descriptions= {0}.", _descriptionCount));

                            for (int _iter = 0; _iter < _descriptionCount; _iter++)
                            {
                                Debug.Log(string.Format("[Index {0}]: {1}", _iter, _descriptions[_iter]));
                            }
                        }
                    }
                });

            }
#endif
        }

        private void LoadAchievements()
        {
#if !SOFTCEN_AMAZON
            if (Authenticated)
            {
                NPBinding.GameServices.LoadAchievements((Achievement[] _achievements, string _error) => {
                    if (debuglog)
                    {
                        Debug.Log("Request to load achievements finished.");
                        Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));

                        if (_achievements != null)
                        {
                            int _achievementCount = _achievements.Length;
                            Debug.Log(string.Format("Total loaded achievements= {0}.", _achievementCount));
                            for (int _iter = 0; _iter < _achievementCount; _iter++)
                            {
                                Debug.Log(string.Format("[Index {0}]: {1}", _iter, _achievements[_iter]));
                            }
                        }
                    }

                });
            }
#endif
        }


        private void SignOut()
        {
#if SOFTCEN_AMAZON
            if (AGSClient.IsServiceReady() )
            {
                AGSClient.ShowSignInPage();
            }
#else
#if UNITY_ANDROID
            PlayerPrefs.SetInt("AutoSignIn", 0);
            PlayerPrefs.Save();
#endif

            if (Authenticated)
            {
                NPBinding.GameServices.LocalUser.SignOut((bool _success, string _error) => {

                    if (_success)
                    {
                        if (debuglog) Debug.Log("Local user is signed out successfully!");
                    }
                    else
                    {
                        if (debuglog) Debug.Log("Request to signout local user failed.");
                        if (debuglog) Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
                    }
                    if (OnGameServiceChange != null)
                    {
                        OnGameServiceChange();
                    }

                });
            }
#endif
        }

        private void LoadFriends()
        {
#if !SOFTCEN_AMAZON
            if (Authenticated)
            {
                NPBinding.GameServices.LocalUser.LoadFriends((User[] _friends, string _error) =>
                {

                    if (debuglog) Debug.Log("Load friends info request finished.");
                    if (debuglog) Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));

                    if (_friends != null)
                    {
                        foreach (User _curFriend in _friends)
                        {
                            if (debuglog) Debug.Log(_curFriend.ToString());
                        }
                    }
                });
            }
#endif
        }

#endregion


#region Misc Methods

        private void ExtractGID()
        {
            IDContainer[] _leaderboardGIDCollection = NPSettings.GameServicesSettings.LeaderboardIDCollection;
            IDContainer[] _achievementGIDCollection = NPSettings.GameServicesSettings.AchievementIDCollection;

            // Extract id infomation
            m_leaderboardGIDList = new string[_leaderboardGIDCollection.Length];
            m_achievementGIDList = new string[_achievementGIDCollection.Length];

            for (int _iter = 0; _iter < _leaderboardGIDCollection.Length; _iter++)
                m_leaderboardGIDList[_iter] = _leaderboardGIDCollection[_iter].GlobalID; // .GetGlobalID();

            for (int _iter = 0; _iter < _achievementGIDCollection.Length; _iter++)
                m_achievementGIDList[_iter] = _achievementGIDCollection[_iter].GlobalID; // .GetGlobalID();
        }
#endregion

#region UI API Methods

        /// <summary>
        /// Shows Game Services Achievements UI
        /// </summary>
        public void ShowAchievementsUI()
        {
            if (debuglog) Debug.Log("Sending request to show achievements view.");
#if SOFTCEN_AMAZON
            if (AGSClient.IsServiceReady())
            {
                AGSAchievementsClient.ShowAchievementsOverlay();
            }
#else

            if (Authenticated)
            {
                NPBinding.GameServices.ShowAchievementsUI((string _error) =>
                {
                    if (debuglog) Debug.Log("Achievements view dismissed.");
                    if (debuglog) Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
                });
            }
#endif
        }

        /// <summary>
        /// Shows leaderboard UI
        /// </summary>
        /// <param name="_leaderboadGID">Global ID string of leaderboard</param>
        public void ShowLeaderboardUIWithGlobalID(string _leaderboadGID)
        {
            if (debuglog) Debug.Log("Sending request to show leaderboard view. GID: " + _leaderboadGID);
#if SOFTCEN_AMAZON
            if (AGSClient.IsServiceReady())
            {
                AGSLeaderboardsClient.ShowLeaderboardsOverlay();
            }
#else
            if (debuglog)
            {
                /* TODO NPBinding.GameServices 
                string _leaderboardID = GameServicesIDHandler.GetLeaderboardID(_leaderboadGID);
                Debug.Log("GameServer LeaderboardID: ->:" + _leaderboardID+":<-");*/
            }
            if (Authenticated)
            {
                NPBinding.GameServices.ShowLeaderboardUIWithGlobalID(_leaderboadGID, m_timeScope, (string _error) => {
                    if (debuglog) Debug.Log("Leaderboard view dismissed.");
                    if (debuglog) Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
                });
            }

#endif
        }


        /// <summary>
        /// Returns leaderboard Global Id string.
        /// </summary>
        /// <param name="_index">Index of leaderboard</param>
        public string GetLeaderboardGID(int _index)
        {
            if (_index < m_leaderboardGIDList.Length)
            {
                return m_leaderboardGIDList[_index];
            }
            return "";
        }

        /// <summary>
        /// Reports the score to game service server.
        /// </summary>
        /// <param name="_leaderboardGID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> across all supported platforms.</param>
        /// <param name="_score">The score earned by <see cref="VoxelBusters.NativePlugins.LocalUser"/></param>
        public void ReportScoreWithGlobalID(string _leaderboardGID, int _score)
        {
#if SOFTCEN_AMAZON
            if (AGSClient.IsServiceReady())
            {
                AGSLeaderboardsClient.SubmitScore(_leaderboardGID, _score);
            }
#else
            if (Authenticated)
            {
                NPBinding.GameServices.ReportScoreWithGlobalID(_leaderboardGID, _score, (bool _success, string _error) => {

                    if (_success)
                    {
                        if (debuglog) Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} finished successfully.", _leaderboardGID));
                        if (debuglog) Debug.Log(string.Format("New score= {0}.", _score));
                    }
                    else
                    {
                        if (debuglog) Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} failed.", _leaderboardGID));
                        if (debuglog) Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
                    }
                });

            }
#endif

        }

        /// <summary>
        /// Returns achievement Global Id string.
        /// </summary>
        /// <param name="_index">Index of achievement</param>
        public string GetAchievementGID(int _index)
        {
            if (_index < m_achievementGIDList.Length)
            {
                return m_achievementGIDList[_index];
            }
            return "";
        }

        /// <summary>
        /// Reports the player’s achievement complete progress for given global identifier.
        /// </summary>
        /// <param name="_achievementGID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Achievement"/> across all supported platforms.</param>
        public void UnlockAchievement(string _achievementGID)
        {
            ReportAchievementProgress(_achievementGID, 100);
        }

        /// <summary>
        /// Reports the player’s achievement progress for given global identifier.
        /// </summary>
        /// <param name="_achievementGID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Achievement"/> across all supported platforms.</param>
        /// <param name="_pointsScored">Value indicates how far the player has progressed.</param>
        public void ReportAchievementProgress(string _achievementGID, int _pointsTillNow)
        {
#if SOFTCEN_AMAZON
            if (AGSClient.IsServiceReady())
            {
                AGSAchievementsClient.UpdateAchievementProgress(_achievementGID, (float)_pointsTillNow);
            }
#else
            if (Authenticated)
            {
                /* TODO: NPBinding.GameServices ReportProgressWithGlobalID 
                NPBinding.GameServices.ReportProgressWithGlobalID(_achievementGID, _pointsTillNow, (bool _status, string _error) => {

                    if (_status)
                    {
                        if (debuglog) Debug.Log(string.Format("Request to report progress of achievement with GID= {0} finished successfully.", _achievementGID));
                        if (debuglog) Debug.Log(string.Format("New points= {0}.", _pointsTillNow));
                    }
                    else
                    {
                        if (debuglog) Debug.Log(string.Format("Request to report progress of achievement with GID= {0} failed.", _achievementGID));
                        if (debuglog) Debug.Log(string.Format("Error= {0}.", _error.GetPrintableString()));
                    }
                });
                */
            }
#endif
        }

#endregion

#region Amazon
#if SOFTCEN_AMAZON
        private bool AmazonServiceInitialized = false;
        public void InitAmazon()
        {
            if (!AmazonServiceInitialized)
            {
                AmazonServiceInitialized = true;
                AGSClient.ServiceReadyEvent += AmazonServiceReadyHandler;
                AGSClient.ServiceNotReadyEvent += AmazonServiceNotReadyEvent;
                Social.Active = GameCircleSocial.Instance;
                mAuthenticating = true;
                AGSClient.SetPopUpEnabled(true);
                AGSClient.SetPopUpLocation(GameCirclePopupLocation.TOP_CENTER);

                AGSClient.Init(true, true, false);
            }
        }

        private void AmazonServiceReadyHandler()
        {
            mAuthenticating = false;
            if (OnGameServiceChange != null)
                OnGameServiceChange();
            if (debuglog) Debug.Log("***** AmazonServiceReadyHandler *****");
            if (AGSClient.IsServiceReady())
            {
                if (debuglog) Debug.Log("***** Amazon Service ready");
            }
            if (debuglog)
            {
                if (Authenticated)
                    Debug.Log("***** AUTHENTICATED OK *****");
                else
                    Debug.Log("***** AUTHENTICATED FAIL *****");
            }

        }

        private void AmazonServiceNotReadyEvent(string error)
        {
            mAuthenticating = false;
            if (OnGameServiceChange != null)
                OnGameServiceChange();
            if (debuglog) Debug.Log("AmazonServiceNotReadyEvent " + error);
        }

        private void SubscribeToGameCircleInitializationEvents()
        {
            AGSClient.ServiceReadyEvent += Amazon_ServiceReadyEvent;
            AGSClient.ServiceNotReadyEvent += Amazon_ServiceNotReadyEvent;
        }
        private void UnsubscribeFromGameCircleInitializationEvents()
        {
            AGSClient.ServiceReadyEvent -= Amazon_ServiceReadyEvent;
            AGSClient.ServiceNotReadyEvent -= Amazon_ServiceNotReadyEvent;
        }

        private void Amazon_ServiceReadyEvent()
        {
            //Debug.Log("******************\nGameManager Amazon_ServiceReadyEvent\n******************\n");
            mAuthenticating = false;
            UnsubscribeFromGameCircleInitializationEvents();
            AGSClient.SetPopUpEnabled(true);
            AGSClient.SetPopUpLocation(GameCirclePopupLocation.TOP_CENTER);
        }
        private void Amazon_ServiceNotReadyEvent(string error)
        {
            //Debug.Log("******************\nGameManager Amazon_ServiceNotReadyEvent error: " + error +"\n******************\n");
            mAuthenticating = false;
            UnsubscribeFromGameCircleInitializationEvents();
        }
        private void Amazon_UpdateAchievementSucceededEvent(string achievementID)
        {
            //Debug.Log("******************\nGameManager Amazon_UpdateAchievementSucceededEvent: " +achievementID +"\n******************\n");

        }
        private void Amazon_UpdateAchievementFailedEvent(string achievementID, string error)
        {
            //Debug.Log("******************\nGameManager Amazon_UpdateAchievementFailedEvent: " +achievementID + ", error: " + error +
            //          "\n******************\n");

        }
        private void Amazon_SubmitScoreSucceededEvent(string leaderboardID)
        {
            //Debug.Log("******************\nGameManager Amazon_SubmitScoreSucceededEvent: " +leaderboardID +"\n******************\n");

        }
        private void Amazon_SubmitScoreFailedEvent(string leaderboardID, string error)
        {
            //Debug.Log("******************\nGameManager Amazon_SubmitScoreFailedEvent: " +leaderboardID + ", error: " + error +
            //          "\n******************\n");

        }

#endif

#endregion
// end of USES_GAME_SERVICES
#endif
        }
    }
