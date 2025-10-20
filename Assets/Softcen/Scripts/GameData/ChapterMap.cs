using UnityEngine;

public class ChapterMap : MonoBehaviour {
    public TextMesh tmTitle;
    public TextMesh tmSubTitle;
    public GameObject goSphere;
    public GameObject goLighthouse;
    public CameraFlowData camFlowData;
    public Chapters.Id Id;
    public Item.Identifications itemId;

    public GameObject[] goSelected;
    public GameObject[] goDone;

    public ChapterInfo chapterInfo;

    public enum Mode
    {
        Selected,
        Done,
        Inactive
    }
    void Start()
    {
        /*
        if (GameManager.Instance != null)
        {
        if (GameManager.Instance.playerData.CurrentChapter == (int)Id 
                || GameManager.Instance.playerData.CurrentChapter == 0 && Id == Chapters.Id.Chapter1)
        {
                SetSelected(true);
            }
        else
        {
                SetSelected(false);
            }
        }
        SetDone(false);*/

    }

    public void SetSelected(Mode mode, Color titleColor)
    {
#if SOFTCEN_DEBUG
        Debug.Log("SetSelected go: " + gameObject.name + ", mode: " + mode.ToString());
#endif
        bool state = false;
        if (mode == Mode.Selected)
            state = true;

        for (int i=0; i < goSelected.Length; i++)
        {
            goSelected[i].SetActive(state);
        }

        if (mode == Mode.Inactive)
        {
            tmTitle.gameObject.SetActive(false);
            tmTitle.color = titleColor;
        }
        else
        {
            tmTitle.gameObject.SetActive(true);
            tmTitle.color = titleColor;
        }

        // Done:
        if ((int)Id < GameManager.Instance.playerData.CurrentChapter ||
            ((int)Id == GameManager.Instance.playerData.CurrentChapter && GameManager.Instance.playerData.CurrentPhase >= 4))
        {
            for (int i = 0; i < goDone.Length; i++)
            {
                goDone[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < goDone.Length; i++)
            {
                goDone[i].SetActive(false);
            }
        }
    }

    public void SetDone(bool state)
    {
        for (int i = 0; i < goDone.Length; i++)
        {
            goDone[i].SetActive(state);
        }
    }

}
