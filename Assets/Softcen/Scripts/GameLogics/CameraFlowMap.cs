using UnityEngine;
using System.Collections;

public class CameraFlowMap : MonoBehaviour {
    public float delta = 0.1f;
    public Transform trTarget;
    public Vector3 MinDelta;
    public Vector3 MaxDelta;
    public Vector3 SmoothTime;

    private Transform m_currentTarget;
    private Vector3 m_pos;
    private Transform tr;

    public Vector3 TargetPos = new Vector3(0, 0, 0);
    public Vector3 TargetCompare = new Vector3(0, 0, 0);
    public Vector3 Velocity = new Vector3(0, 0, 0);


    // Use this for initialization
    void Start () {
        tr = transform;
        SetTarget();
    }

    // Update is called once per frame
    void Update () {
	    if (m_currentTarget != trTarget)
        {
            // Change Target
            SetTarget();
        }

        m_pos.x = Mathf.SmoothDamp(tr.position.x, TargetPos.x, ref Velocity.x, SmoothTime.x);
        m_pos.y = Mathf.SmoothDamp(tr.position.y, TargetPos.y, ref Velocity.y, SmoothTime.y);
        m_pos.z = Mathf.SmoothDamp(tr.position.z, TargetPos.z, ref Velocity.z, SmoothTime.z);

        CheclLimits();

        tr.position = m_pos;
    }

    private void CheclLimits()
    {
        if (TargetPos.y > MaxDelta.y && m_pos.y >= MaxDelta.y)
        {
            TargetPos.y = trTarget.position.y - Mathf.Abs(MinDelta.y) - delta;
        }
        else if (TargetPos.y < MinDelta.y && m_pos.y <= MinDelta.y)
        {
            TargetPos.y = trTarget.position.y + MaxDelta.y + delta;
        }

        if (m_pos.x > trTarget.position.x && m_pos.x >= TargetCompare.x)
        {
            TargetCompare.x = trTarget.position.x - Mathf.Abs(MinDelta.x);
            TargetPos.x = TargetCompare.x - delta;
        }
        else if (m_pos.x < trTarget.position.x && m_pos.x <= TargetCompare.x)
        {
            TargetCompare.x = trTarget.position.x + MaxDelta.x;
            TargetPos.x = TargetCompare.x + delta;
        }

        if (m_pos.z > trTarget.position.z && m_pos.z >= TargetCompare.z)
        {
            TargetCompare.z = trTarget.position.z - Mathf.Abs(MinDelta.z);
            TargetPos.z = TargetCompare.z - delta;
        }
        else if (m_pos.z < trTarget.position.z && m_pos.z <= TargetCompare.z)
        {
            TargetCompare.z = trTarget.position.z + MaxDelta.z;
            TargetPos.z = TargetCompare.z + delta;
        }
    }

    private void SetTarget()
    {
        if (trTarget == null)
            return;

        Velocity.x = 0f;
        Velocity.y = 0f;
        Velocity.z = 0f;

        m_pos = trTarget.position;
        m_pos.y += MinDelta.y;
        TargetCompare.y = m_pos.y + MaxDelta.y;
        TargetPos.y = TargetCompare.y + delta;
        TargetCompare.x = m_pos.x + MaxDelta.x;
        TargetPos.x = TargetCompare.x + delta;
        TargetCompare.z = m_pos.z + MaxDelta.z;
        TargetPos.z = TargetCompare.z + delta;
        /*
        if (m_currentTarget != null)
        {
            m_currentTarget.GetComponent<ChapterMap>().SetSelected(false);
        }*/
        m_currentTarget = trTarget;
        
        tr.position = m_pos;
        //trTarget.GetComponent<ChapterMap>().SetSelected(true);
    }



}
