using UnityEngine;
using System.Collections;

public class CameraFlow : MonoBehaviour {
    public struct CamMove
    {
        float min;
        float max;
        float distance;
        float speed;
    }
    // Target to look at
    private Transform TargetLookAt;
    public Transform TargetMain;
    public Transform TargetSub;

    public bool useTargetSub;

    // Camera distance variables
    public bool moveDistance = false;
    public float Distance = 10.0f;
    public float DistanceMin = 5.0f;
    public float DistanceMax = 15.0f;
    float startingDistance = 0.0f;
    public float desiredDistance = 0.0f;
    public float distanceSpeed = 1f;
    private float distanceDirection = 1f;
    // Mouse variables
    public float mouseX = 0.0f;
    public float mouseY = 0.0f;
    //public float X_MouseSensitivity = 5.0f;
    //public float Y_MouseSensitivity = 5.0f;
    public float MouseWheelSensitivity = 5.0f;

    // Axis limit variables
    //public float Y_MinLimit = 15.0f;
    //public float Y_MaxLimit = 70.0f;
    public float DistanceSmooth = 0.025f;
    float velocityDistance = 0.0f;
    Vector3 desiredPosition = Vector3.zero;
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;

    // Velocity variables
    float velX = 0.0f;
    float velY = 0.0f;
    float velZ = 0.0f;
    Vector3 position = Vector3.zero;

    public bool moveX = true;
    public float xMin = 0f;
    public float xMax = 80f;
    public float xSpeed = 1f;
    private float xDirection = 1f;

    public bool moveY = true;
    public float yMin = 0f;
    public float yMax = 80f;
    public float ySpeed = 1f;
    private float yDirection = 1f;

    public CameraPathAnimator camPathAnimator = null;

    #region MonoBehaviour

    void Start()
    {
        //Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        TargetLookAt = TargetMain;
        Distance = Vector3.Distance(TargetLookAt.transform.position, gameObject.transform.position);
        if (Distance > DistanceMax)
            DistanceMax = Distance;
        startingDistance = Distance;
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (useTargetSub && TargetLookAt != TargetSub)
        {
            TargetLookAt = TargetSub;
        }
        else if (!useTargetSub && TargetLookAt != TargetMain)
        {
            TargetLookAt = TargetMain;
        }
        if (moveX)
        {
            mouseX += Time.deltaTime * xSpeed * xDirection;
            if (mouseX < xMin)
                xDirection = 1f;
            else if (mouseX > xMax)
                xDirection = -1f;
        }
        if (moveY)
        {
            mouseY += Time.deltaTime * ySpeed * yDirection;
            if (mouseY < yMin)
                yDirection = 1f;
            else if (mouseY > yMax)
                yDirection = -1f;
        }
        if (moveDistance)
        {
            desiredDistance += distanceDirection * Time.deltaTime * distanceSpeed;
            if (desiredDistance < DistanceMin)
                distanceDirection = 1f;
            else if (desiredDistance > DistanceMax)
                distanceDirection = -1f;
        }

    }

    [System.Serializable]
    public struct Parameters
    {
        public float xmin;
        public float xmax;
        public float xspeed;
        public float ymin;
        public float ymax;
        public float yspeed;
        public float dmin;
        public float dmax;
        public float dspeed;
        public float startdistance;
    };

    public void StopPathAnimator()
    {
        if (camPathAnimator != null)
        {
            if (camPathAnimator.isPlaying)
            {
                camPathAnimator.Stop();
            }
            else
            {
                CameraPath cp = camPathAnimator.cameraPath.nextPath;
                if (cp != null)
                {
                    CameraPathAnimator cpa = cp.GetComponent<CameraPathAnimator>();
                    camPathAnimator = cpa;
                    if (cpa != null)
                    {
                        if (cpa.isPlaying)
                        {
                            cpa.Stop();
                        }
                    }
                }
            }
        }

    }

