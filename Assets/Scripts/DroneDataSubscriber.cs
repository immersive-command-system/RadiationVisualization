using System;

public class DroneDataSubscriber : DataSubscriber
{
    public DataServer server;

    void OnReceiveMessage(float timestamp, string message)
    {
        float x, y, z, rgb;
        string[] values = message.Split(',');
        x = float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat);
        y = float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat);
        z = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat);
        rgb = float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat);

        server.RegisterDataSubscriber("Drone", this);

    }
}
