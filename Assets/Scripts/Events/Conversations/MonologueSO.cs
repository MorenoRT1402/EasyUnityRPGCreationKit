using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Create Monologue")]
public class MonologueSO : ScriptableObject
{
    public bool thisEvent;
    [TextArea] public List<string> message;

    [Header("If not this event")]
    public Sprite face;
    public string speakerName;

    public List<string> GetFormatedList()
    {
        return GetFormatedList(message);
    }

    internal static MonologueSO NewMonologue(bool thisEvent, List<string> message, Sprite face, string speakerName)
    {
        MonologueSO newMonologue = CreateInstance<MonologueSO>();
        newMonologue.SetData(thisEvent, message, face, speakerName);
        return newMonologue;
    }

    private void SetData(bool thisEvent, List<string> message, Sprite face, string speakerName)
    {
        this.thisEvent = thisEvent;
        this.message = GetFormatedList(message);
        this.face = face;
        this.speakerName = speakerName;
    }

    private static List<string> GetFormatedList(List<string> message)
    {
        List<string> formatedString = new();
        foreach (string paragraph in message)
        {
            //            Debug.Log($"New Paragraph: {NCalcManager.LocateVariables(paragraph)}");
            string newParagraph = NCalcManager.LocateVariables(paragraph);
            formatedString.Add(newParagraph);
        }
        return formatedString;
    }

    internal static MonologueSO New(MonologueSO monologue)
    {
        MonologueSO newMonologue = CreateInstance<MonologueSO>();
        newMonologue.Initialize(monologue, true);
        return newMonologue;
    }

    private void Initialize(MonologueSO other, bool formated)
    {
        if (other != null)
        {
            thisEvent = other.thisEvent;
            message = other.message;
            face = other.face;
            speakerName = other.speakerName;
        }
        if (formated)
            message = GetFormatedList();
    }
}
