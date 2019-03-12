using System;

public class CloudDataSubscriber : DataSubscriber
{
    public DataServer server;

    void OnReceiveMessage(float timestamp, string message)
    {
        float x, y, z;
        string[] values = message.Split(',');

        x = float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat);
        y = float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat);
        z = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat);

        server.RegisterDataSubscriber("Cloud", this);

    }
}
