using SimpleJSON;
using ROSBridgeLib.cartographer_msgs;
using UnityEngine;

public class SubmapServiceResponse : JSONServiceResponse {

    public static string GetServiceName() {
    	return "submap_query";
    }

    public static void JSONServiceCallBack(JSONNode node) {
    	StatusResponseMsg status = new StatusResponseMsg(node["status"]);
        Debug.Log("Response Status\n\tCode: " + status.code + "\n\tMessage: " + status.message);
    	int version = node["submap_version"].AsInt;
        int texCount = node["textures"].Count;
        Debug.Log("Decoding version " + version + " with " + texCount + " textures.");
        SubmapTextureMsg[] textures = new SubmapTextureMsg[texCount];
    	for (int i = 0; i < texCount; i++) {
            Debug.Log("Texture: " + i);
    		textures[i] = new SubmapTextureMsg(node["textures"][i]);
    	}
        // Display textures
        Debug.Log("Number of textures: " + texCount);
        GameObject.Find("Submap Connection").GetComponent<SubmapConnection>().isUpdating = false;
    }
}