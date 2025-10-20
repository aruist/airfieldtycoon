using UnityEngine;
using System.Collections;

public class ObjectRotateAround : MonoBehaviour {
    public float m_Speed = 1f;
    public Vector3 m_Direction;

	
	// Update is called once per frame
	void Update () {
        transform.Rotate(m_Direction * m_Speed * Time.deltaTime);
    }
}
