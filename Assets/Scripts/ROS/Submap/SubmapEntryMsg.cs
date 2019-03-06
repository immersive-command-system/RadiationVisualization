using System.Text;

using SimpleJSON;
using ROSBridgeLib.geometry_msgs;

namespace ROSBridgeLib
{
    namespace cartographer_msgs
    {
        public class SubmapEntryMsg : ROSBridgeMsg
        {
            public int trajectory_id;
            public int submap_index;
            public int submap_version;
            public PoseMsg pose;
            public bool is_frozen;

            public SubmapEntryMsg(JSONNode node)
            {
                this.trajectory_id = node["trajectory_id"].AsInt;
                this.submap_index = node["submap_index"].AsInt;
                this.submap_version = node["submap_version"].AsInt;
                this.pose = new PoseMsg(node["pose"]);
                this.is_frozen = node["is_frozen"].AsBool;
            }

            public SubmapEntryMsg(int trajectory_id, int submap_index, int submap_version, PoseMsg pose, bool is_frozen)
            {
                this.trajectory_id = trajectory_id;
                this.submap_index = submap_index;
                this.submap_version = submap_version;
                this.pose = pose;
                this.is_frozen = is_frozen;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Submap Index: {0}\n", submap_index);
                sb.AppendFormat("Submap Version: {0}\n", submap_version);
                sb.AppendFormat("Frozen: {0}\n", is_frozen);
                return sb.ToString();
            }
        }
    }
}
