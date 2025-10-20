using UnityEngine;

public class LookAtCamera : MonoBehaviour {
    Camera m_cam;
	// Use this for initialization
	void Start () {
        m_cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.LookRotation(-m_cam.transform.forward);
    }
}
