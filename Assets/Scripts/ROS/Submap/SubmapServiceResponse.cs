using SimpleJSON;
using ROSBridgeLib.cartographer_msgs;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

public class SubmapServiceResponse : JSONServiceResponse {

    public static string GetServiceName() {
    	return "submap_cloud_query";
    }

    public static void JSONServiceCallBack(JSONNode node)
    {
        SubmapCloudMsg cloudMsg = new SubmapCloudMsg(node);
    }

        //public static void JSONServiceCallBack(JSONNode node) {
        //	StatusResponseMsg status = new StatusResponseMsg(node["status"]);
        //	int version = node["submap_version"].AsInt;
        //    int texCount = node["textures"].Count;

        //    Debug.Log("Decoding version " + version + " with " + texCount + " textures.");
        //    SubmapTextureMsg[] textures = new SubmapTextureMsg[texCount];
        //	for (int i = 0; i < texCount; i++) {
        //        Debug.Log("Texture: " + i);
        //		textures[i] = new SubmapTextureMsg(node["textures"][i]);
        //	}

        //    GameObject obj = GameObject.Find("Submap Connection");
        //    obj.GetComponent<SubmapConnection>().isUpdating = false;

        //    // Display textures
        //    RawImage display = obj.GetComponent<RawImage>();
        //    Texture2D tex = display.texture as Texture2D;
        //    if (tex == null || tex.width != textures[0].width || tex.height != textures[0].height)
        //    {
        //        display.texture = new Texture2D(textures[0].width, textures[0].height, TextureFormat.RGBA32, false);
        //        tex = display.texture as Texture2D;
        //    }

        //    NativeArray<Color32> pixels = tex.GetRawTextureData<Color32>();
        //    byte[] intensities = textures[0].GetIntensities();
        //    byte[] alphas = textures[0].GetAlphas();
        //    for (int i = 0; i < pixels.Length; i++)
        //    {
        //        pixels[i] = new Color32(intensities[i], intensities[i], intensities[i], alphas[i]);
        //    }
        //    tex.Apply();
        //}
    }