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
            public static byte[] JSONDataToBytes(JSONNode node)
            {
                return System.Convert.FromBase64String(node.Value);
            }

            public static byte[] JSONArrayToBytes(JSONArray array)
            {
                byte[] data = new byte[array.Count];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)array[i].AsInt;
                }
                return data;
            }
        }
    }
}