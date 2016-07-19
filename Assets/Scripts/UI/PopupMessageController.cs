using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupMessage
{
	public string m_title;
	public string m_message;
    public bool m_shown;

    public PopupMessage(string title, string message)
    {
        m_title = title;
        m_message = message;
        m_shown = false;
    }
}

public class PopupMessageController : MonoBehaviour 
{
    public GameObject panel;

    private Text textTitle;
    private Text textMessage;

    private Queue<PopupMessage> popupList;
    private static PopupMessageController instance;
    public static PopupMessageController Instance { get { return instance; } private set {} }


    void Awake()
    {
        instance = this;
        popupList = new Queue<PopupMessage>();
    }

	// Use this for initialization 
	void Start()
    {
        textTitle = panel.transform.FindChild("TextTitle").GetComponent<Text>();
	   	textMessage = panel.transform.FindChild("TextMessage").GetComponent<Text>();
        panel.SetActive(false);
	}

    void Update()
    {
        if (popupList.Count > 0)
        {
            PopupMessage popup = popupList.Peek();
           
            if (!popup.m_shown)
            {
                Show(popup);
            }
        }
    }

	void Show(PopupMessage popup) 
    {
        popup.m_shown = true;
        textTitle.text = popup.m_title;
        textMessage.text = popup.m_message;
        panel.SetActive(true);
       
        // pause the game
        Time.timeScale = 0;
	}
	
    public void OnPanelClick()
    {
        panel.SetActive(false);
        
    		// un pause the game
    	Time.timeScale = 1;

        popupList.Dequeue();
    }

    public void AddPopup(string title, string message)
    {
        PopupMessage popup = new PopupMessage(title, message);
        popupList.Enqueue(popup);
    }
}
