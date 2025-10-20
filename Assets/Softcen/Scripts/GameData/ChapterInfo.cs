using UnityEngine;

public class ChapterInfo : MonoBehaviour {
    public Chapters.Id Id;
    public string ChapterName;
    public string ChapterDescription;
    public Sprite chapterSprite;
    public int startLevel;
    //public ChapterMap chapterMap;
    
    void Start()
    {
        /*if (chapterMap != null)
        {
            chapterMap.tmTitle.text = ChapterName;
            if (chapterMap.tmSubTitle != null)
                chapterMap.tmSubTitle.text = ChapterDescription;
        }*/
    }
}
