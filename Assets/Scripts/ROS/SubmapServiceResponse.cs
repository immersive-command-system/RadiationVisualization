using SimpleJSON;
using ROSBridgeLib.cartographer_msgs;

public class SubmapServiceResponse : JSONServiceResponse {

    public static string GetServiceName() {
    	return "cartographer_ros_msgs/SubmapQuery";
    }

    public static void JSONServiceCallBack(JSONNode node) {
    	StatusResponseMsg status = new StatusResponseMsg(node["status"]);
    	int version = node["submap_version"].AsInt;
        int texCount = node["textures"].Count;
    	SubmapTextureMsg[] textures = new SubmapTextureMsg[texCount];
    	for (int i = 0; i < texCount; i++) {
    		textures[i] = new SubmapTextureMsg(node["textures"][i]);
    	}
    	// Display textures
    }
}