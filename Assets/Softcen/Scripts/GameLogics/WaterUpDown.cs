using UnityEngine;
using System.Collections;

public class WaterUpDown : MonoBehaviour {
    public float speed = 1f;
    public float yMin;
    public float yMax;
    private Transform m_tr;
    private float m_direction;
    private Vector3 m_pos;
	// Use this for initialization
	void Start () {
        m_tr = transform;
        m_direction = 1f;
        m_pos = m_tr.position;
	}
	
	// Update is called once per frame
	void Update () {
        m_pos.y += Time.deltaTime * m_direction * speed;
	    if (m_direction > 0f && m_pos.y >= yMax)
        {
            m_direction = -1f;
        }
        else if (m_direction < 0f && m_pos.y <= yMin)
        {
            m_direction = 1f;
        }
        m_tr.position = m_pos;
	}
}
