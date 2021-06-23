using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
public enum NetEventType
{
    Connect = 0,
    Disconnect,
    SendError,
    RecieveError
}
public enum NetEventResult
{
    Failure = -1,
    Success = 0

}
public class NetEventState
{
    public NetEventType type;
    public NetEventResult result;
    public NetEventState() {}
    public NetEventState(NetEventType Type , NetEventResult Result)
    {
        type = Type;
        result = Result;
    }
}
public delegate void NetEventHandler(NetEventState state);
public class TransportTCP : ITransport
{
    private NetEventHandler m_handler;

    bool m_isServer = false;
    bool m_isConnected = false;
    public bool IsServer{
        get { return m_isServer; }
    }
    public bool IsConnected
    {
        get { return m_isConnected; }
    }
    Socket m_listener;
    Socket m_socket;

    private PacketQueue m_SendQueue = new PacketQueue();
    private PacketQueue m_recvQueue = new PacketQueue();

    public bool StartServer(int port, int connectionNum){
        //리스닝 소켓 생성
        m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_listener.Bind(new IPEndPoint(IPAddress.Any, port));
        m_listener.Listen(connectionNum);
        m_isServer = true;
        return true;

        }
    public void StopServer()
    {
        //리스닝 소켓 닫기
        m_listener.Close();
        m_listener = null;
        m_isServer = false;
    }
    public bool Connect(string address, int port)
    {
        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_socket.NoDelay = true;
        try
        {
            m_socket.Connect(address, port);
        }
        catch (SocketException e)
        {
            Debug.Log(e);
            
            return false;
        }
        m_socket.SendBufferSize = 0;
        m_isConnected = true;
        return true;
    }
    public bool Disconnect()
    {
        m_isConnected = false;
        if (m_socket != null)
        {
            //소켓을 닫는다.
            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();
            m_socket = null;
        }
        if(m_handler != null)
        {
            m_handler(new NetEventState(NetEventType.Disconnect, NetEventResult.Success));
        }

        return true;
    }
    public void AccecptClient()
    {
        if(m_listener != null && m_listener.Poll(0, SelectMode.SelectRead))
        {
            //클라이언트가 접속했다.
            m_socket = m_listener.Accept();
            m_isConnected = true;
            //이벤트 알림
            if(m_handler != null)
            {
                GameObject GM = GameObject.FindGameObjectWithTag("GM"); 
                NetEventState state = new NetEventState();
                state.type = NetEventType.Connect;
                state.result = NetEventResult.Success;
                GM.GetComponent<GameManager>().Status = Mode.PlayMode;
                m_handler(state);
            }
        }
    }
    public int Send(byte[] data, int size)
    {
        return m_SendQueue.Enqueue(data, size);
    }
    public int Recieve(ref byte[] buffer, int size)
    {
        return m_recvQueue.Dequeue(ref buffer, size);
    }
    //알림 델리게이트

    //이벤트 알림함수 등록
    public void RegisterEventHandler(NetEventHandler handler)
    {
        m_handler += handler;
    }
    //이벤트알림함수 삭제
    public void UnregisterEventHandler(NetEventHandler handler)
    {
        m_handler -= handler;
    }
}

public interface ITransport
{

}