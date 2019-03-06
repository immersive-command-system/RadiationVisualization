using SimpleJSON;
using ROSBridgeLib.std_msgs;
using UnityEngine;

namespace ROSBridgeLib
{
    namespace cartographer_msgs
    {
        public class OccupancyGridMsg : ROSBridgeMsg
        {
            private HeaderMsg _header;
            private MapMetaDataInfoMsg _info;
            private byte[] _data;

            public OccupancyGridMsg(JSONNode node)
            {
                _header = new HeaderMsg(node["header"]);
                _info = new MapMetaDataInfoMsg(node["info"]);
                JSONArray originalData = node["data"].AsArray;
                _data = new byte[originalData.Count];
                for (int i = 0; i < _data.Length; i++)
                {
                    _data[i] = (byte)originalData[i].AsInt;
                }
            }

            
            public HeaderMsg GetHeader()
            {
                return _header;
            }

            public MapMetaDataInfoMsg GetInfo()
            {
                return _info;
            }

            public byte[] GetData()
            {
                return _data;
            }
        }
    }
}
