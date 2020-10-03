using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using UnityEngine;

/**
 * Generates an HeatMap for eye data
 */
[ExecuteInEditMode]
public class EyeHeatMap : MonoBehaviour
{
    public GameObject laptop;

    public Boolean update = false;
    private Boolean updateOldState = false;
    public string[] files = new string[0];

    public float scale = .02f;
    public HeatMapType mapType = HeatMapType.ANGLE;

    private static float hueStart = .5f;
    private static float hueEnd = .8f;


    // Start is called before the first frame update
    void Start()
    {
    }
     
    // Update is called once per frame
    void Update()
    {
        if (update != updateOldState)
        {
            updateOldState = update;
            if (update)
                Draw();
        }
    }

    /**
     * Draws the data
     */
    void Draw()
    {
        Debug.Log("Drawing " + files.Length + " items");
        var child = FetchChild(true);

        int fileIndex = 0;
        var valueMap = new Dictionary<float, int>();
        //Go through all files and snapshots
        foreach (string file in files)
        {
            GameObject empty = new GameObject();
            empty.name = file;
            empty.transform.SetParent(child.transform);
            var data = JsonUtility.FromJson<RecordData>(File.ReadAllText(file));
            int index = 0;

            int runState = 0;
            float angle = 55;

            bool laptopVisible = true;
            foreach (var snapshot in data.snapshots)
            {
                var cameraPosition = snapshot.positions.FindAll(p => p.position != Vector3.zero).Find(p => p.id == "camera");

                if (Vector3.Distance(cameraPosition.position, new Vector3(2, 1, 0)) < 3)
                {
                    runState = 0;
                    laptopVisible = true;
                }
                else if (Vector3.Distance(cameraPosition.position, laptop.transform.position) < 3)
                {
                    //Camera in area
                    Vector3 eyeDirection = snapshot.eyeDirection.normalized;
                    Vector3 objectDirection = (laptop.transform.position - cameraPosition.position).normalized;
                    float calculatedAngle = Vector3.Angle(eyeDirection, objectDirection);   //The angle as [0,180]
                    if (calculatedAngle > angle)
                        laptopVisible = false;
                    runState = 1;
                }
                else if (Vector3.Distance(cameraPosition.position, new Vector3(-3.7f, 1f, 6.7f)) < 3)
                {
                    //camera at end
                    if (runState != 2)
                        angle -= 5;
                    runState = 2;
                    laptopVisible = true;
                }


                if (Vector3.Distance(snapshot.eyeHitpoint, laptop.transform.position) > .5f)
                    continue;
                index++;
                var hitpoint = snapshot.eyeHitpoint;
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(empty.transform);
                sphere.transform.position = hitpoint;
                sphere.GetComponent<SphereCollider>().enabled = false;
                sphere.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
                float hue = 0;
                //Get hue bases on different types
                switch (this.mapType)
                {
                    case HeatMapType.ANGLE:
                        hue = GetHueFromAngle(snapshot);
                        break;
                    case HeatMapType.FILE:
                        hue = fileIndex / ((float)files.Length);
                        break;
                    case HeatMapType.VISIBLE:
                        hue = laptopVisible ? 0 : ((55 - angle) / 110f + .3f);
                        RaycastHit[] hit = Physics.RaycastAll(cameraPosition.position, snapshot.eyeDirection);
                        if ((hit[0].collider.gameObject == laptop && !laptopVisible))
                            sphere.transform.position = hit[1].point;
                        break;
                }
                float mapKey = hue;
                int value = valueMap.ContainsKey(mapKey) ? valueMap[mapKey] : 0;
                value++;
                valueMap[mapKey] = value;

                //float hue = ((hueEnd - hueStart) * (index / data.snapshots.Count)*100) + hueStart;
                //float hue = Math.Abs(1.80f - snapshot.eyeHitpoint.y) * 10;
                sphere.GetComponent<Renderer>().material.SetColor("_Color", Color.HSVToRGB(hue, 1f, 1f));
                index++;
            }
            fileIndex++;

        }
        Debug.Log("HeatMap Hue Dictionary: ");
        foreach (KeyValuePair<float, int> kvp in valueMap)
        {
            Debug.Log(kvp.Key + " := " + kvp.Value);
        }
    }

    private float GetHueFromAngle(Snapshot snapshot)
    {
        Vector3 eyeDirection = snapshot.eyeDirection;
        Vector3 objectDirection = (laptop.transform.position - snapshot.positions.Find(s => s.id == "camera").position).normalized;
        float calculatedAngle = Vector3.Angle(eyeDirection, objectDirection);   //The angle as [0,180]
        float hue = calculatedAngle / 60;
        return hue;
    }


    /**
     * Gets the child for the eyepoint, generates an new one if needed
     * Optional: Deletes the previous one
     */
    GameObject FetchChild(bool reset = false)
    {
        var childTransform = this.transform.Find("eye_heatmap");
        if (childTransform != null && reset)
        {
            DestroyImmediate(childTransform.gameObject);
            childTransform = null;
        }
        GameObject child;
        if (childTransform == null)
        {
            child = new GameObject();
            child.name = "eye_heatmap";
            child.transform.SetParent(this.transform);
        }
        else
        {
            child = childTransform.gameObject;
        }
        return child;
    }
}

public enum HeatMapType
{
    ANGLE,
    FILE,
    VISIBLE
}

public class UnityMap
{
    public float key;
    public int value;
}