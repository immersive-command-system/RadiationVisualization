using System;

public interface DataSubscriber
{
    void OnReceiveMessage(float timestamp, string message);
}

public class RadiationDataSubscriber : DataSubscriber
{
    public DataServer server;

    void OnReceiveMessage(float timestamp, string message)
    {
        int x, y, z;
        float intensity;
        string[] values = message.Split(',');
        x = int.Parse(values[0]);
        y = int.Parse(values[1]);
        z = int.Parse(values[2]);
        intensity = float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat);

        server.RegisterDataSubscriber("Radiation", this);
    }
}

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

