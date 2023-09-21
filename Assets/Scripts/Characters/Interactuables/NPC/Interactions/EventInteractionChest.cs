using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class EventInteractionChest : EventInteraction
{
    [Header("Parameters")]
    public ItemSO item;
    public int amount = 1;
    public bool autoGenerateMessage = true;
    [Header("Glosary")]
    public string prefix = "Found ";
    public string suffix = "!";
    public string customMessage = "Found Potion x1!";
    public string emptyMessage = "It`s empty";
    [Header("Audio")]
    public AudioClip chestOpenAudio;

    public override List<EventSO> GetEventList()
    {
        if (actionList == null || actionList.Count <= 0)
            return NewEventList();
        return actionList;
    }

    private List<EventSO> NewEventList()
    {
        List<EventSO> eventList = new()
        {
            NewAudioPlay(),
            NewSpriteChange(),
            NewMessage(),
            NewAddedItem(),
            NewChangeListEvent()
        };
        return eventList;
    }

    private EventSO NewAudioPlay()
    {
        return EventSO.NewAudioPlay(chestOpenAudio, AudioType.SOUND, -1);
    }

    private EventSO NewChangeListEvent()
    {
        List<MonologueSO> monologues = new()
            {
                MonologueSO.NewMonologue(false, new(){emptyMessage}, null, "")
            };
        return EventSO.NewChangeEventList(new() { EventSO.NewConversation(monologues) });
    }

    private EventSO NewSpriteChange()
    {
        KeyValuePair<EventSO.RouteKeys, float> chestOpening = new(EventSO.RouteKeys.LOOK, 0f);
        KeyValuePair<EventSO.RouteKeys, float> waitTime = new(EventSO.RouteKeys.WAIT, 0.3f);
        KeyValuePair<EventSO.RouteKeys, float> chestOpened = new(EventSO.RouteKeys.LOOK, 1f);

        List<KeyValuePair<EventSO.RouteKeys, float>> route = new()
        {
            chestOpening,
            waitTime,
            chestOpened
        };

        return EventSO.NewRoute(route.ToArray());
    }

    private EventSO NewAddedItem()
    {
        return EventSO.NewInventory(item, amount);
    }

    private EventSO NewMessage()
    {
        string message;
        if (autoGenerateMessage)
            message = $"{prefix}{item.Name} x{amount}{suffix}";
        else
            message = customMessage;

        List<MonologueSO> monologues = new()
            {
                MonologueSO.NewMonologue(false, new(){message}, null, "")
            };

        return EventSO.NewConversation(monologues);
    }
}
