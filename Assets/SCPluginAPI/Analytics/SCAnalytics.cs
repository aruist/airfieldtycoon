#if !NOSCANALYTIC
using UnityEngine;
using System.Collections;

public class SCAnalytics
{
    /*public enum eventType
    {
        ADDPAYMENTINFO,
        ADDTOCART,
        APPOPEN,
        BEGINCHECKOUT,
        EARNVIRTUALCURRENCY,
        ECOMMERCEPURCHASE,
        GENERATELEAD,
        JOINGROUP,
        LEVELUP,
        LOGIN,
        POSTSCORE,
        PRESENTOFFER,
        PURCHASEREFUND,
        SEARCH,
        SELECTCONTENT,
        SHARE,
        SIGNUP,
        SPENDVIRTUALCURRENCY,
        TUTORIALBEGIN,
        TUTORIALCOMPLETE,
        UNLICKACHIEVEMENT,
        VIEWITEM,
        VIEWITEMLIST,
        VIEWSEARCHRESULTS,
    }*/

    public static void LogEvent(string eventCategory, string eventAction, string eventLabel, long value)
    {
        //Firebase.Analytics.FirebaseAnalytics.LogEvent(eventAction, eventLabel, value);
    }
    public static void InitializeGoogleAnalyticsV4()
    {
    }
    public static void LogScreen(string title)
    {
       // Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventJoinGroup, Firebase.Analytics.FirebaseAnalytics.ParameterGroupId, title);
    }

    // Log an event with no parameters.
    /*public static void LogEvent(eventType type)
    {
        switch(type)
        {
            case eventType.LOGIN:
                Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);
                break;
        }
    }*/

    /*
    // Log an event with a float parameter
    Firebase.Analytics.FirebaseAnalytics.LogEvent("progress", "percent", 0.4f);

// Log an event with an int parameter.
Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventPostScore, Firebase.Analytics.FirebaseAnalytics.ParameterScore, 42);

// Log an event with a string parameter.
Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventJoinGroup, Firebase.Analytics.FirebaseAnalytics.ParameterGroupId,
    "spoon_welders");

// Log an event with multiple parameters, passed as a struct:
Firebase.Analytics.Parameter[] LevelUpParameters = {
    new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterLevel, 5),
    new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, "mrspoon"),
    new Firebase.Analytics.Parameter("hit_accuracy", 3.14f)
};
    Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLevelUp, LevelUpParameters);
    */
}
#endif
