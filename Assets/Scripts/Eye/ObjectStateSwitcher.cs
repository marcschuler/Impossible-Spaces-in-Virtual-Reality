using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Activates or deactivates an object on trigger enter
 */
public class ObjectStateSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject headCollider;

    public GameObject activate;
    public GameObject deactivate;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != headCollider)
            return;

        if (activate != null)
        {
            Debug.Log("Activating " + activate.name);
            activate.SetActive(true);
        }
        if (deactivate != null)
        {
            Debug.Log("Deactivating " + activate.name);
            deactivate.SetActive(false);
        }
    }
}
