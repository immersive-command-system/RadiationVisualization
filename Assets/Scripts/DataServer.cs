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

public class DataServer : MonoBehaviour
{
    public interface DataSubscriber
    {
        void OnReceiveMessage(float timestamp, string message);
    }

    public int[] listenPorts;

    public int line_count;

    private Dictionary<int, TcpListener> listeners = new Dictionary<int, TcpListener>();
    private List<Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder>> clientStreams;

    private ConcurrentQueue<Tuple<string, float, string>> parsedMessages = new ConcurrentQueue<Tuple<string, float, string>>();
    private Thread parseThread;

    private Dictionary<string, List<DataSubscriber>> subscribers = new Dictionary<string, List<DataSubscriber>>();
    private Thread subscriberThread;

    // Start is called before the first frame update
    void Start()
    {
        clientStreams = new List<Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder>>();

        parseThread = new Thread(new ThreadStart(ProcessMessages));
        parseThread.IsBackground = true;
        parseThread.Start();

        subscriberThread = new Thread(new ThreadStart(DelegateMessages));
        subscriberThread.IsBackground = true;
        subscriberThread.Start();

        foreach (int port in listenPorts)
        {
            ListenOnPort(port);
        }
    }

    public void RegisterDataSubscriber(string messageType, DataSubscriber sb)
    {
        List<DataSubscriber> currSubscribers;
        if (!subscribers.TryGetValue(messageType, out currSubscribers))
        {
            currSubscribers = new List<DataSubscriber>();
            subscribers.Add(messageType, currSubscribers);
        }
        if (!currSubscribers.Contains(sb))
        {
            Debug.Log("Registering listener for: " + messageType);
            currSubscribers.Add(sb);
        }
    }

    private void OnApplicationQuit()
    {
        foreach (KeyValuePair<int, TcpListener> curr in listeners)
        {
            curr.Value.Stop();
        }
        foreach (Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder> curr in clientStreams)
        {
            curr.Item1.Close();
        }
        parseThread.Abort();
    }

    private void ListenOnPort(int port)
    {
        if (port <= 0)
        {
            return;
        }
        TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
        listeners.Add(port, tcpListener);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), tcpListener);
        Debug.Log("Listening on " + port);
    }

    private void ClientConnectCallback(IAsyncResult ar)
    {
        TcpListener tcpListener = (TcpListener)ar.AsyncState;
        TcpClient client = tcpListener.EndAcceptTcpClient(ar);
        Debug.Log("Client connected");

        NetworkStream stream = client.GetStream();
        ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
        StringBuilder leftover = new StringBuilder();
        byte[] bytes = new byte[4096];
        Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder> state = 
            new Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder>(client, stream, bytes, messageQueue, leftover);
        lock (clientStreams)
        {
            clientStreams.Add(state);
        }
        
        stream.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(ReadMessages), state);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), tcpListener);
    }

    private void ReadMessages(IAsyncResult ar)
    {
        Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder> state = 
            (Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder>)ar.AsyncState;
        NetworkStream stream = state.Item2;
        int length = stream.EndRead(ar);
        byte[] bytes = state.Item3;
        // Read incomming stream into byte arrary.				
        if (length != 0)
        {
            ConcurrentQueue<string> messageQueue = state.Item4;
            // Convert byte array to string message. 							
            string clientMessage = Encoding.ASCII.GetString(bytes, 0, length);
            messageQueue.Enqueue(clientMessage);
            length = stream.Read(bytes, 0, bytes.Length);
        }
        stream.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(ReadMessages), state);
    }

    private void ProcessMessages()
    {
        char[] charSeparators = new char[] { '\n' };
        string fragment;
        while (true)
        {
            int count;
            lock (clientStreams)
            {
                count = clientStreams.Count;
            }
            
            for (int i = 0; i < count; i++)
            {
                Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder> curr;
                lock (clientStreams)
                {
                    curr = clientStreams[i];
                }
                
                ConcurrentQueue<string> messageQueue = curr.Item4;
                StringBuilder leftover = curr.Item5;
                if (messageQueue.TryDequeue(out fragment))
                {
                    string whole = leftover.Append(fragment).ToString();
                    if (whole.Length > 0)
                    {
                        string[] lines = whole.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                        leftover.Clear();
                        int end = lines.Length;
                        if (whole[whole.Length - 1] != '\n')
                        {
                            leftover.Append(lines[lines.Length - 1]);
                            end--;
                        }
                        for (int j = 0; j < end; j++)
                        {
                            string[] parts = lines[j].Split(':');
                            float timestamp;
                            if (parts.Length >= 3 && float.TryParse(parts[1], out timestamp))
                            {
                                parsedMessages.Enqueue(new Tuple<string, float, string>(parts[0], timestamp, parts[2]));
                                //Debug.Log("[" + timestamp + "]\t" + parts[0] + ": " + parts[2]);
                            }
                        }
                        line_count += end;
                    }
                }
            }
        }
    }

    private void DelegateMessages()
    {
        Tuple<string, float, string> currMessage;
        while (true)
        {
            if (parsedMessages.TryDequeue(out currMessage))
            {
                List<DataSubscriber> currSubscribers;
                if (subscribers.TryGetValue(currMessage.Item1, out currSubscribers))
                {
                    foreach (DataSubscriber sb in currSubscribers)
                    {
                        sb.OnReceiveMessage(currMessage.Item2, currMessage.Item3);
                    }
                }
            }
        }
    }
}
