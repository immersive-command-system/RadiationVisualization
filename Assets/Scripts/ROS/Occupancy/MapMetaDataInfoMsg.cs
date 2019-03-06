using ROSBridgeLib;
using ROSBridgeLib.geometry_msgs;
using UnityEngine;
using SimpleJSON;

namespace ROSBridgeLib
{
    namespace cartographer_msgs
    {
        public class MapMetaDataInfoMsg : ROSBridgeMsg
        {
            private string _time;
            private float _resolution;
            private int _width;
            private int _height;
            private PoseMsg _origin;

            public MapMetaDataInfoMsg(JSONNode node)
            {
                _time = node["map_load_time"];
                _resolution = node["resolution"].AsFloat;
                _width = node["width"].AsInt;
                _height = node["height"].AsInt;
                _origin = new PoseMsg(node["origin"]);
            }


            public string GetTime()
            {
                return _time;
            }

            public float GetResolution()
            {
                return _resolution;
            }

            public int GetWidth()
            {
                return _width;
            }

            public int GetHeight()
            {
                return _height;
            }

            public PoseMsg GetOrigin()
            {
                return _origin;
            }
        }
    }
}

