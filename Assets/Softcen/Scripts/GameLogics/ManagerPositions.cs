using UnityEngine;
using System.Collections;

public class ManagerPositions : MonoBehaviour {
    public CameraPath startupCamPath;

    public void SetPositionAndCamFlow(BusinessmanControl businessmanControl, CameraFlow camFlow, int phase)
    {
        CameraPathAnimator camPathAnimator = null;

        ManagerTransform selectedManagerTr = null;
        camFlow.StopPathAnimator();

        if (businessmanControl != null && camFlow != null )
        {            
            Transform newTr = null;
            //CameraFlowData camFlowData = null;
            int maxPhase = 0;
            for (int i=0; i < transform.childCount; i++) 
            {
                ManagerTransform mt = transform.GetChild(i).GetComponent<ManagerTransform>();
                if (mt != null)
                {
                    if (mt.availableOnPhase <= phase)
                    {
                        if (maxPhase <= mt.availableOnPhase)
                        {
                            maxPhase = mt.availableOnPhase;
                            newTr = mt.transform;
                            //camFlowData = mt.camFlowData;
                            selectedManagerTr = mt;
                        }
                    }
                }
            }
#if SOFTCEN_DEBUG
            if (newTr != null)
            {
                Debug.Log("ManagerPositions SetPositionAndCamFlow phase: " + maxPhase
                    + ", Chapter: " + GameManager.Instance.playerData.CurrentChapter
                    + ", Phase: " + GameManager.Instance.playerData.CurrentPhase
                    + ", Level: " + GameManager.Instance.playerData.Level
                    + ", newTr: " + newTr.name
                    , newTr.gameObject
                    );
            }
            else
            {
                Debug.LogWarning("ManagerPositions SetPositionAndCamFlow NOTSET! phase: " + maxPhase);
            }
#endif
            if (selectedManagerTr != null)
            {
                if (selectedManagerTr.camPath != null)
                {
                    camFlow.enabled = false;
                    if (startupCamPath != null)
                    {
                        camPathAnimator = startupCamPath.GetComponent<CameraPathAnimator>();
                        startupCamPath.nextPath = selectedManagerTr.camPath;
                        CameraPathAnimator cpa = startupCamPath.nextPath.GetComponent<CameraPathAnimator>();
                        if (cpa != null)
                        {
                            cpa.animationObject = camFlow.transform;
                            startupCamPath.interpolateNextPath = true;
                        }

                    }
                    else
                    {
                        camPathAnimator = selectedManagerTr.camPath.GetComponent<CameraPathAnimator>();
                        if (!selectedManagerTr.camPath.gameObject.activeSelf)
                            selectedManagerTr.camPath.gameObject.SetActive(true);

                        if (selectedManagerTr.camPath.nextPath != null)
                        {
                            CameraPathAnimator cpa = selectedManagerTr.camPath.nextPath.GetComponent<CameraPathAnimator>();
                            if (cpa != null)
                                cpa.animationObject = camFlow.transform;
                        }
                    }

                    camFlow.camPathAnimator = camPathAnimator;

                    if (camPathAnimator != null)
                    {
                        camPathAnimator.animationObject = camFlow.transform;
                        camPathAnimator.Play();

                    }
                    if (newTr != null)
                    {
                        businessmanControl.UpdatePosition(newTr);
                    }
                    return;
                }
            }
            if (newTr  != null)
            {
                camFlow.camPathAnimator = null;

                businessmanControl.UpdatePosition(newTr);
                camFlow.TargetMain = businessmanControl.trCamTarget;
                //camFlow.SetParameters(camFlowData.camFlowParams);
                camFlow.enabled = true;
            }
        }
    }
}
