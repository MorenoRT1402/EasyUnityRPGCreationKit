using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Create Consequences")]
public class ConsequenceSO : ScriptableObject
{
    [SerializeField] private List<EventSO> consequenceEvents;

    public List<EventSO> ConsequenceEvents => consequenceEvents;

    internal EventSO GetEvent(int index)
    {
        if (consequenceEvents.Count > index)
        {
//            Debug.Log(consequenceEvents[index]);
            return consequenceEvents[index];
        }
        else
            return null;
    }

    internal void SetData(List<EventSO> eventList)
    {
        consequenceEvents = eventList;
    }
}
