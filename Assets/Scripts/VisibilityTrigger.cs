using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Triggers the visibility between two given object
 */
public class VisibilityTrigger : MonoBehaviour
{
    //When true, activate the trigger on startup
    public bool runOnStartup = false;
    //The camera collider which enters the trigger
    public GameObject camera;
    //The object to enable
    public GameObject enableObject;
    //The object to disable
    public GameObject disableObject;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Using VisibilityTrigger '" + this.name + "' in scene");
        if (this.runOnStartup)
            switchVisibility();
    }

    //Switch visibility on trigger enter
    private void OnTriggerEnter(Collider other)
    {
     
        Debug.Log("Player entering trigger '" + this.name + "' with '" + other.name + "'");
        if (other.gameObject != this.camera)
            return;
        switchVisibility();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != this.camera)
            return;
        Debug.Log("Player '" + other.name + "' exiting trigger '" + this.name + "'");
    }

    /**
     * Enables the first object, disables the second
     */
    public void switchVisibility()
    {
        Debug.Log("Enabling " + (enableObject != null ? enableObject.name : "(null)")
            + ", Disabling " + (disableObject != null ? disableObject.name : "(null)"));
        if (enableObject != null)
            enableObject.SetActive(true);

        if (disableObject != null)
            disableObject.SetActive(false);
    }
}
