using UnityEngine;

public class Chapters : MonoBehaviour
{
    public enum Id
    {
        None = 0,
        Chapter1,
        Chapter2,
        Chapter3,
        Chapter4,
        Chapter5,
        Chapter6,
        Chapter7,
        Chapter8,
        Length
    }
    public ChapterInfo[] chapterInfo;
    public static int MaxChapter
    {
        get { return (int)Id.Chapter8; }
    }

    public Sprite GetChapterSprite(int chapterId)
    {
        for (int i = 0; i < chapterInfo.Length; i++)
        {
            if ((int)chapterInfo[i].Id == chapterId)
            {
                return chapterInfo[i].chapterSprite;
            }
        }
        return null;
    }

    public string GetChapterName(int chapterId)
    {
        for (int i=0; i < chapterInfo.Length; i++)
        {
            if ((int)chapterInfo[i].Id == chapterId)
            {
                return chapterInfo[i].ChapterName;
            }
        }
        return "";
    }

    public ChapterInfo GetChapterInfo(int chapterId)
    {
        for (int i = 0; i < chapterInfo.Length; i++)
        {
            if ((int)chapterInfo[i].Id == chapterId)
            {
                return chapterInfo[i];
            }
        }
        return null;

    }

    public int GetChapterStartLevel(int chapterId)
    {
        for (int i = 0; i < chapterInfo.Length; i++)
        {
            if ((int)chapterInfo[i].Id == chapterId)
            {
                return chapterInfo[i].startLevel;
            }
        }
        return 0;
    }

}
