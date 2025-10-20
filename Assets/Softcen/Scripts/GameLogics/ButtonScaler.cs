using UnityEngine;
using System.Collections;

public class ButtonScaler : MonoBehaviour {
    public float speed = 1f;
    public Vector3 minSize = new Vector3(0.9f, 0.9f, 0.9f);
    public Vector3 maxSize = new Vector3(1.1f, 1.1f, 1.1f);

    private Vector3 m_size;
    private bool m_up = false;
    private Transform tr;
	// Use this for initialization
	void Start () {
        tr = transform;
        m_size = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
	    if (m_up)
        {
            m_size.x += speed * Time.deltaTime;
            m_size.y += speed * Time.deltaTime;
            if (m_size.x >= maxSize.x)
            {
                m_up = false;
            }
        }
        else
        {
            m_size.x -= speed * Time.deltaTime;
            m_size.y -= speed * Time.deltaTime;
            if (m_size.x <= minSize.x)
            {
                m_up = true;
            }
        }
        tr.localScale = m_size;
    }
}
