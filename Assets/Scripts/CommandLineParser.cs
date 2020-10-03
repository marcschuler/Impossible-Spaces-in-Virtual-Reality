using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Recorder))]
[RequireComponent(typeof(PlaybackOptions))]
public class CommandLineParser : MonoBehaviour
{ 

    // Start is called before the first frame update
    void Start()
    {
        var args = System.Environment.GetCommandLineArgs();
        for(int n = 0; n < args.Length; n++)
        {
            if (args[n] == "--playback" && args.Length > n + 1)
            {
                loadData(args[n+1]);
            }
        }
    }

   void loadData(string path)
    {
        GetComponent<Recorder>().load(path);
    }

}
