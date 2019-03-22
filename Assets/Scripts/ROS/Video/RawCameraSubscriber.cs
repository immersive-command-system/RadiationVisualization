using ROSBridgeLib;
using ROSBridgeLib.sensor_msgs;
using SimpleJSON;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

public class RawCameraSubscriber : ROSBridgeSubscriber
{
    private static string objectName = "Camera Display";
    private static int num = 0;

    public static bool verbose = true;

    public new static string GetMessageTopic()
    {
        return "camera/image_raw";
    }

    public new static string GetMessageType()
    {
        return "sensor_msgs/Image";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new ImageMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        ImageMsg image = (ImageMsg)msg;

        int curr = num;
        num++;
        if (verbose)
        {
            Debug.Log("Start processing " + curr);
        }

        uint width = image.GetWidth();
        uint height = image.GetHeight();
        byte[] raw_data = image.GetImage();

        RawImage texObj = GameObject.Find(objectName).GetComponent<RawImage>();

        Texture2D tex = GameObject.Find(objectName).GetComponent<RawImage>().texture as Texture2D;
        if (tex == null)
        {
            tex = new Texture2D((int)width / 2, (int)height / 2, TextureFormat.RGBA32, false);
            texObj.texture = tex;
        }

        NativeArray<Color32> image_data = tex.GetRawTextureData<Color32>();
        if (image_data == null || image_data.Length * 4 != raw_data.Length)
        {
            tex = new Texture2D((int)width / 2, (int)height / 2, TextureFormat.RGBA32, false);
            texObj.texture = tex;
            image_data = tex.GetRawTextureData<Color32>();
        }

        for (int ind = 0, o_ind = raw_data.Length - 4; ind < image_data.Length; ind += 1, o_ind -= 4)
        {
            image_data[ind] = new Color32(raw_data[o_ind + 3], (byte)((raw_data[o_ind + 1] + raw_data[o_ind + 2]) / 2), raw_data[o_ind], 255);
        }
        
        //tex.LoadRawTextureData(image_data);
        tex.Apply();

        if (verbose)
        {
            Debug.Log("Finish processing " + curr);
        }

        //File.WriteAllBytes(Application.dataPath + "/Captures/" + curr + ".png", tex.EncodeToPNG());
    }
}