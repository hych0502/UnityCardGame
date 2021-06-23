using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PacketQueue { 
    struct PacketInfo
    {
        public int offset;
        public int size;
    };
    //데이터를 보존할 버퍼
    private MemoryStream m_streamBuffer;
    //패킷 정보 관리 리스트
    private List<PacketInfo> m_offsetList;
    //메모리 배치 오프셋
    private int m_offset = 0;
    //락 오브젝트
    private object lockObj = new object();
    public PacketQueue()
    {
        m_streamBuffer = new MemoryStream();
        m_offsetList = new List<PacketInfo>();
    }
     public int Enqueue(byte[] data,int size)
    {
        PacketInfo info = new PacketInfo();
        //패킷정보 작성
        info.offset = m_offset;
        info.size = size;
        lock (lockObj)
        {
            //패킷 저장 정보 보존
            m_offsetList.Add(info);
            //패킷 데이터 보존
            m_streamBuffer.Position = m_offset;
            m_streamBuffer.Write(data, 0, size);
            m_streamBuffer.Flush();
            m_offset += size;
        }
        return size;
    }
    public int Dequeue(ref byte[] buffer,int size)
    {
        if(m_offsetList.Count <= 0)
        {
            return -1;
        }
        int recvSize = 0;
        lock (lockObj)
        {
            PacketInfo info = m_offsetList[0];
            int dataSize = Mathf.Min(size, info.size);
            m_streamBuffer.Position = info.offset;
            recvSize = m_streamBuffer.Read(buffer, 0, dataSize);
            //큐데이터를 꺼냈으므로 맨 앞데이터는 삭제
            if(recvSize > 0)
            {
                m_offsetList.RemoveAt(0);
            }
            //모든 큐 데이터를 꺼냈을 때는 스트림을 정리하여 메모리 절약
            if(m_offsetList.Count == 0)
            {
                
                m_offset = 0;
            }
        }
        return recvSize;
    }
    public void Clear()
    {
        byte[] buffer = m_streamBuffer.GetBuffer();
        Array.Clear(buffer, 0, buffer.Length);
        m_streamBuffer.Position = 0;
        m_streamBuffer.SetLength(0);
    }


}
