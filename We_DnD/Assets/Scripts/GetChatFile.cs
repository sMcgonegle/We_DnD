using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(TwitchIRC))]
public class GetChatFile : MonoBehaviour
{
    private string pollTitle;
    public List<string> actionCommands;
    private string[] actionPoll;
    private GameObject[] textUI;
    public Text actionNum;
    private float animationTimer = 0;
    private bool animationFail = false;
    private TwitchIRC IRC;

    private void Start()
    {
        IRC = this.GetComponent<TwitchIRC>();
        actionCommands = new List<string>();
        actionPoll = new string[5];
        textUI = new GameObject[5];
        for (int i = 0; i < 5; i++)
            textUI[i] = GameObject.Find("Action Text " + (i+1));
    }

    public void ReadString()
    {
        string path = "C:/Streamlabs_Chatbot/Twitch_Chatlog.txt";
        StreamReader reader = new StreamReader(path);
        actionCommands = new List<string>();
        while(!reader.EndOfStream)
            actionCommands.Add(reader.ReadLine());
        reader.Close();
        pollTitle = actionCommands[0];
        Debug.Log(pollTitle);
        actionCommands.RemoveAt(0);
        ChooseActions();
    }

    public void WritePollOBS()
    {
        string path = "C:/Streamlabs_Chatbot/OBS_Poll.txt";
        StreamWriter writer = new StreamWriter(path);
        for (int i = 0; i < 5; i++)
        {
            if (actionPoll[i] != null)
                writer.WriteLine((i + 1) + ") " + actionPoll[i]);
            else
                writer.WriteLine("---------------");
        }
        writer.Close();
    }

    public void WritePollTwitchChat()
    {
        string poll = ("!poll start " + pollTitle + " ");
        for (int i = 0; i < 5; i++)
            if (actionPoll[i] != null)
                poll += "| " + actionPoll[i] + " ";
        IRC.SendMsg(poll);
        /*
                string path = "C:/Streamlabs_Chatbot/Twitch_Poll.txt";
                StreamWriter writer = new StreamWriter(path);
                writer.Write("!poll start " + pollTitle + " ");

                for (int i = 0; i < 5; i++)
                    if (actionPoll[i] != null || actionPoll[i] == "---------------")
                        writer.Write("| " + actionPoll[i] + " ");
                writer.Close();
                */
    }
    public void ChooseActions()
    {
        for (int i = 0; i < actionPoll.Length; i++)
        {
            try
            {
                int choice = UnityEngine.Random.Range(0, actionCommands.Count);
                actionPoll[i] = actionCommands[choice];
                actionCommands.RemoveAt(choice);
                SetUI(i);
            }
            catch (Exception e)
            {
                BadText(i);
            }
        }
    }

    public void ShuffleAction(int actionNumber)
    {
        if (actionPoll[actionNumber] != null)
        {
            try
            {
                int choice = UnityEngine.Random.Range(0, actionCommands.Count);
                actionPoll[actionNumber] = actionCommands[choice];
                actionCommands.RemoveAt(choice);
                SetUI(actionNumber);
            }
            catch (Exception e)
            {
                BadText(actionNumber);
            }
        }
    }

    public void ClearAction(int i)
    {
        actionPoll[i] = "---------------";
        SetUI(i);
    }

    private void SetUI(int i)
    {
        textUI[i].GetComponent<Text>().text = actionPoll[i];
    }

    private void BadText(int i)
    {
        textUI[i].GetComponent<Text>().fontStyle = FontStyle.Bold;
        textUI[i].GetComponent<Text>().color = Color.red;
        animationTimer = 20;
        animationFail = true;
    }
    
    private void GoodText()
    {
        for (int i = 0; i < textUI.Length; i++)
        {
            textUI[i].GetComponent<Text>().fontStyle = FontStyle.Normal;
            textUI[i].GetComponent<Text>().color = Color.black;
            animationFail = false;
        }
    }

    void Update()
    {
        actionNum.text = actionCommands.Count.ToString();

        if (Input.GetKeyDown("e"))
            ReadString();
        if (Input.GetKeyDown("1"))
            ShuffleAction(0);
        if (Input.GetKeyDown("2"))
            ShuffleAction(1);
        if (Input.GetKeyDown("3"))
            ShuffleAction(2);
        if (Input.GetKeyDown("4"))
            ShuffleAction(3);
        if (Input.GetKeyDown("5"))
            ShuffleAction(4);

        if (animationTimer > 0)
            animationTimer -= 1;
        if (animationTimer <= 0 && animationFail == true)
            GoodText();
    }
}