using UnityEngine;
using System.Collections;

public class MapCloud : MonoBehaviour {
    public Vector3 moveVector;
    public float speedMin;
    public float speedMax;
    public float heightMin;
    public float heightMax;
    public float yMin;
    public float yMax;
    public float xMin;
    public float xMax;
    public float sizeXMin;
    public float sizeXMax;
    public float sizeYMin;
    public float sizeYMax;

    public float m_speed;

    private Transform tr;
    private Vector3 m_pos;

	// Use this for initialization
	void Start () {
        tr = transform;
        m_pos = tr.localPosition;
        m_pos.x = Random.Range(xMin, xMax);
        NewStartPosition();
    }
	
	// Update is called once per frame
	void Update () {
        tr.Translate(moveVector * m_speed * Time.deltaTime);
        if (tr.localPosition.x < xMin)
        {
            m_pos.x = xMax;
            NewStartPosition();
        }
	}

    private void NewStartPosition()
    {
        Vector3 size = tr.localScale;
        size.x = Random.Range(sizeXMin, sizeXMax);
        size.y = Random.Range(sizeYMin, sizeYMax);
        tr.localScale = size;

        m_speed = Random.Range(speedMin, speedMax);
        m_pos.y = Random.Range(heightMin, heightMax);
        m_pos.z = Random.Range(yMin, yMax);
        tr.localPosition = m_pos;
    }
}
