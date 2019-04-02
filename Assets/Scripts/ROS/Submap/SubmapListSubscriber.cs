using ROSBridgeLib;
using SimpleJSON;
using System.Text;
using UnityEngine;

using ROSBridgeLib.cartographer_msgs;

public class SubmapListSubscriber : ROSBridgeSubscriber
{

    public new static string GetMessageTopic()
    {
        return "submap_list";
    }

    public new static string GetMessageType()
    {
        return "cartographer_ros_msgs/SubmapList";
    }

    public new static SubmapListMsg ParseMessage(JSONNode msg)
    {
        return new SubmapListMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        SubmapListMsg submapList = (SubmapListMsg)msg;
        GameObject.Find("Submaps").GetComponent<SubmapManager>().HandleSubmapList(submapList);
    }
}
