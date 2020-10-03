using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

/**
 * This class contains the logic for recording and storing data
 */
public class Recorder : MonoBehaviour
{
    private RecordData recordData = new RecordData();

    private EntityRecorder[] recorder;
    //The recording state. Defaults to recording on start
    public RecorderState state = RecorderState.RECORD;

    //Tracks the Eye data
    public bool trackEyeData = true;
    public bool saveOnDestroy = true;

    public GameObject vrCamera;


    private int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        frame = 0;
        loadRecorder();
        ReloadState();
    }

    private void OnDestroy()
    {
        if (saveOnDestroy && state == RecorderState.RECORD)
            save();
    }

    /**
     * Sets a new state. When recording stops the data gets saved
     * */
    public void SetState(RecorderState state)
    {
        if (this.state == RecorderState.RECORD)
        {
            this.save();
        }
        this.state = state;
        this.ReloadState();
    }

    public RecordData GetData()
    {
        return this.recordData;
    }

    /**
     * Load all recorders. Gets called on startup and manuelly by each client which want to join or left the recording
     * */
    [Obsolete]
    public void loadRecorder()
    {
        this.recorder = FindObjectsOfTypeAll(typeof(EntityRecorder)) as EntityRecorder[];
    }

    /**
     * FixedUpdate - gets called 50 times a second
     * */
    void FixedUpdate()
    {
        switch (this.state)
        {
            case RecorderState.RECORD:
                createSnapshot();
                break;
            case RecorderState.PLAY:
                playback();
                break;
        }
        frame++;
    }

    /**
     * Reloads the recording and playback setup
     * */
    public void ReloadState()
    {
        this.frame = 0;
        vrCamera.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Both; // See startPlayback

        switch (this.state)
        {
            case RecorderState.RECORD:
                startRecording();
                break;
            case RecorderState.PLAY:
                startPlayback();
                break;
            case RecorderState.NONE:
                //ignore
                break;
        }

    }

    void startPlayback()
    {
        Debug.Log("Starting Playback");
        //This deactivates the postional and rotational tracking of the HMD
        // While the position can be disabled with the following command...
        //  UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        // ... rotation is quite a problem with unity and steamvr
        // So the best and "cleanest" variant is to disconnect the camera from the Headset eye system while playback
        vrCamera.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.None;

    }
    void startRecording()
    {
        Debug.Log("Starting Recording");
    }

    /**
     * */
    void playback()
    {
        var currentFrame = Math.Min(frame, this.recordData.snapshots.Count - 1);
        var snapshot = this.recordData.snapshots[currentFrame];

        if (trackEyeData)
        {
            var eyeDataTracker = GetComponent<EyeDataTracker>();
            eyeDataTracker.eyeHitpoint = snapshot.eyeHitpoint;
            eyeDataTracker.eyeDirection = snapshot.eyeDirection;
            eyeDataTracker.validTracking = snapshot.eyeValid;

        }

        foreach (var record in recorder)
        {
            snapshot.positions.FindAll(e => e.id == record.id).ForEach(delegate (PositionData entry)
            {
                record.gameObject.SetActive(entry.active);
                record.transform.position = entry.position;
                record.transform.rotation = Quaternion.Euler(entry.rotation);
            });
        }
    }

    void createSnapshot()
    {
        this.recordData.scene = SceneManager.GetActiveScene().name;
        var snapshot = new Snapshot();
        if (trackEyeData)
        {
            var eyeDataTracker = GetComponent<EyeDataTracker>();
            snapshot.eyeHitpoint = eyeDataTracker.eyeHitpoint;
            snapshot.eyeDirection = eyeDataTracker.eyeDirection;
            snapshot.eyeValid = eyeDataTracker.validTracking;
        }
        foreach (var record in recorder)
        {
            if (record.enabled && record.recordEnabled)
            {
                var data = new PositionData();
                data.id = record.id;
                data.active = record.enabled;
                if (record.recordPosition)
                {
                    data.position = record.transform.position;
                }
                if (record.recordRotation)
                {
                    data.rotation = record.transform.rotation.eulerAngles;
                }
                snapshot.positions.Add(data);
            }
            record.events.ForEach(s => snapshot.events.Add(s));
            record.events.Clear();
        }
        this.recordData.snapshots.Add(snapshot);
        if (this.recordData.snapshots.Count % 1000 == 0)
            Debug.Log("Recorded " + this.frame + " Frames");
    }


    /**
     * Saves the data to file
     * */
    public void save()
    {
        var json = JsonUtility.ToJson(this.recordData, true);
        var filename = "data_" + SceneManager.GetActiveScene().name + "_" + this.recordData.recordingStart + ".json";
        File.WriteAllText(filename, json);
        Debug.Log("Saved data to '" + filename + "'");
    }

    
    /**
     * Loads data from a file
     * */
    public void load(string path)
    {
        Debug.Log("Loading playback data from '" + path + "'");
        var data = File.ReadAllText(path);
        this.recordData = JsonUtility.FromJson<RecordData>(data);
        this.startPlayback();
    }

}
