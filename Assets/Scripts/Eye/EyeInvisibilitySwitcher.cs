using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Disables an object in sight
 */
public class EyeInvisibilitySwitcher : MonoBehaviour
{
    public GameObject eyeDataObject; //the object to enable/disable
    public GameObject headCollider; //the collider object
    public GameObject vrCamera;     //the camera

    public GameObject eyeObject;
    public float angle; //The minimum euler angle to hide the object

    private bool inTrigger = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!inTrigger)
            return;
        var eyeData = eyeDataObject.GetComponent<EyeDataTracker>();
        Vector3 eyeDirection = eyeData.eyeDirection.normalized;
        Vector3 objectDirection = (eyeObject.transform.position - vrCamera.transform.position).normalized;
        float calculatedAngle = Vector3.Angle(eyeDirection, objectDirection);   //The angle as [0,180]
        //To display to angle: Debug.Log(calculatedAngle);
        if (angle <= calculatedAngle && eyeObject.active)
        {
            eyeObject.SetActive(false);
            Debug.Log("Deactivated " + eyeObject.name + " at " + angle + " (of allowed " + this.angle + ") Degree");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //TODO check player on trigger
        if (other.gameObject == headCollider) { 
            this.inTrigger = true;
            Debug.Log("EyeInvSwitcher active");
        }
    }

    void OnTriggerExit(Collider other)
    {
        //TODO check player on trigger
        if (other.gameObject == headCollider)
        {
            this.inTrigger = false;
            Debug.Log("EyeInvSwitcher deactivated");
        }
    }
}
