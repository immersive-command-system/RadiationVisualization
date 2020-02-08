using SimpleJSON;

namespace ROSBridgeLib
{
    namespace cartographer_msgs
    {
        public class SubmapCloudMsg
        {
            public int submap_version { get; private set; }
            public float resolution { get; private set; }
            public bool finished { get; private set; }
            public PointCloud2Msg cloud { get; private set; }

            public SubmapCloudMsg(JSONNode node)
            {
                submap_version = node["submap_version"].AsInt;
                resolution = node["resolution"].AsFloat;
                finished = node["finished"].AsBool;
                cloud = new PointCloud2Msg(node["cloud"]);
            }
        }
    }
}