using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Minimizes the angle in TEST 03 every time the trigger is entered
 */
public class AngleMinizer : MonoBehaviour
{

    public GameObject headCollider;
    public GameObject deactivatedObject;
    public GameObject trigger;
    public float angleDiff = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == headCollider)
        {
            if (deactivatedObject.active)
            {
                Debug.LogError("Object not deactivated - continue");
            }
            deactivatedObject.SetActive(true);
            if (trigger.GetComponent<EyeInvisibilitySwitcher>().angle > this.angleDiff)
                trigger.GetComponent<EyeInvisibilitySwitcher>().angle -= this.angleDiff;
            Debug.Log("Resettet object, set angle to " + trigger.GetComponent<EyeInvisibilitySwitcher>().angle);
        }
    }
}
