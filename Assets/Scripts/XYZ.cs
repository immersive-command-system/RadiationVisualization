using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;
using System.Collections.Concurrent;

public class XYZ : MonoBehaviour
{
    public int listenPort = 50007;

    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private Thread tcpReaderThread;
    private List<Tuple<TcpClient, NetworkStream, ConcurrentQueue<string>, StringBuilder>> clientStreams;
    private ConcurrentQueue<Tuple<string, float, string>> parsedMessages = new ConcurrentQueue<Tuple<string, float, string>>();

    // Start is called before the first frame update
    void Start()
    {
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();

        tcpReaderThread = new Thread(new ThreadStart(ReadMessages));
        tcpReaderThread.IsBackground = true;
        tcpReaderThread.Start();
    }

    private void ListenForIncomingRequests()
    {
        try
        {
            // Create listener on localhost port 50007. 			
            tcpListener = new TcpListener(IPAddress.Any, listenPort);
            tcpListener.Start();
            Debug.Log("Listening on port " + listenPort);

            Byte[] bytes = new Byte[1024];
            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
                string leftover = new StringBuilder(); ;
                clientStreams.Add(new Tuple<TcpClient, NetworkStream, ConcurrentQueue<string>, StringBuilder>(client, stream, messageQueue, leftover));
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    private void ReadMessages()
    {
        byte[] bytes = new byte[4096];
        while (true)
        {
            for (int i = 0; i < clientStreams.Count; i++)
            {
                NetworkStream stream = clientStreams[i].Item2;
                ConcurrentQueue<string> messageQueue = clientStreams[i].Item3;
                int length = stream.Read(bytes, 0, bytes.Length);
                // Read incomming stream into byte arrary. 						
                if (length != 0)
                {
                    // Convert byte array to string message. 							
                    string clientMessage = Encoding.ASCII.GetString(bytes, 0, length);
                    messageQueue.Enqueue(clientMessage);
                }
            }
        }
    }

    private void ProcessMessages()
    {
        char[] charSeparators = new char[] { '\n' };
        string fragment;
        while (true)
        {
            for (int i = 0; i < clientStreams.Count; i++)
            {
                ConcurrentQueue<string> messageQueue = clientStreams[i].Item3;
                StringBuilder leftover = clientStreams[i].Item4;
                if (messageQueue.TryDequeue(out fragment))
                {
                    string whole = leftover.Append(fragment).ToString();
                    if (whole.IndexOf('\n') >= 0)
                    {
                        string[] lines = whole.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                        leftover.Clear();
                        if (whole[leftover.Length - 1] != '\n')
                        {
                            leftover.Append(lines[lines.Length - 1]);
                        }
                        for (int j = 0; j < lines.Length; j++)
                        {
                            string[] parts = lines[i].Split(':');
                            float timestamp;
                            if (parts.Length >= 3 && float.TryParse(parts[1], out timestamp))
                            {
                                parsedMessages.Enqueue(new Tuple<string, float, string>(parts[0], timestamp, parts[2]));
                            }
                        }
                    }
                }
            }
        }
    }
    

    void Update()
    {
    }
}
