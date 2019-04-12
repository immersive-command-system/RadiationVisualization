using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.custom_utils;

namespace ROSBridgeLib
{
    public class RNDataMsg : ROSBridgeMsg
    {
        public HeaderMsg header;

        public RNPointFieldMsg[] fields;
        public RNPointFieldMsg[] scalar_fields;

        public bool is_bigendian;
        public int point_step;
        public int scalar_step;
        public byte[] data;
        public int width;

        public bool is_dense;

        public RNDataMsg(JSONNode node)
        {
            header = new HeaderMsg(node["header"]);
            width = node["width"].AsInt;
            is_bigendian = node["is_bigendian"].AsBool;
            point_step = node["point_step"].AsInt;
            scalar_step = node["scalar_step"].AsInt;
            is_dense = node["is_dense"].AsBool;

            JSONArray fields_arr = node["fields"].AsArray;
            fields = new RNPointFieldMsg[fields_arr.Count];
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = new RNPointFieldMsg(fields_arr[i]);
            }
            fields_arr = node["scalar_fields"].AsArray;
            scalar_fields = new RNPointFieldMsg[fields_arr.Count];
            for (int i = 0; i < scalar_fields.Length; i++)
            {
                scalar_fields[i] = new RNPointFieldMsg(fields_arr[i]);
            }

            data = ROSBridgeUtils.ParseJSONRawData(node["data"]);
        }
    }
}

