using ROSBridgeLib;
using SimpleJSON;
using System.Text;
using UnityEngine;

public class RadiationMsgSubscriber : ROSBridgeSubscriber
{

    private static bool verbose = true;

    public new static string GetMessageTopic()
    {
        return "interaction_data";
    }

    public new static string GetMessageType()
    {
        return "rntools/RNData";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new RNDataMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        RNDataMsg radiationMsg = (RNDataMsg)msg;
        if (verbose)
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
            foreach(RNPointFieldMsg field in radiationMsg.scalar_fields)
            {
                sb.Append("\n\t");
                sb.Append(field.ToString());
            }
            Debug.Log(sb.ToString());
        }
    }
}
