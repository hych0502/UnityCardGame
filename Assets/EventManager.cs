using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EventType
{
    e1
}
public class EventManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static EventManager Instance
    {
        get { return instance; }
        set { }
    }
    private static EventManager instance = null;

    public delegate void OnEvent(EventType eventType, Component Sender, object Param = null);
    private Dictionary<EventType, List<OnEvent>> Listeners = new Dictionary<EventType, List<OnEvent>>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(this);
    }

    public void AddListener(EventType eventType, OnEvent Listener)
    {
        List<OnEvent> ListenerList = null;
        //이벤트 형식키 검색 확인 
        if(Listeners.TryGetValue(eventType,out ListenerList))
        {
            ListenerList.Add(Listener);
            return;
        }
        ListenerList = new List<OnEvent>();
        ListenerList.Add(Listener);
        Listeners.Add(eventType, ListenerList);
    }
    public void PostNotification(EventType eventType,Component Sender,object Param = null)
    {
        List<OnEvent> ListenList = null;
        if (!Listeners.TryGetValue(eventType, out ListenList))
            return;
        

        for(int i = 0; i < ListenList.Count; i++){
            if (!ListenList[i].Equals(null))
                ListenList[i](eventType, Sender, Param);
        }
    }
    public void RemoveTrash()
    {
        Dictionary<EventType, List<OnEvent>> TmpListeners = new Dictionary<EventType, List<OnEvent>>();
        foreach (KeyValuePair<EventType, List<OnEvent>> Item in Listeners)
        {
            for (int i = Item.Value.Count - 1; i >= 0; i--)
                if (Item.Value[i].Equals(null))
                    Item.Value.RemoveAt(i);

            //알림받는 항목 임시딕셔너리
            if (Item.Value.Count > 0)
                TmpListeners.Add(Item.Key, Item.Value);
        }
        Listeners = TmpListeners;
    }
    /*
    private void OnLevelWasLoaded()
    {
        RemoveTrash();
    }
    */
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
