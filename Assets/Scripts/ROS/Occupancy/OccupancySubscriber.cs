using ROSBridgeLib;
using SimpleJSON;
using System.Text;
using UnityEngine;
using ROSBridgeLib.cartographer_msgs;
using UnityEngine.UI;
using ROSBridgeLib.geometry_msgs;
using Unity.Collections;

public class OccupancySubscriber : ROSBridgeSubscriber
{
    private static string objectName = "Occupancy Display";

    public new static string GetMessageTopic()
    {
        return "map";
    }

    public new static string GetMessageType()
    {
        return "nav_msgs/OccupancyGrid";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new OccupancyGridMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        OccupancyGridMsg occupancy = (OccupancyGridMsg)msg;
        MapMetaDataInfoMsg meta = occupancy.GetInfo();

        int width = meta.GetWidth(), height = meta.GetHeight();
        float resolution = meta.GetResolution();
        PointMsg origin = meta.GetOrigin().GetPosition();
        QuaternionMsg pose = meta.GetOrigin().GetOrientation();

        Debug.Log("Width: " + width + "\nHeight: " + height + "\nResolution: " + resolution);

        RawImage texObj = GameObject.Find(objectName).GetComponent<RawImage>();
        Texture2D tex = texObj.texture as Texture2D;

        if (tex == null || tex.width != width || tex.height != height)
        {
            tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texObj.texture = tex;
        }

        NativeArray<Color32> pixels = tex.GetRawTextureData<Color32>();
        byte[] raw = occupancy.GetData();
        for (int i = 0; i < raw.Length; i++)
        {
            pixels[i] = new Color32((byte)(255 - raw[i]), (byte)(255 - raw[i]), (byte)(255 - raw[i]), (byte)Mathf.Min(255, 350 - raw[i]));
        }

        tex.Apply();
    }
}
