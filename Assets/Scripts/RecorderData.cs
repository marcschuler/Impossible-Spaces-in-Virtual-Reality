using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * All available states: None, Record and Play
 */
public enum RecorderState
{
    NONE,
    RECORD,
    PLAY
}

/**
 * Recording date with a startpoint, the scene name and a list of snapshots
 */
[Serializable]
public class RecordData
{
    public long recordingStart = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
    public string scene;
    public List<Snapshot> snapshots = new List<Snapshot>();
}

/**
 * A snapshot - a single moment in time containing
 * - the time in ms
 * - eye data
 * - a list of all object data
 * - a list of events
 */
[Serializable]
public class Snapshot
{
    public long timeMs = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
    public Vector3 eyeDirection;
    public Vector3 eyeHitpoint;
    public bool eyeValid;
    public List<PositionData> positions = new List<PositionData>();
    public List<string> events = new List<string>();
}

/**
 * Contains the id, if the object is active, the position and rotation
 */
[Serializable]
public class PositionData
{
    public string id;
    public bool active;
    public Vector3 position;
    public Vector3 rotation;
}

public class Event
{
    private string name;
}