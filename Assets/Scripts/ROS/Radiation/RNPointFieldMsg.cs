using SimpleJSON;
using ROSBridgeLib.sensor_msgs;


namespace ROSBridgeLib
{
    public class RNPointFieldMsg : ROSBridgeMsg
    {
        public string name;
        public int offset;
        
        public enum Datatype
        {
            INT8 = PointFieldMsg.INT8,
            UINT8 = PointFieldMsg.UINT8,
            INT16 = PointFieldMsg.INT16,
            UINT16 = PointFieldMsg.UINT16,
            INT32 = PointFieldMsg.INT32,
            UINT32 = PointFieldMsg.UINT32,
            INT64 = 7,
            FLOAT32 = PointFieldMsg.FLOAT32 + 1,
            FLOAT64 = PointFieldMsg.FLOAT64 + 1
        }
        public Datatype datatype;
        public int count;

        public RNPointFieldMsg(JSONNode node)
        {
            name = node["name"];
            offset = node["offset"].AsInt;
            count = node["count"].AsInt;

            switch(node["datatype"].AsInt)
            {
                case PointFieldMsg.INT8:
                    datatype = Datatype.INT8;
                    break;
                case PointFieldMsg.UINT8:
                    datatype = Datatype.UINT8;
                    break;
                case PointFieldMsg.INT16:
                    datatype = Datatype.INT16;
                    break;
                case PointFieldMsg.UINT16:
                    datatype = Datatype.UINT16;
                    break;
                case PointFieldMsg.INT32:
                    datatype = Datatype.INT32;
                    break;
                case PointFieldMsg.UINT32:
                    datatype = Datatype.UINT32;
                    break;
                case 7:
                    datatype = Datatype.INT64;
                    break;
                case PointFieldMsg.FLOAT32 + 1:
                    datatype = Datatype.FLOAT32;
                    break;
                case PointFieldMsg.FLOAT64 + 1:
                    datatype = Datatype.FLOAT64;
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}({1}): {2}", name, count, datatype);
        }
    }
}
