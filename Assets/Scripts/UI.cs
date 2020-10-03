using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The UI class
 */
[RequireComponent(typeof(EyeDataTracker))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Recorder))]
public class UI : MonoBehaviour
{
    // When true, shows the eye line
    public bool showEyeLine = false;
    public bool showLookAt = false;

    //The VR head object (most likely the VR camera)
    public GameObject head;

    //Optional: The look at visualizer
    public GameObject lookAtVisualizer;
    //The material for a tracked eye line
    public Material trackMaterial;
    //The material for an error or outdated track line
    public Material errorMaterial;

    //The components
    private EyeDataTracker eyeDataTracker;
    private LineRenderer lineRenderer;
    private Recorder recorder;

    // Start is called before the first frame update
    void Start()
    {
        this.eyeDataTracker = GetComponent<EyeDataTracker>();
        this.lineRenderer = GetComponent<LineRenderer>();
        this.recorder = GetComponent<Recorder>();

        this.lineRenderer.enabled = this.showEyeLine;
        //If the local space is used, the line would move
        //with the object (RecordedVrPlayer) and one couldn't move
        //the object without distorting the line rendering
        this.lineRenderer.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        //F3 -> Show eyeline
        if (Input.GetKeyDown(KeyCode.F3))
        {
            this.showEyeLine = !this.showEyeLine;
            Debug.Log("UI EyeLine " + (this.showEyeLine ? "en" : "dis") + "abled");
            this.lineRenderer.enabled = this.showEyeLine;
        }
        //F4 -> Show LookAt
        if (Input.GetKeyDown(KeyCode.F4))
        {
            this.showLookAt = !this.showLookAt;
            Debug.Log("UI LookAt " + (this.showLookAt ? "en" : "dis") + "abled");
            if (this.showLookAt && this.lookAtVisualizer == null)
                Debug.LogWarning("'lookAtVisualizer' is null");
            this.lookAtVisualizer.SetActive(this.showLookAt);
        }
        //P  -> Start playback
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Starting Playback mode");
            this.recorder.SetState(RecorderState.PLAY);
        }
        //R  -> Start recording
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Starting Recording mode");
            this.recorder.SetState(RecorderState.RECORD);
        }
        //N  -> Stop recording/playback
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Stopping recording or playback");
            this.recorder.SetState(RecorderState.NONE);
        }

        //Update data
        if (this.showEyeLine)
            this.UpdateLineRenderer();
        if (this.showLookAt)
            this.UpdateVisualizer();
    }


    /**
     * Updates the Line Renderer
    */
    void UpdateLineRenderer()
    {
        var eyeHitpoint = this.eyeDataTracker.eyeHitpoint;
        var eyeTracked = this.eyeDataTracker.validTracking;

        // Render two positions - Head position and the eye hitpoint
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, head.transform.position);

        lineRenderer.material = (eyeTracked) ? trackMaterial : errorMaterial;
        if (eyeHitpoint != null)
            lineRenderer.SetPosition(1, eyeHitpoint);
    }

    /**
     * Updates the visualizer box if available
     */
    void UpdateVisualizer()
    {
        if (lookAtVisualizer == null)
            return;
        var eyeTracked = this.eyeDataTracker.validTracking;
        var eyeHitpoint = this.eyeDataTracker.eyeHitpoint;
        //Get the renderer and set the material
        Renderer renderer = lookAtVisualizer.GetComponent<Renderer>();
        renderer.material = (eyeTracked) ? trackMaterial : errorMaterial;
        //Update position
        lookAtVisualizer.transform.position = eyeHitpoint;
    }
}
