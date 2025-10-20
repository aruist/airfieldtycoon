using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]

public class SCUpdateBounds : MonoBehaviour {
    public SkinnedMeshRenderer smr;
    public Vector3 smrAdjust;
    public Vector3 smrSize;

    // Use this for initialization
    void Start () {
        if (smr == null)
            smr = GetComponent<SkinnedMeshRenderer>();
        UpdateBounds();
    }


    private void UpdateBounds()
    {
        if (smr != null)
        {
            Vector3 center = transform.position + smrAdjust;
            Bounds b = new Bounds(center, smrSize);
            smr.localBounds = b;
        }
    }

#if SOFTCEN_DEBUG
    void OnDrawGizmosSelected()
    {
        //Vector3 center = smr.bounds.center;
        //float radius = smr.bounds.. extents.magnitude;
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + smrAdjust, smrSize);
        //Gizmos.DrawCube(smr.bounds.center, smr.bounds.size);
        //Gizmos.DrawWireSphere(center, radius);
    }
#endif

}
