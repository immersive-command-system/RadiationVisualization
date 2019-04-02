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
            public SubmapEntryMsg[] submap_entries;

            public SubmapListMsg(JSONNode node)
            {
                this.header = new HeaderMsg(node["header"]);
                JSONNode entries = node["submap"];
                this.submap_entries = new SubmapEntryMsg[entries.Count];
                for (int i = 0; i < entries.Count; i++)
                {
                    this.submap_entries[i] = new SubmapEntryMsg(entries[i]);
                }
            }

            public SubmapListMsg(HeaderMsg header, SubmapEntryMsg[] submap)
            {
                this.header = header;
                this.submap_entries = submap;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (SubmapEntryMsg msg in submap_entries)
                {
                    sb.Append(msg.ToString());
                    sb.Append("\n");
                }
                return sb.ToString();
            }
        }
    }
}
