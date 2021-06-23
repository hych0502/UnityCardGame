using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TTCP
{
    private Socket m_listener;
    private Socket m_socket;

    private bool m_isServer;

    public bool IsServer
    {
        get { return m_isServer; }
    }

    private bool m_isConnected;

    public bool IsConnected
    {
        get { return m_isConnected; }
    }

    protected bool m_threadloop = false;
    protected Thread m_thread = null;

    private List<int> SendList = new List<int>();
    private List<int> RecieveList = new List<int>();

    public bool Connect(string address, int port)
    {
        Debug.Log("connect called");
        if (m_listener != null)
            return false;
        bool ret = false;
        try
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.NoDelay = true;
            m_socket.Connect(address, port);
            ret = LaunchThread();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            m_socket = null;
            Application.Quit();
            Debug.Break();
        }
        if (ret == true)
        {
            m_isConnected = true;
            Debug.Log("connection success");
        }
        else
        {
            m_isConnected = false;
            Debug.Log("connection fail");
        }
        return m_isConnected;
    }
    public bool StartServer(int port, int conNum)
    {
        try
        {
            m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_listener.Bind(new IPEndPoint(IPAddress.Any, port));
            m_listener.Listen(conNum);
        }
        catch(System.Exception e)
        {
            Debug.Log(e);
            Application.Quit();
            return false;
        }
        m_isServer = true;
        return LaunchThread();
    }
    bool LaunchThread()
    {
        try
        {
            m_threadloop = true;
            m_thread = new Thread(new ThreadStart(Dispatch));
            m_thread.Start();

        }
        catch
        {
            Debug.Log("Thread Fail");
            return false;
        }
        return true;
    }
    public void Dispatch()
    {
        while (m_threadloop)
        {
            AcceptClient();
            if (m_socket != null && m_isConnected == true)
            {
                DispatchSend();
                DispatchRecieve();
            }
            Thread.Sleep(5);
        }
        Debug.Log("Dispatch thread ended");
    }
    void AcceptClient()
    {
        if (m_listener != null && m_listener.Poll(0, SelectMode.SelectRead))
        {
            m_socket = m_listener.Accept();
            m_isConnected = true;

        }
    }
    void DispatchSend()
    {
        try
        {
            if (m_socket.Poll(0, SelectMode.SelectWrite))
            {
                while (SendList.Count > 0)
                {
                    byte[] buffer = System.BitConverter.GetBytes(SendList[0]);
                    SendList.RemoveAt(0);
                    m_socket.Send(buffer, buffer.Length, SocketFlags.None);
                }
            }
        }
        catch
        {
            return;
        }
    }
    void DispatchRecieve()
    {
        try
        {
            while (m_socket.Poll(0, SelectMode.SelectRead))
            {
                byte[] buffer = new byte[1400];
                int recvSize = m_socket.Receive(buffer, buffer.Length, SocketFlags.None);
                if (recvSize == 0)
                {
                    Disconnect();
                }
                else if (recvSize > 0)
                {
                    RecieveList.Add(System.BitConverter.ToInt32(buffer, 0));
                    if (m_isServer)
                        Debug.Log("Server: " + RecieveList[RecieveList.Count - 1]);
                    else
                        Debug.Log("Client: " + RecieveList[RecieveList.Count - 1]);
                }
            }
        }
        catch
        {
            return;
        }
    }
    public void Disconnect()
    {
        m_isConnected = false;
        if (m_socket != null)
        {
            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();
            m_socket = null;

        }
    }
    public void StopServer()
    {
        m_threadloop = false;
        if (m_thread != null)
        {
            m_thread.Join();
            m_thread = null;
        }
        Disconnect();
        if (m_listener != null)
        {
            m_listener.Close();
            m_listener = null;
        }
        m_isServer = false;
    }
    public void Send(int a)
    {
        SendList.Add(a);
    }
    public int UpRecieve()
    {
        if (RecieveList.Count > 0)
        {
            int temp = RecieveList[0];
            RecieveList.RemoveAt(0);
            return temp;
        }
        else return -1;
    }
    ~TTCP()
    {
        if (m_isServer)
        {
            StopServer();
        }
    }
}