
public class SCGiftItem
{
    private const int m_version = 1;
    public int id;
    public long startTime;
    public long duration;
    public int cloudVersion;
    public int version {  get { return m_version;  } }

    public SCGiftItem()
    {
        id = -1;
        startTime = 0;
        duration = 0;
        cloudVersion = 0;
    }
}
