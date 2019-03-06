using SimpleJSON;
using ROSBridgeLib.std_msgs;
using UnityEngine;
using ROSBridgeLib.custom_utils;

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
                //_data = ROSBridgeUtils.JSONArrayToBytes(node["data"].AsArray);
                _data = ROSBridgeUtils.JSONDataToBytes(node["data"]);
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
