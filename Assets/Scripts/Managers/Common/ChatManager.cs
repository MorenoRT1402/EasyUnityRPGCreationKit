using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChatManager : Singleton<ChatManager>
{
    [Header("Parameters")]
    public float animateTextInterval = 0.03f;

    [Header("Components")]
    public GameObject chatGO;
    public GameObject chatSpeakerNameGO;
    public TextMeshProUGUI chatSpeakerNameTMP;
    public GameObject faceAndTextContainer;
    public Image chatSpeakerFaceImage;
    public TextMeshProUGUI chatMessageTMP;
    public GameObject choicesSpacer;

    [Header("Prefabs")]
    public GameObject choicePrefab;

    public bool Chatting => chatGO.activeSelf;
    public static bool Choosing;

    private UIManager UIM;
    private List<MonologueSO> actualConversation;
    private int actualMonologueIndex = 0;
    private int actualMessageIndex = 0;

    private bool animatedText = false;

    private void Start()
    {
        UIM = UIManager.Instance;
        SetMainPanels(false);

    }

    public void SetMainPanels(bool enabled)
    {
        UIM.SetGeneralTheme(chatSpeakerNameGO);
        UIM.SetGeneralTheme(faceAndTextContainer);
        UIM.SetGeneralTheme(choicesSpacer);

        chatGO.SetActive(enabled);
        choicesSpacer.SetActive(enabled);
    }

    private void Update()
    {
        ActionKey();
    }

    private void ActionKey()
    {
        if (!Chatting) return;
        if (Input.GetKeyDown(InputManager.Instance.action))
        {
            if (!animatedText)
            {
                //                Debug.Log("animating Text");
                chatMessageTMP.text = actualConversation[actualMonologueIndex].message[actualMessageIndex];
                animatedText = true;
            }
            else
            {
                //                Debug.Log("animated Text");
                actualMessageIndex++;
                Run(actualConversation);
            }
        }
    }

    internal void Run(List<MonologueSO> conversation)
    {
        actualConversation = GetFormatedConversation(conversation);
        chatGO.SetActive(true);

//        if(DebugManager.Instance.keyDebugs) Debug.Log($"KEY_DEBUG: {conversation.Count} > {actualMonologueIndex} ? {conversation.Count > actualMonologueIndex}");
        if (conversation.Count > actualMonologueIndex)
        {
            MonologueSO actualMonologue = conversation[actualMonologueIndex];
            UpdateUI(actualMonologue);
        }
        else
        {
            EndConversation();
        }
    }

    private List<MonologueSO> GetFormatedConversation(List<MonologueSO> conversation)
    {
        List<MonologueSO> formatedConversation = new();
        foreach(MonologueSO monologue in conversation){
            MonologueSO formatedMonologue = MonologueSO.New(monologue);
            formatedConversation.Add(formatedMonologue);
        }
        return formatedConversation;
    }

    public void EndConversation()
    {
        if (Choosing) return;

        actualConversation = null;
        actualMonologueIndex = 0;
        actualMessageIndex = 0;
        animatedText = false;
        chatGO.SetActive(false);
        if (EventManager.actualEventInteraction != null) EventManager.Instance.NextEvent();
    }

    private void UpdateUI(MonologueSO actualMonologue)
    {
        EventInteraction eventInteract = EventManager.actualEventInteraction;

        if (actualMonologue.message.Count > actualMessageIndex)
        {
            string speakerName = actualMonologue.speakerName;
            Sprite speakerFace = actualMonologue.face;
            if (eventInteract is EventInteractionNPC npc)
            {
                speakerName = actualMonologue.thisEvent ? npc.npcName : actualMonologue.speakerName;
                speakerFace = actualMonologue.thisEvent ? npc.npcSprite : actualMonologue.face;
            }
            List<string> message = actualMonologue.GetFormatedList();
            string paragraph = message[actualMessageIndex];
            UpdateUI(speakerName, speakerFace, paragraph);
        }
        else
        {
            actualMessageIndex = 0;
            actualMonologueIndex++;
            Run(actualConversation);
        }
    }

    internal void UpdateUI(string speakerName, Sprite speakerFace, string message)
    {
        chatSpeakerNameGO.SetActive(speakerName != null && speakerName != "");
        chatSpeakerNameTMP.text = speakerName ?? "Nameless";

        chatSpeakerFaceImage.gameObject.SetActive(speakerFace != null);
        chatSpeakerFaceImage.sprite = speakerFace;

        ShowAnimateText(message);
    }

    private void ShowAnimateText(string message)
    {
        StartCoroutine(AnimateText(message));
    }

    private IEnumerator AnimateText(string message)
    {
        animatedText = false;

        chatMessageTMP.text = "";
        char[] letters = message.ToCharArray();
//        Debug.Log(message);

        for (int i = 0; i < letters.Length; i++)
        {
            if (animatedText) yield break;

            //          Debug.Log($"{i}/{letters.Length}");
            chatMessageTMP.text += letters[i];
            yield return new WaitForSeconds(animateTextInterval);
        }
        animatedText = true;
    }

    internal void ShowChoices(List<string> choicesText, List<ConsequenceSO> choicesConsequences)
    {
        Choosing = true;
        SetMainPanels(true);

        InstanceChoices(choicesText, choicesConsequences);
    }

    private void InstanceChoices(List<string> choicesText, List<ConsequenceSO> choicesConsequences)
    {
        UIManager.Clear(choicesSpacer);

        for (int i = 0; i < choicesText.Count; i++)
        {
            GameObject choiceInstance = Instantiate(choicePrefab, choicesSpacer.transform);
            ButtonAndTMP choiceButton = choiceInstance.GetComponent<ButtonAndTMP>();
            List<EventSO> eventActions = new();
            if (choicesConsequences.Count > i)
                eventActions = choicesConsequences[i].ConsequenceEvents;
            choiceButton.UpdateUI(choicesText[i], eventActions);
        }
    }
}
