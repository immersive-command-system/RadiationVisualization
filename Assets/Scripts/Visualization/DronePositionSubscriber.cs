using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePositionSubscriber : MonoBehaviour, DataServer.DataSubscriber
{

    public DataServer server;
    public bool renderTrail = true;
    public bool flipYZ = false;

    private Vector3 newPosition;
    private bool positionDidUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail == null && renderTrail == true)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
            trail.widthMultiplier = 0.5f;
            trail.time = 10000;
            trail.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        } else if (trail != null && renderTrail == false)
        {
            Destroy(trail);
        }

        server.RegisterDataSubscriber("Drone", this);
    }

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

    // Update is called once per frame
    void Update()
    {
        if (positionDidUpdate)
        {
            positionDidUpdate = false;
            transform.position = newPosition;
        }
    }
}
