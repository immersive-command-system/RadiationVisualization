using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using SimpleJSON;

namespace ROSBridgeLib
{
    namespace custom_utils
    {
        public class ROSBridgeUtils
        {
            public static byte[] JSONDataToBytes(JSONNode node, bool is_bigendian = true)
            {
                byte[] data = System.Convert.FromBase64String(node.Value);
                if (!is_bigendian)
                {
                    System.Array.Reverse(data);
                }
                return data;
            }

            public static byte[] JSONArrayToBytes(JSONArray array, bool is_bigendian = true)
            {
                byte[] data = new byte[array.Count];
                if (is_bigendian)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = (byte)array[i].AsInt;
                    }
                }
                else
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[data.Length - i - 1] = (byte)array[i].AsInt;
                    }
                }
                
                return data;
            }

            public static byte[] ParseJSONRawData(JSONNode node, bool is_bigendian = true)
            {
                if (node.GetType() == typeof(JSONArray)) {
                    return JSONArrayToBytes(node.AsArray);
                } else
                {
                    return JSONDataToBytes(node);
                }
            }
        }
    }
}