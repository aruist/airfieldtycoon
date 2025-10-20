using UnityEngine;
using System.Collections;

public class AndroidAmazonDestroy : MonoBehaviour {

	void Awake ()
    {
#if !SOFTCEN_AMAZON
        Destroy(gameObject);
#endif       
    }
	
}
