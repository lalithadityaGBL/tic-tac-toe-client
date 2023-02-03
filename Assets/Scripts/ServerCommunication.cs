using UnityEngine;
public class ServerCommunication : MonoBehaviour
{

    public static void SendMove(int move)
    {
        string json = JsonClass.JsonMessageClass.GetJsonMessage(new JsonClass.JsonMessageClass("move", move, "Hey, I made my move", false));
        NetworkManager.GetNetworkManager.ws.Send(json);
    }
    public static void FindMatch()
    {
        string json = JsonClass.JsonMessageClass.GetJsonMessage(new JsonClass.JsonMessageClass("match", -1, "Hey, I need a match", false));
        NetworkManager.GetNetworkManager.ws.Send(json);
    }
    public static void GameOver()
    {
        string json = JsonClass.JsonMessageClass.GetJsonMessage(new JsonClass.JsonMessageClass("over", -1, "Hey, I the game is done", false));
        NetworkManager.GetNetworkManager.ws.Send(json);
    }
}
