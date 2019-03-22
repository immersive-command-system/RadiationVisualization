using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * This file takes care of parsing messages about the Drone position that are received by the DataServer.
 * The data currently comes in a format of "x, y, z".
 */
public class DronePositionSubscriber : MonoBehaviour, DataServer.DataSubscriber
{
    // Attach DataServer object. If nonexistant, create an empty GameObject and attach the script `DataServer.cs`.
    public DataServer server;

    // Setting this to true will give a horizontal view of the data - This should be equal throughout all subscribers.
    public bool flipYZ = false;

    // Set to true to see the trail of the drone.
    public bool renderTrail = true;

    private Vector3 newPosition;
    private bool positionDidUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail == null && renderTrail == true)
        {
            // Deals with visualization of drone trail
            trail = gameObject.AddComponent<TrailRenderer>() as TrailRenderer;
            trail.widthMultiplier = 0.25f;
            trail.time = 10000;
            trail.material.color = new Color(0.0f, 1.0f, 1.0f, 0.5f);
        } else if (trail != null && renderTrail == false)
        {
            Destroy(trail);
        }

        // Called to attach as a subscriber to DataServer.
        server.RegisterDataSubscriber("Drone", this);
    }

    /* Parses and checks if message is corrupted. Stores data ready for visualization. */
    public void OnReceiveMessage(float timestamp, string message)
    {
        Debug.Log("Drone Received: " + message);
        string[] parts = message.Split(',');
        float x, y, z;
        if (parts.Length >= 3 && float.TryParse(parts[0], out x) && 
            float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z))
        {
            newPosition = (flipYZ) ? new Vector3(x, z, y) : new Vector3(x, y, z);
            positionDidUpdate = true;
        }
    }

    /* Update is called once per frame. If we see that we received a new xyz point for the data, we change the position. */
    void Update()
    {
        if (positionDidUpdate)
        {
            positionDidUpdate = false;
            transform.position = newPosition;
        }
    }
}
