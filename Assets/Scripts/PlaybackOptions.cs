using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[RequireComponent(typeof(Recorder))]

public class PlaybackOptions: MonoBehaviour
{

    public Camera camera;
    public GameObject leftController;
    public GameObject rightController;

    public string playbackPath;

    // Start is called before the first frame update
    void Start()
    {
        if (playbackPath != null && playbackPath!="")
        {
            GetComponent<Recorder>().load(playbackPath);
        }

    }


    // Update is called once per frame
    void Update()
    {
    }

 
}
