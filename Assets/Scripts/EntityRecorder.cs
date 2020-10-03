using System.Collections.Generic;
using UnityEngine;

/**
 * This class should be extended to every entity which should be recorded
 */
public class EntityRecorder : MonoBehaviour
{

    public string id;                   //The unique ID for recording OR playback

    public bool recordPosition = true;  //Record the position
    public bool recordRotation = true;  //Record the rotation

    public bool recordEnabled = true;   //Use when recording (like VR-Camera)
    public bool playbackEnabled = true; //Use when playback (like normal camera)

    public List<string> events = new List<string>();



    // Start is called before the first frame update
    void Start()
    {
     
        GetRecord().loadRecorder();  //Force reload of data

    }

    private Recorder GetRecord()
    {
        var records = FindObjectsOfType(typeof(Recorder)) as Recorder[];
        if (records.Length == 0)
        {
            Debug.LogError("No Record found in scene");
            return null;
        }
        else if (records.Length == 1)
        {
            return records[0];
        }
        else
        {
            Debug.LogError("More than one Record found in scene");
            return records[0];
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    } 

    public void addEvent(string name)
    {
        Debug.Log("Event '" + name + "' started");
        this.events.Add(name);
    }
}
