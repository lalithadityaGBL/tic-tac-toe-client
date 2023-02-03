using System;
using UnityEngine;
using WebSocketSharp;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static NetworkManager instance;
    public static NetworkManager GetNetworkManager
    {
        get
        {
            if (instance == null)
            {
                GameObject nm = new GameObject { };
                instance = nm.AddComponent<NetworkManager>();
            }
            return instance;
        }
    }

    public WebSocket ws;

    JsonClass.JsonMessageClass jsonMessageObject = new JsonClass.JsonMessageClass();

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void EstablishWebSocketCommunication()
    {
        ws = new WebSocket("ws://localhost:8080/ws");
        ws.OnOpen += onOpenEventHandler;
        ws.OnMessage += onMessageRecieved;
        ws.OnError += onErrorEventHandler;
        ws.OnClose += onCloseEventHandler;
        ws.Connect();
    }

    //Event handlers
    private void onOpenEventHandler(object sender, EventArgs e)
    {
        Debug.Log("Connection Opened !");
        ServerCommunication.FindMatch();
    }
    private void onMessageRecieved(object sender, MessageEventArgs e)
    {
        JsonClass.JsonMessageClass jsonMessageObject = JsonUtility.FromJson<JsonClass.JsonMessageClass>(e.Data);
        ActionHandler.HandleAction(jsonMessageObject);
    }
    private void onErrorEventHandler(object sender, ErrorEventArgs e)
    {
        Debug.Log("Error log from on error event handler");
    }
    private void onCloseEventHandler(object sender, CloseEventArgs e)
    {
        Debug.Log("Connection closed !");
    }
}
