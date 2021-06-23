using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerScript : MonoBehaviour
{
    public TTCP Network = new TTCP();
    public Button Server;
    public Button Connect;
    public InputField IPfield;
    public InputField Portfield;
    public string ServerIP;
    public int Port;
    bool isServer = false;
    public bool IsServer
    {
        get { return isServer; }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        Server.onClick.AddListener(Serv);
        Connect.onClick.AddListener(Con);
    }

    void Con() {
        ServerIP = IPfield.text;
        Port = Convert.ToInt32(Portfield.text);
        if(Network.Connect(ServerIP, Port))
            SceneManager.LoadScene("Load");

    }
    private void Serv()
    {
        isServer = true;
        Port = Convert.ToInt32(Portfield.text);
        if (Network.StartServer(Port, 1))
              SceneManager.LoadScene("Load");
    }
}
