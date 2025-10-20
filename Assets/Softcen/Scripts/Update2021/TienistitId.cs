public class TienistitId
{
    public static string unity_interstialId() {
        return "video";
    }
    public static string unity_bannerId() {
        return "banner";
    }
    public static string unity_rewardId() {
        return "rewardedVideo";
    }
    public static string unity_gameId()
    {
#if UNITY_ANDROID
        return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.level1data); // UNITY ADS ID
#elif UNITY_IOS
        return M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.level2data); // UNITY ADS ID
#else
        return "unexpected_platform";
#endif
    }

    // INTERSTITIAL
    public static string gp_interstitialId()
    {
#if AD_DEBUG && UNITY_ANDROID
        return "ca-app-pub-3940256099942544/1033173712"; // Test Ad Id Android
#elif AD_DEBUG && UNITY_IOS
        return "ca-app-pub-3940256099942544/4411468910"; // Test Ad Id IOS
#elif UNITY_IOS || UNITY_ANDROID
        return GameConsts.amhix2wi;
#else
        return "unexpected_platform";
#endif
    }

    // REWARD
    public static string gp_rewardId() {
#if AD_DEBUG && UNITY_ANDROID
        return "ca-app-pub-3940256099942544/5224354917"; // Test Ad Id Android
#elif AD_DEBUG && UNITY_IOS
        return "ca-app-pub-3940256099942544/1712485313"; // Test Ad Id IOS
#elif UNITY_ANDROID || UNITY_IOS
        return GameConsts.ams3fgsfs;
#else
        return "unexpected_platform";
#endif
    }

    // BANNER
    public static string gp_bannerId()
    {
#if UNITY_ANDROID || UNITY_IOS
        return GameConsts.amhix2wb;
#else
        return "unexpected_platform";
#endif
    }

}
