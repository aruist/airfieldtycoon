using UnityEngine;

public class ItemCount{

    public int id;
    public int _count;
    public int count {
        get { return (3275 ^ _count)-2317; }
        set { _count = (value+2317) ^ 3275; }
    }

    public ItemCount() {
        id = -1;
        count = 0;
    }

    public ItemCount(int varId, int varCount) {
        id = varId;
        count = varCount;
    }
}
