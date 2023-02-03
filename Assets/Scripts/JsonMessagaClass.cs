using System;
using UnityEngine;

public class JsonClass
{
    [Serializable]
    public class JsonMessageClass
    {
        public string Message;
        public string Action;
        public string Username;
        public int Move;
        public bool FirstMove;
        public JsonMessageClass() { }
        public JsonMessageClass(string action, int move, string msg, bool firstMove)
        {
            this.Action = action;
            this.Message = msg;
            this.Move = move;
            this.FirstMove = firstMove;
        }
        //Methods
        public static string GetJsonMessage(JsonClass.JsonMessageClass jsonMessageObject)
        {
            string json = JsonUtility.ToJson(jsonMessageObject);
            return json;
        }
    }
}
