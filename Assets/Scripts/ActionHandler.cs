using UnityEngine;
public class ActionHandler
{
    public static void HandleAction(JsonClass.JsonMessageClass jsonMessageObect)
    {
        Debug.Log("In handle action with " + jsonMessageObect.Action);
        switch (jsonMessageObect.Action)
        {
            case "matchSuccess":
                Debug.Log("Opponent found !");
                Board._canPlay = jsonMessageObect.FirstMove;
                if (Board._canPlay)
                    Board.currentMark = Mark.X;
                else
                    Board.currentMark = Mark.O;
                break;
            case "matchFail":
                ServerCommunication.FindMatch();
                break;
            case "move":
                PayloadBuffer.payloads.Enqueue(jsonMessageObect);
                Debug.Log(PayloadBuffer.payloads.Count);
                break;
            default:
                // code block
                break;
        }
    }
}
