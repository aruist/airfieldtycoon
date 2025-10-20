using UnityEngine;
using Beebyte.Obfuscator;

public class StoreDlg : MonoBehaviour {
    public GameObject goRestorePurchases;

    public Sprite[] ShopSprites;

    public StoreButton[] storeButtons;

	// Use this for initialization
	void Start () {
#if UNITY_ANDROID
        goRestorePurchases.SetActive(false);
#else
        goRestorePurchases.SetActive(true);
#endif
        storeButtons[0].SetStoreId(Kauppa.ID.INN_APP_FISTFUL_OF_GEMS);
        storeButtons[1].SetStoreId(Kauppa.ID.INN_APP_POUCH_OF_GEMS);
        storeButtons[2].SetStoreId(Kauppa.ID.INN_APP_BUCKET_OF_GEMS);
        storeButtons[3].SetStoreId(Kauppa.ID.INN_APP_BARREL_OF_GEMS);
        storeButtons[4].SetStoreId(Kauppa.ID.INN_APP_AD_FREE);
        storeButtons[5].SetStoreId(Kauppa.ID.INN_APP_AUTO_TAP);
    }

    [SkipRename]
    public void RestorePurchases()
    {
        if (Kauppa.Instance != null)
        {
            Kauppa.Instance.RestorePurchases();
        }
    }
}
