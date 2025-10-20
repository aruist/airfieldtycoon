using UnityEngine;
using UnityEngine.EventSystems;

public class SCScaleFactor : MonoBehaviour {
    public Canvas myCanvas;
    public EventSystem myEventSystem;

    // Use this for initialization
    void Awake () {
        if (myCanvas.scaleFactor >= 1)
            myEventSystem.pixelDragThreshold = (int)(5 * myCanvas.scaleFactor);
        else
            myEventSystem.pixelDragThreshold = 5;

    }

}
