using UnityEngine;
using System.IO;
using System.Collections;

public class ItemsPreviewManager : MonoBehaviour {
    public Transform trTargetPos;
    private BonusManager bonusManager;
    public SCScreenCapture screenCapture;
    public int startIndex;
    public int endIndex;
    private itemType mode;
    public int currentIndex;
    public GameObject currentGo;
    private int m_maxIndex;
    public Camera cam;
    public string filename;
    public string path;

    private enum itemType
    {
        Level,
        Idle,
        Tap
    }


	// Use this for initialization
	void Start () {
        Debug.Log("I=Idle, L=Level, T=Tap, +=Next, -=Prev");
        bonusManager = BonusManager.Instance;
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyUp(KeyCode.L))
        {
            Debug.Log("<color=yellow>Level Items</color>");
            mode = itemType.Level;
            currentIndex = 0;
            m_maxIndex = bonusManager.levelItemList.Count - 1;
            SetCurrentItem();
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            Debug.Log("<color=yellow>Idle Items</color>");
            mode = itemType.Idle;
            currentIndex = 0;
            m_maxIndex = bonusManager.idleItemList.Count - 1;
            SetCurrentItem();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            Debug.Log("<color=yellow>Tap Items</color>");
            mode = itemType.Tap;
            currentIndex = 0;
            m_maxIndex = bonusManager.tapItemList.Count - 1;
            SetCurrentItem();
        }

        if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            //Debug.Log("Plus pressed");
            currentIndex = Mathf.Min(currentIndex + 1, m_maxIndex);
            SetCurrentItem();
        }
        if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus))
        {
            //Debug.Log("Minus pressed");
            currentIndex = Mathf.Max(0, currentIndex - 1);
            SetCurrentItem();
        }
    }

    private void SetCurrentItem()
    {
        screenCapture.storePath = path;
        if (currentGo != null)
        {
            Destroy(currentGo);
            currentGo = null;
        }

        if (mode == itemType.Idle)
        {
            if (currentIndex < bonusManager.idleItemList.Count)
            {
                if (bonusManager.idleItemList[currentIndex].goPrefab != null)
                {
                    currentGo = (GameObject)Instantiate(bonusManager.idleItemList[currentIndex].goPrefab);
                    filename = bonusManager.idleItemList[currentIndex].Id.ToString();
                    Debug.Log("<color=yellow>Idle filename: " + filename + "</color>");

                }
            }

        }
        else if (mode == itemType.Level)
        {
            if (currentIndex < bonusManager.levelItemList.Count)
            {
                if (bonusManager.levelItemList[currentIndex].goPrefab != null)
                {
                    if (currentGo != null)
                    {
                        Destroy(currentGo);
                    }
                    currentGo = (GameObject)Instantiate(bonusManager.levelItemList[currentIndex].goPrefab);
                    filename = bonusManager.levelItemList[currentIndex].Id.ToString();
                    Debug.Log("<color=yellow>Level filename: " + filename + "</color>");
                }
            }

        }
        else if (mode == itemType.Tap)
        {
            if (currentIndex < bonusManager.tapItemList.Count)
            {
                if (bonusManager.tapItemList[currentIndex].goPrefab != null)
                {
                    if (currentGo != null)
                    {
                        Destroy(currentGo);
                    }
                    currentGo = (GameObject)Instantiate(bonusManager.tapItemList[currentIndex].goPrefab);
                    filename = bonusManager.tapItemList[currentIndex].Id.ToString();
                    Debug.Log("<color=yellow>Tap filename: " + filename + "</color>");
                }
            }

        }
        else
        {
            return;
        }

        if (currentGo != null)
        {
            screenCapture.filename = filename;
            currentGo.transform.localPosition = Vector3.zero;
            currentGo.SetActive(true);
            SkinnedMeshRenderer smr = currentGo.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                trTargetPos.localPosition = smr.bounds.center;
                return;
            }
            MeshRenderer mr = currentGo.GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                trTargetPos.localPosition = mr.bounds.center;
            }
        }
    }


}
