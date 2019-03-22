using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib.cartographer_msgs;

public class SubmapManager : MonoBehaviour
{
    private Dictionary<int, SubmapVisualizer> submaps = new Dictionary<int, SubmapVisualizer>();

    public void AddNewSubmap(SubmapCloudMsg msg)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = transform;
        obj.transform.position = new Vector3(msg.pose._position.GetX(), msg.pose._position.GetY(), msg.pose._position.GetZ());
        obj.transform.rotation = new Quaternion(msg.pose._orientation.GetX(), msg.pose._orientation.GetY(), msg.pose._orientation.GetZ(), msg.pose._orientation.GetW());
        SubmapVisualizer vis = obj.AddComponent<SubmapVisualizer>();
        submaps.Add(msg.submap_index, vis);
    }
}
