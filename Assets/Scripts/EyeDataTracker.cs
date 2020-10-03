using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using ViveSR.anipal.Eye;


/**
 * This class tracks the EyeData
 */
public class EyeDataTracker : MonoBehaviour
{
    //The vr camera
    public GameObject head;

    //The current eyedata
    private EyeData EyeData = new EyeData();

    //The world eye direction
    public Vector3 eyeDirection;
    //The world hitpoint is available, else the point 100 meter towards
    public Vector3 eyeHitpoint;
    //True if the given eye tracking points are valid and up-to-date
    public bool validTracking = false;

    private ViveSR.Error lastError;



    // Start is called before the first frame update
    void Start()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING)
        {
            Debug.LogError("SRanipal not running (Status " + SRanipal_Eye_Framework.Status + "). Trying to (re)initialise");
            var sranipal = SRanipal_Eye_Framework.Instance;
            if (sranipal == null)
            {
                //The Framework script should be included in the scene
                //If not, as a fallback, create a component
                Debug.LogWarning("SRanipal_Eye_Framework should be included in world.");
                sranipal = gameObject.AddComponent<SRanipal_Eye_Framework>();
            }
            sranipal.StartFramework();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var error = SRanipal_Eye_API.GetEyeData(ref EyeData);
        var newData = SRanipal_Eye.GetVerboseData(out EyeData.verbose_data);

        //No new data received from camera sensor - skip step
        if (!newData)
            return;

        //Show error only once and not every frame
        if (error != ViveSR.Error.WORK && lastError != error)
        {
            Debug.LogError("An error happened: " + error.ToString());
        }
        lastError = error;

        var leftEye = EyeData.verbose_data.left.gaze_direction_normalized;
        var rightEye = EyeData.verbose_data.right.gaze_direction_normalized;

        // if (leftEye != Vector3.zero && rightEye != Vector3.zero)
        // Debug.Log("Eyes: LEFT:=" + leftEye + ", RIGHT:=" + rightEye);


        if (leftEye != Vector3.zero)
        {
            this.validTracking = true;
            CalculateWorldSpace(leftEye);
        }
        else if (rightEye != Vector3.zero)
        {
            this.validTracking = true;
            CalculateWorldSpace(rightEye);
        }
        else
        {
            this.validTracking = false;
        }
    }

    /**
     * Calculates the world direction and updates the "LookAt" position
     */
    void CalculateWorldSpace(Vector3 direction)
    {// Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        //The direction from sr_anipal is in "raw" world space
        //So no translation or rotation of the head is taken in account
        //This will translate it to the location and rotation of the world space
        direction = head.transform.TransformDirection(direction);

        //The data we get from sr_anipal is wrongly coded
        //When looking up right, we get the direction up left
        //Instead of down left, we get down right
        //So we switch to 2d space (using x and z but not the height y)
        //Then we get the angle between the cameras lookat and the eye lookat
        //We negate the angle, multipliying by two (lookat -> center -> lookat) and
        // use the quaternion transform to get the "real" data.
        var eyeDirection2 = new Vector2(direction.x, direction.z);
        var headDirection2 = new Vector2(head.transform.forward.x, head.transform.forward.z);
        var correctedDirecion3 = Quaternion.AngleAxis(-2 * Vector2.SignedAngle(eyeDirection2, headDirection2),Vector3.up) * direction;
        direction = correctedDirecion3;



        this.eyeDirection = direction;


        RaycastHit hit;
        if (Physics.Raycast(head.transform.position, direction, out hit, Mathf.Infinity, layerMask))
        {
            //When hit collider: Use collider as hitpoint
            this.eyeHitpoint = hit.point;
            //Debug.Log("Hit:=" + hit.collider + "@" + hit.point);
        }
        else
        {
            //When not hit: Draw line 100 meters
            this.eyeHitpoint = direction.normalized * 100;
        }
    }

}
