using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPoll : MonoBehaviour {

    public List<string> actionList = new List<string>();
    private TwitchIRC IRC;

    void OnChatMsgRecieved(string msg)
    {
        Debug.Log("Recieve");
        if (msg.Contains("!action"))
        {
            int msgIndex = msg.IndexOf("PRIVMSG #");
            string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
            actionList.Add(msgString.Replace("!action ", ""));
        }
    }

    void Start ()
    {
        IRC = this.GetComponent<TwitchIRC>();
        //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
        IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
