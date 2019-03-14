using System;

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

