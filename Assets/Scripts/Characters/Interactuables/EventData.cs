using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EventData
{
    public string id;
    public List<string> eventListPaths;
    public KeyValuePair<float[], int> position;

    internal static EventData CreateNew(string id, List<EventSO> eventList, KeyValuePair<Vector3, int> positions)
    {
        EventData eventData = new();
        eventData.SetData(id, eventList, positions);
        return eventData;
    }

    private void SetData(string id, List<EventSO> eventList, KeyValuePair<Vector3, int> positions)
    {
        this.id = id;
        eventListPaths = GetEventListPath(eventList);
        SetPosition(positions.Key, position.Value);
    }

    private List<string> GetEventListPath(List<EventSO> eventList)
    {
        if(eventList == null) return new();
        List<string> eventListPaths = new();
        for (int i = 0; i < eventList.Count; i++)
            eventListPaths.Add(GeneralManager.ToPath(eventList[i]));
        return eventListPaths;
    }

        public List<EventSO> GetEventList()
    {
        List<EventSO> eventList = new();
        for (int i = 0; i < eventList.Count; i++)
            eventList.Add((EventSO)GeneralManager.To(eventListPaths[i]));
        return eventList;
    }

    internal void SetPosition(Vector3 vectorPosition, int routeIndex)
    {
        float[] posVector = new float[] { vectorPosition.x, vectorPosition.y, vectorPosition.z };
        position = new(posVector, routeIndex);
    }
}