    public void PauseGameView(bool state)
    {
        if (state == true)
        {
            if (camPathAnimator != null)
            {
                if (camPathAnimator.isPlaying)
                {
                    camPathAnimator.Pause();
                    enabled = true;
                }
                else
                {
                    CameraPath cp = camPathAnimator.cameraPath.nextPath;
                    if (cp != null)
                    {
                        CameraPathAnimator cpa = cp.GetComponent<CameraPathAnimator>();
                        camPathAnimator = cpa;
                        if (cpa != null)
                        {
                            if (cpa.isPlaying)
                            {
                                cpa.Pause();
                                enabled = true;
                            }
                            else
                            {
                                camPathAnimator = null;
                                enabled = true;
                            }
                        }
                    }
                    else
                    {
                        camPathAnimator = null;
                        enabled = true;
                    }
                }
            }
        }
        else
        {
            if (camPathAnimator != null)
            {
                if (!camPathAnimator.isPlaying)
                {
                    camPathAnimator.Play();
                    enabled = false;
                }
            }
        }
    }

    public void SetParameters(Parameters param)
    {
        SetParameters(param.xmin, param.xmax, param.xspeed, param.ymin, param.ymax, param.yspeed,
            param.dmin, param.dmax, param.dspeed, param.startdistance);
    }

    public void SetParameters(
        float xmin, float xmax, float xspeed,
        float ymin, float ymax, float yspeed,
        float dmin, float dmax, float dspeed,
        float startdistance
        )
    {
        moveX = (xspeed == 0) ? false : true;
        xMin = xmin;
        xMax = xmax;
        xSpeed = xspeed;
        moveY = (yspeed == 0) ? false : true;
        yMin = ymin;
        yMax = ymax;
        ySpeed = yspeed;
        moveDistance = (dspeed == 0) ? false : true;
        DistanceMin = dmin;
        DistanceMax = dmax;
        distanceSpeed = dspeed;
        startingDistance = startdistance;
        Reset();
    }

    // LateUpdate is called after all Update functions have been called.
    void LateUpdate()
    {
        if (TargetLookAt == null)
            return;

        //HandlePlayerInput();

        CalculateDesiredPosition();

        UpdatePosition();
    }

    #endregion

    // ######################################################################
    // Player Input Functions
    // ######################################################################

    #region Component Segments
/*
    void HandlePlayerInput()
    {
        // mousewheel deadZone
        float deadZone = 0.01f;

        if (Input.GetMouseButton(0))
        {
            mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
        }

        // this is where the mouseY is limited - Helper script
        mouseY = ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

        // get Mouse Wheel Input
        if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
        {
            desiredDistance = Mathf.Clamp(Distance - (Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity),
                                                      DistanceMin, DistanceMax);
        }
    }
    */
    #endregion

    // ######################################################################
    // Calculation Functions
    // ######################################################################

    #region Component Segments

    void CalculateDesiredPosition()
    {
        // Evaluate distance
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velocityDistance, DistanceSmooth);

        // Calculate desired position -> Note : mouse inputs reversed to align to WorldSpace Axis
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        return TargetLookAt.position + (rotation * direction);
    }

    #endregion

    // ######################################################################
    // Utilities Functions
    // ######################################################################

    #region Component Segments

    // update camera position
    void UpdatePosition()
    {
        float posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        float posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        float posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);
        position = new Vector3(posX, posY, posZ);

        transform.position = position;

        transform.LookAt(TargetLookAt);
    }

    // Reset Mouse variables
    void Reset()
    {
        mouseX = xMin;
        mouseY = yMin;
        Distance = startingDistance;
        desiredDistance = Distance;
    }

    // Clamps angle between a minimum float and maximum float value
    float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360.0f || angle > 360.0f)
        {
            if (angle < -360.0f)
                angle += 360.0f;
            if (angle > 360.0f)
                angle -= 360.0f;
        }

        return Mathf.Clamp(angle, min, max);
    }

    #endregion
}
