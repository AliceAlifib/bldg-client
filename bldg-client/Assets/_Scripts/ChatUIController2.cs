
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Models;



public class ChatUIController2 : MonoBehaviour {

    public VerticalLayoutGroup listOfMessages;
    public TMP_InputField chatMessageInput;
    public GameObject chatMessageObj;


    private ResidentController rsdtController;
    private string residentAlias;


    List<SayAction> chatHistory = new List<SayAction>();
    Dictionary<string, bool> chatCache = new Dictionary<string, bool>();    // to know whether we've seen a msg before. key is speaker + time 
    private ScrollRect scrollRect;



    void OnEnable()
    {
        if (scrollRect == null) {
            scrollRect = gameObject.GetComponent<ScrollRect>();
        }
        chatMessageInput.onSubmit.AddListener(HandleNewMessage);
    }

    void OnDisable()
    {
        
        chatMessageInput.onSubmit.RemoveListener(HandleNewMessage);
    }

    // making this public to allow other controllers to generate & send chat messages
    public void HandleNewMessage(string text) {
        SayAction act = CreateChatMessage(text);
        AddToChatOutput(act); 
        SendChatMessage(act);
    }


    void AddToChatOutput(SayAction act)
    {
        // Clear Input Field
        chatMessageInput.text = string.Empty;
        List<SayAction> chatWithNewMessage = new List<SayAction>(chatHistory);
        chatWithNewMessage.Add(act);
        RedrawChat(chatWithNewMessage);
    }

    SayAction CreateChatMessage(string text) {
        // TODO extract recipient
        CurrentResidentController crc = CurrentResidentController.Instance;
        // Debug.Log("Found a rsdt controller");
        SayAction act = new SayAction {
            resident_email = crc.resident.email,
            action_type = "SAY",
            say_speaker = crc.resident.alias,
            say_text = text,
            say_flr = crc.resident.flr,
            say_flr_url = crc.resident.flr_url,
            say_location = crc.getResidentRelativeLocation(),
            say_mimetype = "text/plain",
            say_recipient = null
        };
        Debug.Log("~~~~~~~~~ Created chat message for " + act.say_speaker + " at " + act.say_location);
        return act;
    }


    void SendChatMessage(SayAction act) {
        // TODO extract recipient
        CurrentResidentController crc = CurrentResidentController.Instance;
        Debug.Log("~~~~~ Sending chat message from " + crc.resident.alias + " at " + act.say_location + " ~~~~~~~~");
        crc.SendSayAction(act);
    }
    

    public void SetResidentController(ResidentController controller) {
        rsdtController = controller;
        residentAlias = rsdtController.resident.alias;
    }

    string getMsgKey(SayAction msg) {
        return msg.say_speaker + msg.say_time;
    }

    public void AddMessageToHistory(string from, SayAction msg) {
        if (chatCache.ContainsKey(getMsgKey(msg))) {
            return;
        }
        chatCache.Add(getMsgKey(msg), true);
        chatHistory.Add(msg);
        chatHistory.Sort(delegate(SayAction m1, SayAction m2) {
            if (m1.say_time > m2.say_time) return 1;
            if (m1.say_time < m2.say_time) return -1;
            return 0;
        });

        RedrawChat(chatHistory);

        //GameObject newMessage = Instantiate(chatMessageObj, new Vector3 (0,0,0), Quaternion.identity) as GameObject;
        //newMessage.transform.parent = listOfMessages.transform;

        // TODO check if you have enough children - at least |chatHistory| - and create more if needed
        // TODO populte the content of the |chatHistory| items 
        // TODO SetActive to the populated children
    }


    void RedrawChat(List<SayAction> chat) {
        // add more children if needed
        if (chat.Count > listOfMessages.transform.childCount) {
            for (int j = listOfMessages.transform.childCount; j < chat.Count; j++) {
                GameObject newMessage = Instantiate(chatMessageObj, listOfMessages.transform) as GameObject;
                newMessage.gameObject.SetActive(false);
            }
        }

        // loop through children
        int i = 0;

        foreach (Transform child in listOfMessages.transform)
        {
            if (i < chat.Count) {
                SayAction msg = chat[i];
                child.gameObject.SetActive(true);
                ChatMessageController msgController = child.gameObject.GetComponent<ChatMessageController>();
                if (msgController != null) {
                    msgController.Clear();
                    msgController.SetMessage(msg.say_speaker, msg.say_text, msg.say_time);
                }
            } else {
                child.gameObject.SetActive(false);
            }
            i++;
        }
        scrollRect.verticalNormalizedPosition = 0;
    }


    public void ClearMessageHistory() {
        chatHistory = new List<SayAction>();
        chatCache = new Dictionary<string, bool>();

        foreach (Transform child in listOfMessages.transform)
        {
            // TODO clear content
            child.gameObject.SetActive(false);
        }
    }

}