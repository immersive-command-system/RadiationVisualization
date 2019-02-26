using System.Text;

using SimpleJSON;
using ROSBridgeLib.std_msgs;

namespace ROSBridgeLib
{
    namespace cartographer_msgs
    {
        public class SubmapListMsg : ROSBridgeMsg
        {
            public HeaderMsg header;
            public SubmapEntryMsg[] submap;

            public SubmapListMsg(JSONNode node)
            {
                this.header = new HeaderMsg(node["header"]);
                JSONNode entries = node["submap"];
                this.submap = new SubmapEntryMsg[entries.Count];
                for (int i = 0; i < entries.Count; i++)
                {
                    this.submap[i] = new SubmapEntryMsg(entries[i]);
                }
            }

            public SubmapListMsg(HeaderMsg header, SubmapEntryMsg[] submap)
            {
                this.header = header;
                this.submap = submap;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (SubmapEntryMsg msg in submap)
                {
                    sb.Append(msg.ToString());
                    sb.Append("\n");
                }
                return sb.ToString();
            }
        }
    }
}
