using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;
// using TCPServer.cs;
public class XYZ : MonoBehaviour
{

    private List<float> x = new List<float>();
    private List<float> y = new List<float>();
    private List<float> z = new List<float>();
    private List<string> data = new List<string>();
    private TcpListener tcpListener;
    private TcpClient connectedTcpClient;
    private NetworkStream stream;
    private Byte[] bytes;
    private Thread tcpListenerThread;

    // Start is called before the first frame update
    void Start()
    {
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();

        //using (var reader = new StreamReader(@"\Users\ISAACS\Desktop\LBL\posXYZ.csv"))
        //{
        //    while (!reader.EndOfStream)
        //    {
        //        var line = reader.ReadLine();
        //        var values = line.Split(',');
        //        x.Add(float.Parse(values[0]));
        //        y.Add(float.Parse(values[1]));
        //        z.Add(float.Parse(values[2]));
        //    }
        //}
    }

    private void ListenForIncomingRequests()
    {
        try
        {
            // Create listener on localhost port 50007. 			
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50007);
            tcpListener.Start();
            Debug.Log("Server is listening");
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    // Get a stream object for reading 					
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 						
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message. 							
                            string clientMessage = Encoding.ASCII.GetString(incommingData);
                            data.Add(clientMessage);
                            Debug.Log("client message received as: " + clientMessage);
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    int i = 0;
    // Update is called once per frame
    void Update()
    {
        if (i >= data.Count)
        {
            return;
        }
        var values = data[i].Split(',');
        //        x.Add(float.Parse(values[0]));
        //        y.Add(float.Parse(values[1]));
        //        z.Add(float.Parse(values[2]));
        float cur_x = float.Parse(values[0].Substring(1));
        float cur_y = float.Parse(values[1]);
        float cur_z = float.Parse(values[2].Substring(0, values[2].Length - 1));
        Debug.Log("About to transform!");
        transform.position = new Vector3(cur_x, cur_y, cur_z);
        Debug.Log("transformed.");
        i = i + 1;
    }
}
