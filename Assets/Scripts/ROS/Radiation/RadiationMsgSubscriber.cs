using ROSBridgeLib;
using System.Text;
using UnityEngine;

public class RadiationMsgSubscriber : MonoBehaviour
{

    private static string radiationFieldName = "E";
    private static string positionXFieldName = "x";
    private static string positionYFieldName = "y";
    private static string positionZFieldName = "z";


    public RadiationConnection data_source = null;
    public RNDataVisualizer visualizer = null;
    public static int verbose = 2;
    

    void Update()
    {
        if (visualizer == null)
        {
            visualizer = GetComponent<RNDataVisualizer>();
            if (visualizer == null)
            {
                visualizer = gameObject.AddComponent<RNDataVisualizer>();
            }
        }
        if (data_source == null)
        {
            data_source = GetComponent<RadiationConnection>();
            if (data_source != null)
            {
                data_source.AddSubscriber(HandleRNDMsg);
            }
        }
    }

    public void HandleRNDMsg(in RNDataMsg radiationMsg)
    {
        if (verbose > 1)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(radiationMsg.header.GetSeq());
            sb.Append(string.Format("\nWidth: {0} ({1})", radiationMsg.width, radiationMsg.data.Length / radiationMsg.point_step));
            sb.Append("\nFields:");
            foreach (RNPointFieldMsg field in radiationMsg.fields)
            {
                sb.Append("\n\t");
                sb.Append(field.ToString());
            }
            sb.Append("\nScalar Fields:");
            foreach (RNPointFieldMsg field in radiationMsg.scalar_fields)
            {
                sb.Append("\n\t");
                sb.Append(field.ToString());
            }
            Debug.Log(sb.ToString());
        }

        RNPointFieldMsg field_x = null, field_y = null, field_z = null,
            field_e = null, field_channel = null, field_detector = null;
        for (int i = 0; i < radiationMsg.fields.Length; i++)
        {
            if (radiationMsg.fields[i].name.Equals(positionXFieldName))
            {
                field_x = radiationMsg.fields[i];
            } else if (radiationMsg.fields[i].name.Equals(positionYFieldName))
            {
                field_y = radiationMsg.fields[i];
            } else if (radiationMsg.fields[i].name.Equals(positionZFieldName))
            {
                field_z = radiationMsg.fields[i];
            } else if (radiationMsg.fields[i].name.Equals(radiationFieldName))
            {
                field_e = radiationMsg.fields[i];
            } else if (radiationMsg.fields[i].name.Equals("channel"))
            {
                field_channel = radiationMsg.fields[i];
            } else if (radiationMsg.fields[i].name.Equals("detector_id"))
            {
                field_detector = radiationMsg.fields[i];
            }
        }
        if (field_x == null || field_y == null || field_z == null || field_e == null)
        {
            return;
        }
        
        int currStart = 0;
        float x, y, z, intensity;
        for (int i = 0; i < radiationMsg.width; i++, currStart += radiationMsg.point_step)
        {
            x = System.BitConverter.ToSingle(radiationMsg.data, currStart + field_x.offset);
            y = System.BitConverter.ToSingle(radiationMsg.data, currStart + field_y.offset);
            z = System.BitConverter.ToSingle(radiationMsg.data, currStart + field_z.offset);
            intensity = conformToFloat(radiationMsg.data, currStart, field_e);
            visualizer.AddRadiationPoint(new Vector3(x, y, z), intensity);
            if (verbose > 0)
            {
                Debug.Log(field_x.ToString() + "\n" + field_y.ToString() + "\n" + field_z.ToString() + "\n" + field_e.ToString());
                Debug.Log(string.Format("({0}, {1}, {2}): {3}", x, y, z, intensity));
                if (field_channel != null)
                {
                    Debug.Log(System.BitConverter.ToInt16(radiationMsg.data, currStart + field_channel.offset));
                }
                if (field_detector != null)
                {
                    Debug.Log(System.BitConverter.ToInt16(radiationMsg.data, currStart + field_detector.offset));
                }
            }
        }
    }

    private static float conformToFloat(byte[] data, int offset, RNPointFieldMsg field)
    {
        float x = float.NaN;
        if (field.datatype == RNPointFieldMsg.Datatype.FLOAT32)
        {
            x = System.BitConverter.ToSingle(data, offset + field.offset);
        }
        else if (field.datatype == RNPointFieldMsg.Datatype.FLOAT64)
        {
            x = (float)System.BitConverter.ToDouble(data, offset + field.offset);
        }
        else if (field.datatype == RNPointFieldMsg.Datatype.UINT8 || 
            field.datatype == RNPointFieldMsg.Datatype.INT8)
        {
            x = System.BitConverter.ToChar(data, offset + field.offset);
        }
        else if (field.datatype == RNPointFieldMsg.Datatype.UINT16)
        {
            x = System.BitConverter.ToUInt16(data, offset + field.offset);
        }
        else if (field.datatype == RNPointFieldMsg.Datatype.INT16)
        {
            x = System.BitConverter.ToInt16(data, offset + field.offset);
        }
        else if (field.datatype == RNPointFieldMsg.Datatype.UINT32)
        {
            x = System.BitConverter.ToUInt32(data, offset + field.offset);
        }
        else if (field.datatype == RNPointFieldMsg.Datatype.INT32)
        {
            x = System.BitConverter.ToInt32(data, offset + field.offset);
        }
        else if (field.datatype == RNPointFieldMsg.Datatype.INT64)
        {
            x = System.BitConverter.ToInt64(data, offset + field.offset);
        }
        return x;
    }
}
