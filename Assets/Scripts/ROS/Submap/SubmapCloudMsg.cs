using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using ROSBridgeLib.geometry_msgs;

namespace ROSBridgeLib
{
    namespace cartographer_msgs
    {
        public class SubmapCloudMsg
        {
            public int submap_index;
            public PoseMsg pose;
            public int submap_version;
            public float resolution;
            public bool finished;
            public PointCloud2Msg cloud;

            public SubmapCloudMsg(JSONNode node)
            {
                this.submap_version = node["submap_version"].AsInt;
                this.resolution = node["resolution"].AsFloat;
                this.finished = node["finished"].AsBool;
                this.cloud = new PointCloud2Msg(node["cloud"]);
            }
        }
    }
}